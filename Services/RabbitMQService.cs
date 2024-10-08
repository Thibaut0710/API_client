﻿using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;

public class RabbitMQService : IRabbitMQService
{
    private readonly string _hostName;
    private readonly string _userName;
    private readonly string _password;
    private static ConcurrentDictionary<string, TaskCompletionSource<string>> _pendingMessages = new ConcurrentDictionary<string, TaskCompletionSource<string>>();
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMQService(IConfiguration configuration)
    {
        _hostName = configuration["RabbitMQ:HostName"];
        _userName = configuration["RabbitMQ:UserName"];
        _password = configuration["RabbitMQ:Password"];

        var factory = new ConnectionFactory() { HostName = _hostName, UserName = _userName, Password = _password };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public async Task<string> SendMessageAndWaitForResponseAsync(string message, string CommandQueueName, string ReplyQueueName)
    {
        _channel.QueueDeclare(queue: ReplyQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        await Task.Delay(100);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += OnResponseReceived;
        _channel.BasicConsume(consumer: consumer, queue: ReplyQueueName, autoAck: true);
        Console.WriteLine(ReplyQueueName);


        var correlationId = Guid.NewGuid().ToString();
        var tcs = new TaskCompletionSource<string>();

        _pendingMessages[correlationId] = tcs;

        var properties = _channel.CreateBasicProperties();
        properties.CorrelationId = correlationId;
        properties.ReplyTo = ReplyQueueName;

        var body = Encoding.UTF8.GetBytes(message);
        Console.WriteLine(CommandQueueName);
        _channel.BasicPublish(exchange: string.Empty,
                             routingKey: CommandQueueName, // Utilisation de la queue de commande en dur
                             basicProperties: properties,
                             body: body);

        Console.WriteLine($" [x] Sent {message} with CorrelationId {correlationId}");

        // Attendre de manière asynchrone que la réponse arrive
        return await tcs.Task;
    }

    private void OnResponseReceived(object sender, BasicDeliverEventArgs ea)
    {
        var correlationId = ea.BasicProperties?.CorrelationId;

        if (string.IsNullOrEmpty(correlationId))
        {
            Console.WriteLine("Received a message without a CorrelationId or with a null CorrelationId.");
            return;
        }

        var response = Encoding.UTF8.GetString(ea.Body.ToArray());

        Console.WriteLine($" [x] Received {response} with CorrelationId {correlationId}");

        // Vérifier si le message attendu est dans la liste
        if (_pendingMessages.TryRemove(correlationId, out var tcs))
        {
            if (tcs != null)
            {
                tcs.SetResult(response); // Renvoie la réponse à la méthode appelante
            }
            else
            {
                Console.WriteLine($"TaskCompletionSource for CorrelationId {correlationId} is null.");
            }
        }
        else
        {
            Console.WriteLine($"No pending message found for CorrelationId {correlationId}.");
        }
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}
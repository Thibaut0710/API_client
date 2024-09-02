public interface IRabbitMQService
{
    Task<string> SendMessageAndWaitForResponseAsync(string message, string CommandQueueName, string ReplyQueueName);

}
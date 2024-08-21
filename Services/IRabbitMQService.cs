public interface IRabbitMQService
{
    Task<string> SendMessageAndWaitForResponseAsync(string message);

}
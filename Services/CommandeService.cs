using API_Commande.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json;

namespace API_Client.Services
{
    public class CommandeService
    {
        private readonly HttpClient _httpClient;
        private readonly IRabbitMQService _rabbitMQService;

        public CommandeService(HttpClient httpClient, IRabbitMQService rabbitMQService)
        {
            _httpClient = httpClient;
            _rabbitMQService = rabbitMQService;
        }

        // Appeler l'API_Commande pour récupérer les commandes liées à un client
        public async Task<string> GetOrdersByClientId(int clientId)
        {
            Console.WriteLine(clientId);
            //var response = await _httpClient.GetAsync($"Commande/client/{clientId}");
            string jsonString = JsonSerializer.Serialize(new { Id = clientId });
            var response = await _rabbitMQService.SendMessageAndWaitForResponseAsync(jsonString);
            if (response == null)
            {
                return "Aucune commande trouvée pour ce client.";
            }

            return response;
            /*Console.WriteLine(response);

            if (!response.IsSuccessStatusCode)
            {
                return null; // ou gérer l'erreur selon les besoins
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response Content: {responseContent}");

            var orders = JsonSerializer.Deserialize<List<Commande>>(responseContent, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            });*/


        }
    }
}

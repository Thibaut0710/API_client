
using System.Text.Json;

namespace API_Client.Services
{
    public class ClientIdModel
    {
        public int Id { get; set; }
        public ClientIdModel(int id)
        {
            Id = id;
        }
    }
    public class ClientService
    {
        private readonly IRabbitMQService _rabbitMQService;

        public ClientService(IRabbitMQService rabbitMQService)
        {
            _rabbitMQService = rabbitMQService;
        }

        // Appeler l'API_Commande pour récupérer les commandes liées à un client
        public async Task<string> GetOrdersByClientId(int clientId)
        {
            Console.WriteLine(clientId);
            //var response = await _httpClient.GetAsync($"Commande/client/{clientId}");
            var clientIdObject = new ClientIdModel(clientId);
            string jsonString = JsonSerializer.Serialize(clientIdObject);
            var response = await _rabbitMQService.SendMessageAndWaitForResponseAsync(jsonString, "Channel_Commande", "Channel_Client");
            var res = JsonSerializer.Deserialize<List<Dictionary<string,object>>>(response);
            Console.WriteLine($"Response: {response}");
            if (response == null)
            {
                return "Aucune commande trouvée pour ce client.";
            }

            return response;

        }


        public async Task<string> GetOrderByClient(int clientId,int commandeId)
        {
            Console.WriteLine(clientId);
            //var response = await _httpClient.GetAsync($"Commande/client/{clientId}");
            string jsonString = JsonSerializer.Serialize(new { clientId = clientId, commandeId = commandeId });
            var response = await _rabbitMQService.SendMessageAndWaitForResponseAsync(jsonString,"channelCommmandeClient","channel_client_commande_id"); ;
            if (response == null)
            {
                return "Aucune commande trouvée pour ce client.";
            }

            return response;

        }

        public async Task<string> GetOrderByClientWithProduit(int clientId, int commandeId)
        {
            Console.WriteLine(clientId);
            //var response = await _httpClient.GetAsync($"Commande/client/{clientId}");
            string jsonString = JsonSerializer.Serialize(new { clientId = clientId, commandeId = commandeId });
            var response = await _rabbitMQService.SendMessageAndWaitForResponseAsync(jsonString, "channelCommandeClientProduit", "channel_client_commande_produit"); ;
            if (response == null)
            {
                return "Aucune commande trouvée pour ce client.";
            }

            return response;

        }
    }
}

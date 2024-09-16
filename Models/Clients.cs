namespace API_Client.Models
{
    public class Clients
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public List<int> CommandeIds { get; set; }
    }
    public class ClientIdModel
    {
        public int Id { get; set; }
        public ClientIdModel(int id)
        {
            Id = id;
        }
    }
}

using API_Client.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Client.Context
{
    public class ClientsContext : DbContext
    {
        public ClientsContext(DbContextOptions<ClientsContext> options) : base(options) { }

        public DbSet<Clients> Customers { get; set; }
    }
}

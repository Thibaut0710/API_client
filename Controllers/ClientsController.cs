using API_Client.Context;
using API_Client.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using API_Client.Services;

namespace API_Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly ClientsContext _context;
        private readonly CommandeService _commandeService;

        public ClientsController(ClientsContext context, CommandeService commandeService)
        {
            _context = context;
            _commandeService = commandeService;
        }

        // GET: api/customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Clients>>> GetCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        // GET: api/customers/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Clients>> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }

        // POST: api/customers
        [HttpPost]
        public async Task<ActionResult<Clients>> PostCustomer(Clients customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }

        // GET: api/clients/{id}/commandes
        [HttpGet("{id}/commandes")]
        public async Task<IActionResult> GetClientWithOrders(int id)
        {
            // Récupérer le client
            var client = await _context.Customers.FindAsync(id);
            if (client == null)
            {
                return NotFound(new { message = "Client non trouvé." });
            }

            // Appeler l'API_Commande pour récupérer les commandes liées à ce client
            var response = await _commandeService.GetOrdersByClientId(id); // Utilisez 'await' ici
            if (response == null)
            {
                return NotFound(new { message = "Aucune commande trouvée pour ce client." });
            }

            // Retourner le client avec ses commandes
            return Ok(new
            {
                Client = client,
                Commandes = response
            });
        }


        // PUT: api/customers/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Clients customer)
        {
            if (id != customer.Id)
            {
                return BadRequest();
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { message = "Client mis à jour avec succès." });
        }

        // DELETE: api/customers/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Client supprimé avec succès." });
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}

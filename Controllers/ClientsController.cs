using API_Client.Context;
using API_Client.Models;
using API_Client.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        // GET: api/clients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Clients>>> GetCustomers()
        {
            return await _context.Customers.AsNoTracking().ToListAsync();
        }

        // GET: api/clients/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Clients>> GetCustomer(int id)
        {
            var customer = await _context.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
            {
                return NotFound(new { message = "Client non trouvé." });
            }

            return Ok(customer);
        }

        // POST: api/clients
        [HttpPost]
        public async Task<ActionResult<Clients>> PostCustomer([FromBody] Clients customer)
        {
            if (customer == null)
            {
                return BadRequest(new { message = "Les informations du client ne peuvent pas être nulles." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
        }

        // GET: api/clients/{id}/commandes
        [HttpGet("{id}/commandes")]
        public async Task<IActionResult> GetClientWithOrders(int id)
        {
            var client = await _context.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            if (client == null)
            {
                return NotFound(new { message = "Client non trouvé." });
            }

            try
            {
                var response = await _commandeService.GetOrdersByClientId(id);
                if (string.IsNullOrEmpty(response))
                {
                    return NotFound(new { message = "Aucune commande trouvée pour ce client." });
                }

                return Ok(new
                {
                    Client = client,
                    Commandes = response
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la récupération des commandes.", detail = ex.Message });
            }
        }

        // PUT: api/clients/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, [FromBody] Clients customer)
        {
            if (id != customer.Id)
            {
                return BadRequest(new { message = "L'ID du client ne correspond pas à l'ID dans l'URL." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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
                    return NotFound(new { message = "Client non trouvé." });
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la mise à jour du client.", detail = ex.Message });
            }

            return Ok(new { message = "Client mis à jour avec succès." });
        }

        // DELETE: api/clients/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound(new { message = "Client non trouvé." });
            }

            _context.Customers.Remove(customer);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur lors de la suppression du client.", detail = ex.Message });
            }

            return Ok(new { message = "Client supprimé avec succès." });
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}

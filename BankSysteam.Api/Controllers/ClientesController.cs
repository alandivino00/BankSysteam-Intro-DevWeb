using Microsoft.AspNetCore.Mvc;
using BankSysteam.Api.models;
using BankSysteam.Api.data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;

namespace BankSysteam.Api.Controllers
{
    [ApiController]
    [Route("api/clientes")]
    public class ClientesController : ControllerBase
    {
        private readonly BankContext _context;

       
        public ClientesController(BankContext context)
        {
            _context = context;
        }
        

        // POST: criar novo cliente
        [HttpPost]
        public async Task<ActionResult<Cliente>> CreateCliente([FromBody] Cliente input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Basic uniqueness check on email (optional)
            var exists = await _context.Clients.AsNoTracking().AnyAsync(c => c.Email == input.Email);
            if (exists)
            {
                ModelState.AddModelError(nameof(input.Email), "Email já cadastrado.");
                return BadRequest(ModelState);
            }

            var cliente = new Cliente
            {
                Nome = input.Nome,
                Email = input.Email
            };

            await _context.Clients.AddAsync(cliente);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCliente), new { id = cliente.Id }, cliente);
        }

        // GET: api/clientes/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Cliente>> GetCliente(Guid id)
        {
            var cliente = await _context.Clients.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            if (cliente == null) return NotFound();
            return Ok(cliente);
        }

        // GET: api/clientes/{id}/contas  -> lista contas do cliente (eager loading)
        [HttpGet("{id:guid}/contas")]
        public async Task<ActionResult> GetContasDoCliente(Guid id, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            if (page <= 0 || size <= 0)
                return BadRequest("Parameters 'page' and 'size' must be greater than zero.");

            var cliente = await _context.Clients
                .AsNoTracking()
                .Include(c => c.Conta)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cliente == null) return NotFound();

            var contas = cliente.Conta
                .Skip((page - 1) * size)
                .Take(size);

            return Ok(contas);
        }
    }
}
    
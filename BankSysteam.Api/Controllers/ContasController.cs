using Microsoft.AspNetCore.Mvc;
using BankSysteam.Api.Models;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using BankSysteam.Api.models;
using BankSysteam.Api.data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BankSysteam.Api.Controllers
{
    [ApiController]
    [Route("api/contas")]
    public class ContasController(BankContext context) : ControllerBase
    {
        private readonly BankContext _context = context;
        
        // Lista em memória (ajustada para string no Numero)
        private static readonly List<Conta> contas = new List<Conta>
        {
            new Conta("1", 1200.50m),
            new Conta("2", 3500.75m),
            new Conta("3", 800.00m)
        };

        // GET: retorna todas as contas (como ViewModel) com paginação e ordenação
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContaViewModel>>> GetContas(
            [FromQuery] int page = 1,
            [FromQuery] int size = 10,
            [FromQuery] string orderBy = "numero",
            [FromQuery] string sort = "asc")
        {
            if (page <= 0 || size <= 0)
                return BadRequest("Parameters 'page' and 'size' must be greater than zero.");

            var query = _context.Contas.AsNoTracking().AsQueryable();

            var order = (orderBy ?? "numero").ToLowerInvariant();
            var direction = (sort ?? "asc").ToLowerInvariant();

            switch (order)
            {
                case "saldo":
                    query = direction == "desc"
                        ? query.OrderByDescending(c => c.Saldo)
                        : query.OrderBy(c => c.Saldo);
                    break;
                case "datacriacao":
                    query = direction == "desc"
                        ? query.OrderByDescending(c => c.DataCriacao)
                        : query.OrderBy(c => c.DataCriacao);
                    break;
                case "tipo":
                    query = direction == "desc"
                        ? query.OrderByDescending(c => c.Tipo)
                        : query.OrderBy(c => c.Tipo);
                    break;
                default: // numero
                    query = direction == "desc"
                        ? query.OrderByDescending(c => c.Numero)
                        : query.OrderBy(c => c.Numero);
                    break;
            }

            var vms = await query
                .Skip((page - 1) * size)
                .Take(size)
                .Select(c => new ContaViewModel
                {
                    Numero = c.Numero,
                    Titular = string.Empty,
                    Saldo = c.Saldo
                })
                .ToListAsync();

            return Ok(vms);
        }

        // GET: retorna uma conta por número 
        [HttpGet("{numero}")]
        public async Task<ActionResult<ContaViewModel>> GetConta(string numero)
        {
            var conta = await _context.Contas
                .Include(c => c.Cliente)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Numero == numero);

            if (conta == null)
                return NotFound();

            var vm = new ContaViewModel
            {
                Numero = conta.Numero,
                Titular = string.Empty,
                Saldo = conta.Saldo
            };

            return Ok(vm);
        }

        // POST: cria nova conta, valida e retorna 201 com Location
        [HttpPost]
        public async Task<ActionResult<ContaViewModel>> CreateConta([FromBody] ContaInputModel input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Verifica se cliente existe
            var clienteExists = await _context.Clients.AsNoTracking().AnyAsync(c => c.Id == input.ClienteId);
            if (!clienteExists)
            {
                ModelState.AddModelError(nameof(input.ClienteId), "Cliente não existe.");
                return BadRequest(ModelState);
            }

            // Verifica se já existe conta com mesmo número
            var exists = await _context.Contas.AsNoTracking().AnyAsync(c => c.Numero == input.Numero);
            if (exists)
            {
                ModelState.AddModelError(nameof(input.Numero), "Número de conta já existe.");
                return BadRequest(ModelState);
            }

            var conta = new Conta(input.Numero, input.Saldo, input.ClienteId);

            await _context.Contas.AddAsync(conta);
            await _context.SaveChangesAsync();

            var vm = new ContaViewModel
            {
                Numero = conta.Numero,
                Saldo = conta.Saldo
            };

            return CreatedAtAction(nameof(GetConta), new { numero = conta.Numero }, vm);
        }

        // PUT: acrescenta saldo na conta especificada
        [HttpPut("{numero}/deposito")]
        public async Task<ActionResult<ContaViewModel>> Deposito(string numero, [FromBody] DepositoInputModel input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Buscar a conta no banco para que fique rastreada pelo EF
            var conta = await _context.Contas.FirstOrDefaultAsync(c => c.Numero == numero);
            if (conta == null)
                return NotFound(); // 404 quando conta não encontrada

            conta.Saldo += input.Valor;

            // Persiste a alteração no banco
            await _context.SaveChangesAsync();

            var vm = new ContaViewModel
            {
                Numero = conta.Numero,               
                Saldo = conta.Saldo
            };

            return Ok(vm); // 200 com a conta atualizada
        }

        // DELETE: remove conta por número
        [HttpDelete("{numero}")]
        public async Task<IActionResult> DeleteConta(string numero)
        {
            // Buscar a conta no banco
            var conta = await _context.Contas.FirstOrDefaultAsync(c => c.Numero == numero);
            if (conta == null)
                return NotFound(); // 404 quando não encontrada

            _context.Contas.Remove(conta);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 quando removida com sucesso
        }
    }
}
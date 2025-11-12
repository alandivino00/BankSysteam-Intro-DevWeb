using BankSysteam.Api.Models;
using BankSysteam.Api.Service;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankSysteam.Api.Controllers
{
    [ApiController]
    [Route("api/contas")]
    public class ContasController(IContaService service) : ControllerBase
    {
        private readonly IContaService _service = service;

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

            var (items, total) = await _service.GetContasAsync(page, size, orderBy, sort);

            Response.Headers["X-Total-Count"] = total.ToString();

            return Ok(items);
        }

        // GET: retorna uma conta por número 
        [HttpGet("{numero}")]
        public async Task<ActionResult<ContaViewModel>> GetConta(string numero)
        {
            var vm = await _service.GetContaAsync(numero);
            if (vm == null)
                return NotFound();

            return Ok(vm);
        }

        // POST: cria nova conta, valida e retorna 201 com Location
        [HttpPost]
        public async Task<ActionResult<ContaViewModel>> CreateConta([FromBody] ContaInputModel input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var vm = await _service.CreateContaAsync(input);
                return CreatedAtAction(nameof(GetConta), new { numero = vm.Numero }, vm);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return BadRequest(ModelState);
            }
        }

        // PUT: acrescenta saldo na conta especificada
        [HttpPut("{numero}/deposito")]
        public async Task<ActionResult<ContaViewModel>> Deposito(string numero, [FromBody] DepositoInputModel input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var vm = await _service.DepositoAsync(numero, input.Valor);
            if (vm == null)
                return NotFound();

            return Ok(vm);
        }

        // DELETE: remove conta por número
        [HttpDelete("{numero}")]
        public async Task<IActionResult> DeleteConta(string numero)
        {
            var ok = await _service.DeleteContaAsync(numero);
            if (!ok)
                return NotFound();

            return NoContent();
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using BankSysteam.Api;
using BankSysteam.Api.Models;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace BankSysteam.Api.Controllers
{
    [ApiController]
    [Route("api/contas")]
    public class ContasController : ControllerBase
    {
        // Lista em memória simulando dados do banco
        private static readonly List<Conta> contas = new List<Conta>
        {
            new Conta(1, "João", 1200.50m),
            new Conta(2, "Maria", 3500.75m),
            new Conta(3, "Ana", 800.00m)
        };

        // GET: retorna todas as contas (como ViewModel)
        [HttpGet]
        public ActionResult<IEnumerable<ContaViewModel>> GetContas()
        {
            var vms = contas.Select(c => new ContaViewModel
            {
                Numero = c.Numero,
                Titular = c.Titular,
                Saldo = c.Saldo
            });

            return Ok(vms);
        }

        // GET: retorna uma conta por número
        [HttpGet("{numero}")]
        public ActionResult<ContaViewModel> GetConta(int numero)
        {
            var conta = contas.FirstOrDefault(c => c.Numero == numero);
            if (conta == null)
                return NotFound();

            var vm = new ContaViewModel
            {
                Numero = conta.Numero,
                Titular = conta.Titular,
                Saldo = conta.Saldo
            };

            return Ok(vm);
        }

        // POST: cria nova conta, valida e retorna 201 com Location
        [HttpPost]
        public ActionResult<ContaViewModel> CreateConta([FromBody] ContaInputModel input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var exists = contas.Any(c => c.Numero == input.Numero);
            if (exists)
            {
                ModelState.AddModelError(nameof(input.Numero), "Número de conta já existe.");
                return BadRequest(ModelState);
            }

            var conta = new Conta(input.Numero, input.Titular, input.Saldo);
            contas.Add(conta);

            var vm = new ContaViewModel
            {
                Numero = conta.Numero,
                Titular = conta.Titular,
                Saldo = conta.Saldo
            };

            return CreatedAtAction(nameof(GetConta), new { numero = conta.Numero }, vm);
        }

        // PUT: acrescenta saldo na conta especificada
        [HttpPut("{numero}/deposito")]
        public ActionResult<ContaViewModel> Deposito(int numero, [FromBody] DepositoInputModel input)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var conta = contas.FirstOrDefault(c => c.Numero == numero);
            if (conta == null)
                return NotFound(); // 404 quando conta não encontrada          

            conta.Saldo += input.Valor;

            var vm = new ContaViewModel
            {
                Numero = conta.Numero,
                Titular = conta.Titular,
                Saldo = conta.Saldo
            };

            return Ok(vm); // 200 com a conta atualizada
        }

        // DELETE: remove conta por número
        [HttpDelete("{numero}")]
        public IActionResult DeleteConta(int numero)
        {
            var conta = contas.FirstOrDefault(c => c.Numero == numero);
            if (conta == null)
                return NotFound(); // 404 quando não encontrada

            contas.Remove(conta);
            return NoContent(); // 204 quando removida com sucesso
        }
    }
}
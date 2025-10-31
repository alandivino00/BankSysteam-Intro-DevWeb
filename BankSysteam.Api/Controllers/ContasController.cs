using Microsoft.AspNetCore.Mvc;
using BankSysteam.Api;
using BankSysteam.Api.Models;
using System.Collections.Generic;

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

        // Ação GET: retorna a lista de contas
        [HttpGet]
        public ActionResult<IEnumerable<Conta>> GetContas()
        {
            return Ok(contas);
        }

        // Ação POST: recebe InputModel, adiciona e retorna ViewModel
        [HttpPost]
        public ActionResult<ContaViewModel> CreateConta([FromBody] ContaInputModel input)
        {
            if (!ModelState.IsValid)
            {
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

            return CreatedAtAction(nameof(GetContas), null, vm);
        }
    }
}
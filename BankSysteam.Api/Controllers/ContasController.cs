using Microsoft.AspNetCore.Mvc;

namespace BankSysteam.Api.Controllers
{
    [ApiController]
    [Route("api/contas")]
    public class ContasController : ControllerBase
    {
        // Lista em memória simulando dados do banco
        private static readonly List<string> contas = new List<string>
        {
            "Conta Corrente - João",
            "Conta Poupança - Maria",
            "Conta Salário - Ana"
        };

        // Ação GET: retorna a lista de contas
        [HttpGet]
        public ActionResult<IEnumerable<string>> GetContas()
        {
            return Ok(contas);
        }
    }
}
using BankSysteam.Api.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankSysteam.Api.Service
{
    public interface IContaService
    {
        Task<(IEnumerable<ContaViewModel> Items, int Total)> GetContasAsync(int page = 1, int size = 10, string orderBy = "numero", string sort = "asc");
        Task<ContaViewModel?> GetContaAsync(string numero);
        Task<ContaViewModel> CreateContaAsync(ContaInputModel input);
        Task<ContaViewModel?> DepositoAsync(string numero, decimal valor);
        Task<bool> DeleteContaAsync(string numero);
    }
}

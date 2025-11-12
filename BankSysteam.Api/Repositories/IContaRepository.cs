using BankSysteam.Api.models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankSysteam.Api.Repositories
{
    public interface IContaRepository
    {
        Task<(IEnumerable<Conta> Items, int TotalCount)> GetAllAsync(int page, int size, string orderBy, string sort);
        Task<Conta?> GetByNumeroAsync(string numero, bool asNoTracking = true);
        Task<bool> ExistsByNumeroAsync(string numero);
        Task AddAsync(Conta conta);
        Task UpdateAsync(Conta conta);
        Task DeleteAsync(Conta conta);
        Task<bool> ClienteExistsAsync(Guid clienteId);
    }
}

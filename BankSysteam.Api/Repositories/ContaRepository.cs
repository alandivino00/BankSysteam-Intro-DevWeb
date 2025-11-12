using BankSysteam.Api.data;
using BankSysteam.Api.models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BankSysteam.Api.data;
using BankSysteam.Api.models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankSysteam.Api.Repositories
{
    public class ContaRepository : IContaRepository
    {
        private readonly BankContext _context;

        public ContaRepository(BankContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Conta conta)
        {
            await _context.Contas.AddAsync(conta);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Conta conta)
        {
            _context.Contas.Remove(conta);
            await _context.SaveChangesAsync();
        }

        public async Task<Conta?> GetByNumeroAsync(string numero, bool asNoTracking = true)
        {
            var query = _context.Contas.AsQueryable();

            if (asNoTracking)
                query = query.AsNoTracking();

            return await query.Include(c => c.Cliente)
                              .FirstOrDefaultAsync(c => c.Numero == numero);
        }

        public async Task<(IEnumerable<Conta> Items, int TotalCount)> GetAllAsync(int page, int size, string orderBy, string sort)
        {
            var query = _context.Contas.AsNoTracking().AsQueryable();

            var order = (orderBy ?? "numero").ToLowerInvariant();
            var direction = (sort ?? "asc").ToLowerInvariant();

            switch (order)
            {
                case "saldo":
                    query = direction == "desc" ? query.OrderByDescending(c => c.Saldo) : query.OrderBy(c => c.Saldo);
                    break;
                case "datacriacao":
                    query = direction == "desc" ? query.OrderByDescending(c => c.DataCriacao) : query.OrderBy(c => c.DataCriacao);
                    break;
                case "tipo":
                    query = direction == "desc" ? query.OrderByDescending(c => c.Tipo) : query.OrderBy(c => c.Tipo);
                    break;
                default:
                    query = direction == "desc" ? query.OrderByDescending(c => c.Numero) : query.OrderBy(c => c.Numero);
                    break;
            }

            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();

            return (items, total);
        }

        public async Task UpdateAsync(Conta conta)
        {
            _context.Contas.Update(conta);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByNumeroAsync(string numero)
        {
            return await _context.Contas.AsNoTracking().AnyAsync(c => c.Numero == numero);
        }

        public async Task<bool> ClienteExistsAsync(Guid clienteId)
        {
            return await _context.Clients.AsNoTracking().AnyAsync(c => c.Id == clienteId);
        }
    }
}

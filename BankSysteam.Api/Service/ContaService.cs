using BankSysteam.Api.Models;
using BankSysteam.Api.Repositories;
using BankSysteam.Api.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankSysteam.Api.Service
{
    public class ContaService : IContaService
    {
        private readonly IContaRepository _repo;

        public ContaService(IContaRepository repo)
        {
            _repo = repo;
        }

        public async Task<(IEnumerable<ContaViewModel> Items, int Total)> GetContasAsync(int page = 1, int size = 10, string orderBy = "numero", string sort = "asc")
        {
            if (page <= 0 || size <= 0)
                throw new ArgumentException("Parameters 'page' and 'size' must be greater than zero.");

            var (items, total) = await _repo.GetAllAsync(page, size, orderBy, sort);

            var vms = items.Select(c => new ContaViewModel
            {
                Numero = c.Numero,
                Titular = string.Empty,
                Saldo = c.Saldo
            }).ToList();

            return (vms, total);
        }

        public async Task<ContaViewModel?> GetContaAsync(string numero)
        {
            var conta = await _repo.GetByNumeroAsync(numero);
            if (conta == null) return null;

            return new ContaViewModel
            {
                Numero = conta.Numero,
                Titular = string.Empty,
                Saldo = conta.Saldo
            };
        }

        public async Task<ContaViewModel> CreateContaAsync(ContaInputModel input)
        {
            // validações de negócio
            var clienteExists = await _repo.ClienteExistsAsync(input.ClienteId);
            if (!clienteExists)
                throw new InvalidOperationException("Cliente não existe.");

            var exists = await _repo.ExistsByNumeroAsync(input.Numero);
            if (exists)
                throw new InvalidOperationException("Número de conta já existe.");

            var conta = new Conta(input.Numero, input.Saldo, input.ClienteId);

            await _repo.AddAsync(conta);

            return new ContaViewModel
            {
                Numero = conta.Numero,
                Saldo = conta.Saldo
            };
        }

        public async Task<ContaViewModel?> DepositoAsync(string numero, decimal valor)
        {
            if (valor <= 0)
                throw new ArgumentException("Valor de depósito deve ser maior que zero.", nameof(valor));

            var conta = await _repo.GetByNumeroAsync(numero, asNoTracking: false);
            if (conta == null) return null;

            conta.Saldo += valor;
            await _repo.UpdateAsync(conta);

            return new ContaViewModel
            {
                Numero = conta.Numero,
                Saldo = conta.Saldo
            };
        }

        public async Task<bool> DeleteContaAsync(string numero)
        {
            var conta = await _repo.GetByNumeroAsync(numero);
            if (conta == null) 
                throw new InvalidOperationException("Conta não encontrada.");

            await _repo.DeleteAsync(conta);
            return true;
        }
    }
}

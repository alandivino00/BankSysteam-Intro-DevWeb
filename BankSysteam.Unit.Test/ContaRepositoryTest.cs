using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankSysteam.Api.data;
using BankSysteam.Api.models;
using BankSysteam.Api.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BankSysteam.Unit.Test
{
    public class ContaRepositoryTest
    {
        private static BankContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<BankContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new BankContext(options);
        }

        [Fact]
        public async Task AddAsync_DeveAdicionarConta_QuandoContaExiste()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            await using var context = CreateContext(dbName);
            var repo = new ContaRepository(context);

            var conta = new Conta("123", 100m);

            // Act
            await repo.AddAsync(conta);

            // Assert
            var persisted = await context.Contas.AsNoTracking().FirstOrDefaultAsync(c => c.Numero == "123");
            Assert.NotNull(persisted);
            Assert.Equal(100m, persisted.Balance);
        }

        [Fact]
        public async Task GetByNumeroAsync_DeveRetornarContaComCliente_QuandoExistir()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            await using var context = CreateContext(dbName);

            var cliente = new Cliente { Id = Guid.NewGuid(), Nome = "Cliente A", Email = "a@example.com" };
            var conta = new Conta("999", 250m, cliente.Id) { Cliente = cliente };
           
            await context.Clients.AddAsync(cliente);
            await context.Contas.AddAsync(conta);
            await context.SaveChangesAsync();

            var repo = new ContaRepository(context);

            // Act
            var result = await repo.GetByNumeroAsync("999");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("999", result!.Numero);
            Assert.Equal(250m, result.Saldo);
            Assert.NotNull(result.Cliente);
            Assert.Equal("Cliente A", result.Cliente.Nome);
        }

        [Fact]
        public async Task ExistsByNumeroAsync_DeveRetornarVerdadeiro_QuandoExisti_E_Falso_CasoContrario()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            await using var context = CreateContext(dbName);
            var repo = new ContaRepository(context);

            var conta = new Conta("E1", 1m);
            await context.Contas.AddAsync(conta);
            await context.SaveChangesAsync();

            // Act
            var exists = await repo.ExistsByNumeroAsync("E1");
            var notExists = await repo.ExistsByNumeroAsync("Nope");

            // Assert
            Assert.True(exists);
            Assert.False(notExists);
        }

        [Fact]
        public async Task ClienteExistsAsync_DeveRetornarVerdadeiro_QuandoClienteExistir()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            await using var context = CreateContext(dbName);
            var repo = new ContaRepository(context);

            var clienteId = Guid.NewGuid();
            await context.Clients.AddAsync(new Cliente { Id = clienteId, Nome = "X", Email = "x@x.com" });
            await context.SaveChangesAsync();

            // Act
            var exists = await repo.ClienteExistsAsync(clienteId);

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task GetAllAsync_DeveRetornarResultadoPaginadoEContTotal_QuandoContaExiste()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            await using var context = CreateContext(dbName);

            // seed 5 contas with numeros 1..5
            var contas = Enumerable.Range(1, 5)
                                   .Select(i => new Conta(i.ToString(), i * 10m))
                                   .ToList();

            await context.Contas.AddRangeAsync(contas);
            await context.SaveChangesAsync();

            var repo = new ContaRepository(context);

            // Act
            // page 2, size 2 -> should return items 3 and 4 (ordered by numero asc)
            var (items, total) = await repo.GetAllAsync(page: 2, size: 2, orderBy: "numero", sort: "asc");

            // Assert
            Assert.Equal(5, total);
            Assert.Equal(2, items.Count());
            Assert.Contains(items, c => c.Numero == "3");
            Assert.Contains(items, c => c.Numero == "4");
        }

        [Fact]
        public async Task UpdateAsync_DevePersistirAtualizações_QuandoAtualizaConta()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            await using var context = CreateContext(dbName);

            var conta = new Conta("UPD", 50m);
            await context.Contas.AddAsync(conta);
            await context.SaveChangesAsync();

            var repo = new ContaRepository(context);

            // Act
            conta.Saldo = 150m; // updates Balance via alias
            await repo.UpdateAsync(conta);

            // Assert
            var updated = await context.Contas.AsNoTracking().FirstOrDefaultAsync(c => c.Numero == "UPD");
            Assert.NotNull(updated);
            Assert.Equal(150m, updated.Balance);
        }

        [Fact]
        public async Task DeleteAsync_DeveRemoverConta_QuandoContaExiste()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            await using var context = CreateContext(dbName);

            var conta = new Conta("DEL", 20m);
            await context.Contas.AddAsync(conta);
            await context.SaveChangesAsync();

            // Act
            var repo = new ContaRepository(context);

            await repo.DeleteAsync(conta);

            // Assert
            var exists = await context.Contas.AnyAsync(c => c.Numero == "DEL");
            Assert.False(exists);
        }
    }
}

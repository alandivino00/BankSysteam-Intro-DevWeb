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
        private static ContaRepository GetBankContextInMemory()
        {
            var options = new DbContextOptionsBuilder<BankContext>()
                .UseInMemoryDatabase("teste")
                .Options;
            var dbContext = new BankContext(options);
            SeedData(dbContext);
            return new ContaRepository(dbContext);
        }

        private static void SeedData(BankContext dbContext)
        {
            dbContext.Contas.Add(new Conta { Numero = "123", Saldo = 1000, ClienteId = Guid.NewGuid() });
            dbContext.Contas.Add(new Conta { Numero = "456", Saldo = 2000, ClienteId = Guid.NewGuid() });

            dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetByNumeroAsync_WhenItemExists_ReturnSuccess()
        {
            var repository = GetBankContextInMemory();

            var contas = await repository.GetByNumeroAsync("123", true);

            Assert.NotNull(contas);
        }

        
        [Fact]
        public async Task AddAsync_DeveAdicionarConta_QuandoContaExiste()
        {
            // Arrange

            var repo = GetBankContextInMemory();

            var conta = new Conta("789", 100m);

            // Act
            await repo.AddAsync(conta);

            // Assert
            var persisted = await repo.GetByNumeroAsync("789");
            Assert.NotNull(persisted);
            Assert.Equal(100m, persisted.Balance);
        }



        [Fact]
        public async Task GetByNumeroAsync_DeveRetornarContaComCliente_QuandoExistir()
        {
            // Arrange

            var repo = GetBankContextInMemory();

            var cliente = new Cliente { Id = Guid.NewGuid(), Nome = "Cliente A", Email = "a@example.com" };
            var conta = new Conta("999", 250m, cliente.Id) { Cliente = cliente };
            

            await repo.AddAsync(conta);            

            // Act
            var result = await repo.GetByNumeroAsync("999");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("999", result!.Numero);
            Assert.Equal(250m, result.Saldo);
            Assert.NotNull(result.Cliente);
            Assert.Equal("Cliente A", result.Cliente.Nome);
        }

        [Theory]
        [InlineData("E1", true)]
        [InlineData("Nope", false)]
        public async Task ExistsByNumeroAsync_DeveRetornarVerdadeiro_QuandoExisti_E_Falso_CasoContrario(string nomeConta, bool result)
        {
            // Arrange
            var repo = GetBankContextInMemory();
                        
            var conta = new Conta("E1", 1m);
            await repo.AddAsync(conta);

            // Act
            var exists = await repo.ExistsByNumeroAsync(nomeConta);

            // Assert            
            Assert.Equal(exists, result );
            

        }

        //[Fact]
        //public async Task ClienteExistsAsync_DeveRetornarVerdadeiro_QuandoClienteExistir()
        //{
        //    // Arrange
        //    var dbName = Guid.NewGuid().ToString();
        //    await using var context = CreateContext(dbName);
        //    var repo = new ContaRepository(context);

        //    var clienteId = Guid.NewGuid();
        //    await context.Clients.AddAsync(new Cliente { Id = clienteId, Nome = "X", Email = "x@x.com" });
        //    await context.SaveChangesAsync();

        //    // Act
        //    var exists = await repo.ClienteExistsAsync(clienteId);

        //    // Assert
        //    Assert.True(exists);
        //}

        //[Fact]
        //public async Task GetAllAsync_DeveRetornarResultadoPaginadoEContTotal_QuandoContaExiste()
        //{
        //    // Arrange
        //    var dbName = Guid.NewGuid().ToString();
        //    await using var context = CreateContext(dbName);

        //    var contas = Enumerable.Range(1, 5)
        //                           .Select(i => new Conta(i.ToString(), i * 10m))
        //                           .ToList();

        //    await context.Contas.AddRangeAsync(contas);
        //    await context.SaveChangesAsync();

        //    var repo = new ContaRepository(context);

        //    // Act            
        //    var (items, total) = await repo.GetAllAsync(page: 2, size: 2, orderBy: "numero", sort: "asc");

        //    // Assert
        //    Assert.Equal(5, total);
        //    Assert.Equal(2, items.Count());
        //    Assert.Contains(items, c => c.Numero == "3");
        //    Assert.Contains(items, c => c.Numero == "4");
        //}

        //[Fact]
        //public async Task UpdateAsync_DevePersistirAtualizações_QuandoAtualizaConta()
        //{
        //    // Arrange
        //    var dbName = Guid.NewGuid().ToString();
        //    await using var context = CreateContext(dbName);

        //    var conta = new Conta("UPD", 50m);
        //    await context.Contas.AddAsync(conta);
        //    await context.SaveChangesAsync();

        //    var repo = new ContaRepository(context);

        //    // Act
        //    conta.Saldo = 150m;
        //    await repo.UpdateAsync(conta);

        //    // Assert
        //    var updated = await context.Contas.AsNoTracking().FirstOrDefaultAsync(c => c.Numero == "UPD");
        //    Assert.NotNull(updated);
        //    Assert.Equal(150m, updated.Balance);
        //}

        //[Fact]
        //public async Task DeleteAsync_DeveRemoverConta_QuandoContaExiste()
        //{
        //    // Arrange
        //    var dbName = Guid.NewGuid().ToString();
        //    await using var context = CreateContext(dbName);

        //    var conta = new Conta("DEL", 20m);
        //    await context.Contas.AddAsync(conta);
        //    await context.SaveChangesAsync();

        //    // Act
        //    var repo = new ContaRepository(context);

        //    await repo.DeleteAsync(conta);

        //    // Assert
        //    var exists = await context.Contas.AnyAsync(c => c.Numero == "DEL");
        //    Assert.False(exists);
        //}


    }
}

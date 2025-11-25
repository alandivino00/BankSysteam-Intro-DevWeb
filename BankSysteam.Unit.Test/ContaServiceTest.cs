using BankSysteam.Api.models;
using BankSysteam.Api.Repositories;
using BankSysteam.Api.Service;
using Moq;
using static System.Net.Mime.MediaTypeNames;

namespace BankSysteam.Unit.Test
{
    public class ContaServiceTest
    {

        private readonly Mock<IContaRepository> _contaRepositoryMock;
        private readonly ContaService _contaService;

        public ContaServiceTest()
        {
            _contaRepositoryMock = new Mock<IContaRepository>();
            _contaService = new ContaService(_contaRepositoryMock.Object);
        }


        [Fact]
        public void GetAllContasAsync_DeveRetornarLista_QuandoContasExistem()
        {
            // Arrange

            var contas = new List<Conta>
            {
                new Conta { Numero = "123", Saldo = 1000, ClienteId = Guid.NewGuid() },
                new Conta { Numero = "456", Saldo = 2000, ClienteId = Guid.NewGuid() }
            };

            _contaRepositoryMock
               .Setup(repo => repo.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
               .ReturnsAsync((Items: (IEnumerable<Conta>)contas, TotalCount: contas.Count));

            // Act

            var resultTuple = _contaService.GetContasAsync().Result;

            // ASSERT
            Assert.NotNull(resultTuple.Items);
            Assert.Equal(2, resultTuple.Items.Count());
            Assert.Contains(resultTuple.Items, c => c.Numero == "123");

        }

        [Fact]
        public async Task GetAllContasAsync_DeveRetornarListaVazia_QuandoNenhumaContaExiste()
        {
            // Arrange
            var contas = new List<Conta>();
            _contaRepositoryMock
               .Setup(repo => repo.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
               .ReturnsAsync((Items: (IEnumerable<Conta>)contas, TotalCount: contas.Count));

            // Act
            var result = await _contaService.GetContasAsync();

            // ASSERT
            Assert.NotNull(result.Items);
            Assert.Empty(result.Items);
        }

        [Fact]
        public async Task GetAllContaAsync_DeveRetornarUmaExcecao_QuandoObjetoForNulo()
        {
            // Arrange            
            _contaRepositoryMock
               .Setup(repo => repo.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
               .ThrowsAsync(new ArgumentNullException("repository"));

            // Act & ASSERT
            await Assert.ThrowsAsync<ArgumentNullException>(() => _contaService.GetContasAsync());
        }

        [Fact]
        public async Task GetContaAsync_DeveRetornarConta_QuandoContaExiste()
        {
            // Arrange
            var numero = "123";
            var conta = new Conta
            {
                Numero = numero,
                Saldo = 1000m,
                ClienteId = Guid.NewGuid()
            };

            _contaRepositoryMock
               .Setup(repo => repo.GetByNumeroAsync(It.Is<string>(s => s == numero), It.IsAny<bool>()))
               .ReturnsAsync(conta);

            // Act
            var result = await _contaService.GetContaAsync(numero);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(numero, result!.Numero);
            Assert.Equal(1000m, result.Saldo);
        }

        [Fact]
        public async Task GetContaAsync_DeveRetornarNull_QuandoContaNaoExiste()
        {
            // Arrange
            var numeroInexistente = "999";
            _contaRepositoryMock
                .Setup(repo => repo.GetByNumeroAsync(It.Is<string>(s => s == numeroInexistente), It.IsAny<bool>()))
                .ReturnsAsync((Conta?)null);

            // Act
            var result = await _contaService.GetContaAsync(numeroInexistente);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateContaAsync_DeveRetornarContaCriada_QuandoDadosSaoValidos()
        {
            // Arrange
            var input = new BankSysteam.Api.Models.ContaInputModel
            {
                Numero = "123",
                Saldo = 500m,
                ClienteId = Guid.NewGuid()
            };
            _contaRepositoryMock
                .Setup(repo => repo.ClienteExistsAsync(It.Is<Guid>(id => id == input.ClienteId)))
                .ReturnsAsync(true);
            _contaRepositoryMock
                .Setup(repo => repo.ExistsByNumeroAsync(It.Is<string>(num => num == input.Numero)))
                .ReturnsAsync(false);
            // Act
            var result = await _contaService.CreateContaAsync(input);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(input.Numero, result.Numero);
            Assert.Equal(input.Saldo, result.Saldo);
        }

        [Fact]
        public async Task CreateContaAsync_DeveLancarExcecao_QuandoClienteNaoExiste()
        {
            // Arrange
            var input = new BankSysteam.Api.Models.ContaInputModel
            {
                Numero = "123",
                Saldo = 500m,
                ClienteId = Guid.NewGuid()
            };
            _contaRepositoryMock
                .Setup(repo => repo.ClienteExistsAsync(It.Is<Guid>(id => id == input.ClienteId)))
                .ReturnsAsync(false);
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _contaService.CreateContaAsync(input));
        }

        [Fact]
        public async Task DepositoAsync_DeveLancarExcecao_QuandoValorForInvalido()
        {
            // Arrange
            var numero = "123";
            var valorInvalido = -100m;
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _contaService.DepositoAsync(numero, valorInvalido));
        }

        [Fact]
        public async Task DepositoAsync_DeveAtualizarSaldo_QuandoValorForValido()
        {
            // Arrange
            var numero = "123";
            var valorDeposito = 200m;
            var contaExistente = new Conta
            {
                Numero = numero,
                Saldo = 500m,
                ClienteId = Guid.NewGuid()
            };
            _contaRepositoryMock
                .Setup(repo => repo.GetByNumeroAsync(It.Is<string>(s => s == numero), It.IsAny<bool>()))
                .ReturnsAsync(contaExistente);
            // Act
            await _contaService.DepositoAsync(numero, valorDeposito);
            // Assert
            _contaRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<Conta>(c => c.Numero == numero && c.Saldo == 700m)), Times.Once);
        }

        [Fact]
        public async Task DeleteContaAsync_DeveLancarExcecao_QuandoContaNaoExiste()
        {
            // Arrange
            var numeroInexistente = "999";
            _contaRepositoryMock
                .Setup(repo => repo.GetByNumeroAsync(It.Is<string>(s => s == numeroInexistente), It.IsAny<bool>()))
                .ReturnsAsync((Conta?)null);
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _contaService.DeleteContaAsync(numeroInexistente));
        }
        [Fact]
        public async Task DeleteContaAsync_DeveExcluirConta_QuandoContaExiste()
        {
            // Arrange
            var numero = "123";
            var contaExistente = new Conta
            {
                Numero = numero,
                Saldo = 500m,
                ClienteId = Guid.NewGuid()
            };
            _contaRepositoryMock
                .Setup(repo => repo.GetByNumeroAsync(It.Is<string>(s => s == numero), It.IsAny<bool>()))
                .ReturnsAsync(contaExistente);
            // Act
            await _contaService.DeleteContaAsync(numero);
            // Assert
            _contaRepositoryMock.Verify(repo => repo.DeleteAsync(It.Is<Conta>(c => c.Numero == numero)), Times.Once);
        }

    }
}
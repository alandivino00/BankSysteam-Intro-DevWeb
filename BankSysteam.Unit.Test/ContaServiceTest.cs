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
        /*
        [Fact]
        public async Task GetAllContaAsync_DeveRetornarUmaExcecao_QuandoObjetoForNulo()
        {
            //a corrigir...
            // Arrange
            _contaRepositoryMock
                .Setup(repo => repo.GetAllContasAsync())
                .ThrowsAsync((List<Conta>) null);
            // Act & ASSERT
        }
        */
    }
}
using Xunit;
using Moq;
using BLL.Implementacion;
using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;

namespace Tecmein.Tests
{
    public class ClienteServiceTests
    {
        private readonly Mock<IGenericRepository<Cliente>> _mockClienteRepo;
        private readonly Mock<IGenericRepository<FormatoNumeroCliente>> _mockFormatoRepo;
        private readonly Mock<IConstructoraServices> _mockConstructoraServices;
        private readonly ClienteServices _clienteService;

        public ClienteServiceTests()
        {
            _mockClienteRepo = new Mock<IGenericRepository<Cliente>>();
            _mockFormatoRepo = new Mock<IGenericRepository<FormatoNumeroCliente>>();
            _mockConstructoraServices = new Mock<IConstructoraServices>();
            _clienteService = new ClienteServices(
                _mockClienteRepo.Object, 
                _mockFormatoRepo.Object, 
                _mockConstructoraServices.Object);
        }

        [Fact]
        public async Task Crear_ConConstructoraValida_DebeCrearCliente()
        {
            // Arrange
            var constructora = new Constructora { Secuencial = 1, Nombre = "Constructora Test" };
            var nuevoCliente = new Cliente { SecConstructora = 1 };
            var formato = new FormatoNumeroCliente { UsaFormato = true, Formato = "CLI-{YYYY}-", LongitudNumero = 3, NumeroInicio = 1 };
            var clientesExistentes = new List<Cliente>().AsQueryable();

            _mockClienteRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<Cliente, bool>>>(), null)).ReturnsAsync((Cliente)null);
            _mockConstructoraServices.Setup(s => s.ConstructoraPorSecuencial(1)).ReturnsAsync(constructora);
            _mockFormatoRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<FormatoNumeroCliente, bool>>>(), null)).ReturnsAsync(formato);
            _mockClienteRepo.Setup(repo => repo.Consultar(It.IsAny<Expression<Func<Cliente, bool>>>(), It.IsAny<string[]>())).ReturnsAsync(clientesExistentes);
            _mockClienteRepo.Setup(repo => repo.Crear(It.IsAny<Cliente>())).ReturnsAsync((Cliente c) => c);

            // Act
            var resultado = await _clienteService.Crear(nuevoCliente);

            // Assert
            Assert.NotNull(resultado);
            Assert.Contains("CLI-", resultado.NumeroCliente);
            Assert.True(resultado.EstaActivo);
            _mockClienteRepo.Verify(repo => repo.Crear(It.IsAny<Cliente>()), Times.Once);
        }

        [Fact]
        public async Task Crear_CuandoConstructoraYaEsCliente_DebeLanzarExcepcion()
        {
            // Arrange
            var clienteExistente = new Cliente { SecConstructora = 1 };
            var nuevoCliente = new Cliente { SecConstructora = 1 };

            _mockClienteRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<Cliente, bool>>>(), null)).ReturnsAsync(clienteExistente);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _clienteService.Crear(nuevoCliente));
        }

        [Fact]
        public async Task Crear_ConConstructoraInexistente_DebeLanzarExcepcion()
        {
            // Arrange
            var nuevoCliente = new Cliente { SecConstructora = 99 };

            _mockClienteRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<Cliente, bool>>>(), null)).ReturnsAsync((Cliente)null);
            _mockConstructoraServices.Setup(s => s.ConstructoraPorSecuencial(99)).ReturnsAsync((Constructora)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _clienteService.Crear(nuevoCliente));
        }
    }
}
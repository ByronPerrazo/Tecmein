using Xunit;
using Moq;
using BLL.Implementacion;
using DAL.Interfaces;
using Entity;
using System; 
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Tecmein.Tests
{
    public class FormatoNumeroClienteServiceTests
    {
        private readonly Mock<IGenericRepository<FormatoNumeroCliente>> _mockRepo;
        private readonly FormatoNumeroClienteServices _service;

        public FormatoNumeroClienteServiceTests()
        {
            _mockRepo = new Mock<IGenericRepository<FormatoNumeroCliente>>();
            _service = new FormatoNumeroClienteServices(_mockRepo.Object);
        }

        [Fact]
        public async Task Guardar_NuevoFormato_DebeCrearCorrectamente()
        {
            // Arrange
            var nuevoFormato = new FormatoNumeroCliente { SecEmpresa = 1, UsaFormato = true, Formato = "CLI-", NumeroInicio = 100 };
            _mockRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<FormatoNumeroCliente, bool>>>(), null)).ReturnsAsync((FormatoNumeroCliente)null);
            _mockRepo.Setup(repo => repo.Crear(It.IsAny<FormatoNumeroCliente>())).ReturnsAsync((FormatoNumeroCliente f) => f);

            // Act
            var resultado = await _service.Guardar(nuevoFormato);

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.UsaFormato);
            Assert.Equal("CLI-", resultado.Formato);
            _mockRepo.Verify(repo => repo.Crear(It.IsAny<FormatoNumeroCliente>()), Times.Once);
            _mockRepo.Verify(repo => repo.Editar(It.IsAny<FormatoNumeroCliente>()), Times.Never);
        }

        [Fact]
        public async Task Guardar_FormatoExistente_DebeEditarCorrectamente()
        {
            // Arrange
            var formatoExistente = new FormatoNumeroCliente { SecFormatoNumeroCliente = 1, SecEmpresa = 1, UsaFormato = false, Formato = "OLD-", NumeroInicio = 50 };
            var formatoEditado = new FormatoNumeroCliente { SecEmpresa = 1, UsaFormato = true, Formato = "NEW-", NumeroInicio = 200 };

            _mockRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<FormatoNumeroCliente, bool>>>(), null)).ReturnsAsync(formatoExistente);
            _mockRepo.Setup(repo => repo.Editar(It.IsAny<FormatoNumeroCliente>())).ReturnsAsync(true);

            // Act
            var resultado = await _service.Guardar(formatoEditado);

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.UsaFormato);
            Assert.Equal("NEW-", resultado.Formato);
            Assert.Equal(200, resultado.NumeroInicio);
            _mockRepo.Verify(repo => repo.Editar(It.IsAny<FormatoNumeroCliente>()), Times.Once);
            _mockRepo.Verify(repo => repo.Crear(It.IsAny<FormatoNumeroCliente>()), Times.Never);
        }
    }
}

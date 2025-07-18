
using Xunit;
using Moq;
using BLL.Implementacion;
using DAL.Interfaces;
using Entity;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Tecmein.Tests
{
    public class CantonServiceTests
    {
        private readonly Mock<IGenericRepository<Canton>> _mockRepo;
        private readonly CantonServices _service;

        public CantonServiceTests()
        {
            _mockRepo = new Mock<IGenericRepository<Canton>>();
            _service = new CantonServices(_mockRepo.Object);
        }

        private IQueryable<Canton> GetTestCantons()
        {
            var provincias = new List<Provincia>
            {
                new Provincia { Secuencial = 1, Nombre = "Provincia A" },
                new Provincia { Secuencial = 2, Nombre = "Provincia B" }
            };

            return new List<Canton>
            {
                new Canton { Secuencial = 1, Nombre = "Canton 1A", SecProvincia = 1, SecProvinciaNavigation = provincias[0] },
                new Canton { Secuencial = 2, Nombre = "Canton 2A", SecProvincia = 1, SecProvinciaNavigation = provincias[0] },
                new Canton { Secuencial = 3, Nombre = "Canton 1B", SecProvincia = 2, SecProvinciaNavigation = provincias[1] }
            }.AsQueryable();
        }

        [Fact]
        public async Task Lista_DebeDevolverTodosLosCantonesConProvincia()
        {
            // Arrange
            var testCantons = GetTestCantons();
            _mockRepo.Setup(repo => repo.Consultar(It.IsAny<Expression<Func<Canton, bool>>>())).ReturnsAsync(testCantons);
            _mockRepo.Setup(repo => repo.Consultar(null)).ReturnsAsync(testCantons);

            // Act
            var resultado = await _service.Lista();

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(3, resultado.Count);
            Assert.True(resultado.All(c => c.SecProvinciaNavigation != null));
            Assert.Contains(resultado, c => c.Nombre == "Canton 1A" && c.SecProvinciaNavigation.Nombre == "Provincia A");
        }

        [Fact]
        public async Task ListaPorProvincia_DebeDevolverCantonesFiltradosPorProvincia()
        {
            // Arrange
            var testCantons = GetTestCantons();
            _mockRepo.Setup(repo => repo.Consultar(It.IsAny<Expression<Func<Canton, bool>>>())).ReturnsAsync((Expression<Func<Canton, bool>> filtro) => testCantons.Where(filtro).AsQueryable());

            // Act
            var resultado = await _service.ListaPorProvincia(1);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
            Assert.True(resultado.All(c => c.SecProvincia == 1));
            Assert.True(resultado.All(c => c.SecProvinciaNavigation != null));
        }

        [Fact]
        public async Task ListaPorProvincia_ProvinciaNoExistente_DebeDevolverListaVacia()
        {
            // Arrange
            var testCantons = GetTestCantons();
            _mockRepo.Setup(repo => repo.Consultar(It.IsAny<Expression<Func<Canton, bool>>>())).ReturnsAsync((Expression<Func<Canton, bool>> filtro) => testCantons.Where(filtro).AsQueryable());

            // Act
            var resultado = await _service.ListaPorProvincia(99);

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado);
        }

        [Fact]
        public async Task CantonPorSecuencial_DebeDevolverCantonCorrecto()
        {
            // Arrange
            var testCantons = GetTestCantons();
            _mockRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<Canton, bool>>>(), It.IsAny<string>())).ReturnsAsync((Expression<Func<Canton, bool>> filtro, string? incluirPropiedades) => testCantons.FirstOrDefault(filtro));

            // Act
            var resultado = await _service.CantonPorSecuencial(1);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Canton 1A", resultado.Nombre);
        }

        [Fact]
        public async Task CantonPorSecuencial_CantonNoExistente_DebeDevolverNull()
        {
            // Arrange
            var testCantons = GetTestCantons();
            _mockRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<Canton, bool>>>(), It.IsAny<string>())).ReturnsAsync((Expression<Func<Canton, bool>> filtro, string? incluirPropiedades) => testCantons.FirstOrDefault(filtro));

            // Act
            var resultado = await _service.CantonPorSecuencial(99);

            // Assert
            Assert.Null(resultado);
        }
    }
}

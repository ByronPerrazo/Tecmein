using Xunit;
using Moq;
using BLL.Implementacion;
using DAL.Interfaces;
using Entity;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;

namespace Tecmein.Tests
{
    public class DashBoardServiceTests
    {
        private readonly Mock<IGenericRepository<Visita>> _mockVisitaRepo;
        private readonly Mock<IGenericRepository<Equiposvisita>> _mockEquiposRepo;
        private readonly DashBoardServices _service;

        public DashBoardServiceTests()
        {
            _mockVisitaRepo = new Mock<IGenericRepository<Visita>>();
            _mockEquiposRepo = new Mock<IGenericRepository<Equiposvisita>>();
           // _service = new DashBoardServices(_mockVisitaRepo.Object, _mockEquiposRepo.Object);
        }

        private IQueryable<Visita> GetTestVisitas() => new List<Visita>
        {
            new Visita { Secuencial = 1, FechaRegistro = DateTime.Now.Date.AddDays(-5) },
            new Visita { Secuencial = 2, FechaRegistro = DateTime.Now.Date.AddDays(-5) },
            new Visita { Secuencial = 3, FechaRegistro = DateTime.Now.Date.AddDays(-10) },
            new Visita { Secuencial = 4, FechaRegistro = DateTime.Now.Date.AddDays(-400) } // Fuera del rango de 360 días
        }.AsQueryable();

        private IQueryable<Equiposvisita> GetTestEquipos() => new List<Equiposvisita>
        {
            new Equiposvisita { Secuencial = 1, Marca = "MarcaA", Cantidad = 5, EstaActivo = 1 },
            new Equiposvisita { Secuencial = 2, Marca = "MarcaB", Cantidad = 10, EstaActivo = 1 },
            new Equiposvisita { Secuencial = 3, Marca = "MarcaA", Cantidad = 3, EstaActivo = 1 },
            new Equiposvisita { Secuencial = 4, Marca = "MarcaC", Cantidad = 2, EstaActivo = 0 },
            new Equiposvisita { Secuencial = 5, Marca = "MarcaD", Cantidad = null, EstaActivo = 1 } // Cantidad nula
        }.AsQueryable();

        [Fact]
        public async Task TotalVisitasUltimaSemana_DebeDevolverTotalCorrecto()
        {
            // Arrange
            var testData = new TestAsyncEnumerable<Visita>(GetTestVisitas().Where(v => v.FechaRegistro >= DateTime.Now.Date.AddDays(-360)));
            _mockVisitaRepo.Setup(repo => repo.Consultar(It.IsAny<Expression<Func<Visita, bool>>>()))
                           .ReturnsAsync(testData);

            // Act
            var resultado = await _service.TotalVisitasUltimaSemana();

            // Assert
            Assert.Equal(3, resultado);
        }

        [Fact]
        public async Task TotalEquipos_DebeDevolverSumaDeCantidadesActivas()
        {
            // Arrange
            var testData = new TestAsyncEnumerable<Equiposvisita>(GetTestEquipos().Where(e => e.EstaActivo == 1));
            _mockEquiposRepo.Setup(repo => repo.Consultar(It.IsAny<Expression<Func<Equiposvisita, bool>>>()))
                            .ReturnsAsync(testData);

            // Act
            var resultado = await _service.TotalEquipos();

            // Assert
            Assert.Equal(18, resultado); // 5 + 10 + 3 = 18 (ignora inactivos y nulos)
        }

        [Fact]
        public async Task TotalMarcas_DebeContarMarcasDistintasActivas()
        {
            // Arrange
            var testData = new TestAsyncEnumerable<Equiposvisita>(GetTestEquipos().Where(e => e.EstaActivo == 1));
            _mockEquiposRepo.Setup(repo => repo.Consultar(It.IsAny<Expression<Func<Equiposvisita, bool>>>()))
                            .ReturnsAsync(testData);

            // Act
            var resultado = await _service.TotalMarcas();

            // Assert
            Assert.Equal(3, resultado); // MarcaA, MarcaB, MarcaD
        }

        [Fact]
        public async Task MarcasMasVendidas_DebeDevolverDiccionarioOrdenado()
        {
            // Arrange
            var testData = new TestAsyncEnumerable<Equiposvisita>(GetTestEquipos());
            _mockEquiposRepo.Setup(repo => repo.Consultar(null)).ReturnsAsync(testData);

            // Act
            var resultado = await _service.MarcasMasVendidas();

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(4, resultado.Count);
            Assert.Equal("MarcaB", resultado.Keys.First());
            Assert.Equal(10, resultado["MarcaB"]);
            Assert.Equal(8, resultado["MarcaA"]);
            Assert.Equal(2, resultado["MarcaC"]);
            Assert.Equal(0, resultado["MarcaD"]);
        }

        [Fact]
        public async Task VisitasUltimaSemana_DebeAgruparPorFechaCorrectamente()
        {
            // Arrange
            var testData = new TestAsyncEnumerable<Visita>(GetTestVisitas().Where(v => v.FechaRegistro >= DateTime.Now.Date.AddDays(-360)));
            _mockVisitaRepo.Setup(repo => repo.Consultar(It.IsAny<Expression<Func<Visita, bool>>>()))
                           .ReturnsAsync(testData);

            // Act
            var resultado = await _service.VisitasUltimaSemana();

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
            Assert.Equal(2, resultado[DateTime.Now.Date.AddDays(-5).ToString("dd/MM/yyyy")]);
            Assert.Equal(1, resultado[DateTime.Now.Date.AddDays(-10).ToString("dd/MM/yyyy")]);
        }

        // --- Pruebas de Casos Límite ---

        [Fact]
        public async Task TotalVisitasUltimaSemana_SinVisitas_DebeDevolverCero()
        {
            // Arrange
            var testData = new TestAsyncEnumerable<Visita>(new List<Visita>());
            _mockVisitaRepo.Setup(repo => repo.Consultar(It.IsAny<Expression<Func<Visita, bool>>>()))
                           .ReturnsAsync(testData);

            // Act
            var resultado = await _service.TotalVisitasUltimaSemana();

            // Assert
            Assert.Equal(0, resultado);
        }

        [Fact]
        public async Task TotalEquipos_SinEquipos_DebeDevolverCero()
        {
            // Arrange
            var testData = new TestAsyncEnumerable<Equiposvisita>(new List<Equiposvisita>());
            _mockEquiposRepo.Setup(repo => repo.Consultar(It.IsAny<Expression<Func<Equiposvisita, bool>>>()))
                            .ReturnsAsync(testData);

            // Act
            var resultado = await _service.TotalEquipos();

            // Assert
            Assert.Equal(0, resultado);
        }

        [Fact]
        public async Task MarcasMasVendidas_SinEquipos_DebeDevolverDiccionarioVacio()
        {
            // Arrange
            var testData = new TestAsyncEnumerable<Equiposvisita>(new List<Equiposvisita>());
            _mockEquiposRepo.Setup(repo => repo.Consultar(null)).ReturnsAsync(testData);

            // Act
            var resultado = await _service.MarcasMasVendidas();

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado);
        }
    }
}
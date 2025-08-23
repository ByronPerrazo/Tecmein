using Xunit;
using Moq;
using BLL.Implementacion;
using DAL.Interfaces;
using Entity;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using BLL.Interfaces;
using System;

namespace Tecmein.Tests
{
    public class EquiposVisitaServiceTests
    {
        private readonly Mock<IGenericRepository<Equiposvisita>> _mockEquiposVisitaRepo;
        private readonly Mock<IVisitaServices> _mockVisitaServices;
        private readonly Mock<IValidacionServices> _mockValidacionServices;
        private readonly Mock<IGenericRepository<Cotizacion>> _mockCotizacionRepo; // New
        private readonly Mock<IGenericRepository<Cotizaciondetalle>> _mockCotizacionDetalleRepo; // New
        private readonly EquiposVisitaServices _service;

        public EquiposVisitaServiceTests()
        {
            _mockEquiposVisitaRepo = new Mock<IGenericRepository<Equiposvisita>>();
            _mockVisitaServices = new Mock<IVisitaServices>();
            _mockValidacionServices = new Mock<IValidacionServices>();
            _mockCotizacionRepo = new Mock<IGenericRepository<Cotizacion>>(); // New
            _mockCotizacionDetalleRepo = new Mock<IGenericRepository<Cotizaciondetalle>>(); // New
            _service = new EquiposVisitaServices(
                _mockEquiposVisitaRepo.Object,
                _mockVisitaServices.Object,
                _mockValidacionServices.Object,
                _mockCotizacionRepo.Object,
                _mockCotizacionDetalleRepo.Object
            );
        }

        private List<Equiposvisita> GetTestEquiposVisita() => new List<Equiposvisita>
        {
            new Equiposvisita { Secuencial = 1, SecVisita = 10, Marca = "MarcaA", Cantidad = 5, EstaActivo = 1 },
            new Equiposvisita { Secuencial = 2, SecVisita = 10, Marca = "MarcaB", Cantidad = 10, EstaActivo = 1 },
            new Equiposvisita { Secuencial = 3, SecVisita = 20, Marca = "MarcaC", Cantidad = 3, EstaActivo = 1 },
            new Equiposvisita { Secuencial = 4, SecVisita = 10, Marca = "MarcaD", Cantidad = 2, EstaActivo = 0 }
        };

        private List<Visita> GetTestVisitas() => new List<Visita>
        {
            new Visita { Secuencial = 10 },
            new Visita { Secuencial = 20 }
        };

        [Fact]
        public async Task ConsultaListaPorVisita_DebeDevolverEquiposActivosParaVisitaEspecifica()
        {
            // Arrange
            var testData = new TestAsyncEnumerable<Equiposvisita>(GetTestEquiposVisita().Where(e => e.SecVisita == 10 && e.EstaActivo == 1));
            _mockEquiposVisitaRepo.Setup(repo => repo.Consultar(It.IsAny<Expression<Func<Equiposvisita, bool>>>()))
                                  .ReturnsAsync(testData);

            // Act
            var resultado = await _service.ConsultaListaPorVisita(10);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Equipos.Count());
            Assert.True(resultado.Equipos.All(e => e.SecVisita == 10 && e.EstaActivo == 1));
        }

        [Fact]
        public async Task Obtener_DebeDevolverEquipoVisitaPorSecuencial()
        {
            // Arrange
            var equipoEsperado = GetTestEquiposVisita().First();
            _mockEquiposVisitaRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<Equiposvisita, bool>>>(), It.IsAny<string>()))
                                  .ReturnsAsync(equipoEsperado);

            // Act
            var resultado = await _service.Obtener(1);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Secuencial);
        }

        [Fact]
        public async Task Obtener_EquipoVisitaNoExistente_DebeDevolverNull()
        {
            // Arrange
            _mockEquiposVisitaRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<Equiposvisita, bool>>>(), It.IsAny<string>()))
                                  .ReturnsAsync((Equiposvisita)null);

            // Act
            var resultado = await _service.Obtener(99);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public async Task ProcesaGuardar_DebeGuardarEquipoVisitaCorrectamente()
        {
            // Arrange
            var nuevoEquipo = new Equiposvisita { Secuencial = 5, SecVisita = 10, Marca = "MarcaE", Cantidad = 1, EstaActivo = 1 };
            _mockEquiposVisitaRepo.Setup(repo => repo.Crear(It.IsAny<Equiposvisita>())).ReturnsAsync(nuevoEquipo);
            _mockVisitaServices.Setup(s => s.ConsultaVisita(10)).ReturnsAsync(GetTestVisitas().First(v => v.Secuencial == 10));

            // Act
            var resultado = await _service.ProcesaGuardar(nuevoEquipo);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(5, resultado.Secuencial);
            _mockEquiposVisitaRepo.Verify(repo => repo.Crear(It.IsAny<Equiposvisita>()), Times.Once);
            _mockValidacionServices.Verify(v => v.ValidarNombre(nuevoEquipo.Marca, "marca del equipo"), Times.Once);
        }

        [Fact]
        public async Task ProcesaGuardar_VisitaNoExiste_DebeLanzarExcepcion()
        {
            // Arrange
            var nuevoEquipo = new Equiposvisita { SecVisita = 99, Marca = "MarcaX", Cantidad = 1 };
            _mockVisitaServices.Setup(s => s.ConsultaVisita(99)).ReturnsAsync((Visita)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<TaskCanceledException>(() => _service.ProcesaGuardar(nuevoEquipo));
            Assert.Equal("La visita con secuencial 99 no existe.", exception.Message);
        }

        [Fact]
        public async Task ProcesaGuardar_CantidadCeroOInferior_DebeLanzarExcepcion()
        {
            // Arrange
            var nuevoEquipo = new Equiposvisita { SecVisita = 10, Marca = "MarcaX", Cantidad = 0 };
            _mockVisitaServices.Setup(s => s.ConsultaVisita(10)).ReturnsAsync(GetTestVisitas().First());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<TaskCanceledException>(() => _service.ProcesaGuardar(nuevoEquipo));
            Assert.Equal("La cantidad del equipo debe ser mayor que cero.", exception.Message);
        }

        [Fact]
        public async Task ProcesaEliminar_DebeEliminarEquipoVisitaCorrectamente()
        {
            // Arrange
            var equipoAEliminar = GetTestEquiposVisita().First();
            _mockEquiposVisitaRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<Equiposvisita, bool>>>(), It.IsAny<string>()))
                                  .ReturnsAsync(equipoAEliminar);
            _mockEquiposVisitaRepo.Setup(repo => repo.Eliminar(It.IsAny<Equiposvisita>())).ReturnsAsync(true);

            // Act
            var resultado = await _service.ProcesaEliminar(equipoAEliminar);

            // Assert
            Assert.True(resultado);
            _mockEquiposVisitaRepo.Verify(repo => repo.Eliminar(equipoAEliminar), Times.Once);
        }

        [Fact]
        public async Task ProcesaEliminar_EquipoVisitaNoExistente_DebeDevolverFalse()
        {
            // Arrange
            _mockEquiposVisitaRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<Equiposvisita, bool>>>(), It.IsAny<string>()))
                                  .ReturnsAsync((Equiposvisita)null);

            // Act
            var resultado = await _service.ProcesaEliminar(new Equiposvisita { Secuencial = 99 });

            // Assert
            Assert.False(resultado);
        }
    }
}
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
    public class PreContratoServiceTests
    {
        private readonly Mock<IGenericRepository<PreContrato>> _mockPreContratoRepo;
        private readonly Mock<ICotizacionServices> _mockCotizacionServices;
        private readonly Mock<IGenericRepository<PlantillaPreContratoParrafo>> _mockPlantillaParrafoRepo;
        private readonly PreContratoServices _preContratoService;

        public PreContratoServiceTests()
        {
            _mockPreContratoRepo = new Mock<IGenericRepository<PreContrato>>();
            _mockCotizacionServices = new Mock<ICotizacionServices>();
            _mockPlantillaParrafoRepo = new Mock<IGenericRepository<PlantillaPreContratoParrafo>>();
            _preContratoService = new PreContratoServices(_mockPreContratoRepo.Object, _mockCotizacionServices.Object, _mockPlantillaParrafoRepo.Object);
        }

        [Fact]
        public async Task Crear_ConCotizacionExistente_DebeCrearPreContrato()
        {
            // Arrange
            var cotizacion = new Cotizacion { Secuencial = 1 };
            var preContrato = new PreContrato { SecCotizacion = 1 };

            _mockCotizacionServices.Setup(s => s.Detalle(1)).ReturnsAsync(cotizacion);
            _mockPreContratoRepo.Setup(repo => repo.Crear(It.IsAny<PreContrato>())).ReturnsAsync(preContrato);

            // Act
            var resultado = await _preContratoService.Crear(preContrato);

            // Assert
            Assert.NotNull(resultado);
            _mockPreContratoRepo.Verify(repo => repo.Crear(It.IsAny<PreContrato>()), Times.Once);
        }

        [Fact]
        public async Task Crear_ConCotizacionInexistente_DebeLanzarExcepcion()
        {
            // Arrange
            var preContrato = new PreContrato { SecCotizacion = 99 };

            _mockCotizacionServices.Setup(s => s.Detalle(99)).ReturnsAsync((Cotizacion)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _preContratoService.Crear(preContrato));
        }

        [Fact]
        public async Task Editar_PreContratoExistente_DebeEditarCorrectamente()
        {
            // Arrange
            var preContratoExistente = new PreContrato { SecPreContrato = 1, Dias = 10, TipoDias = "Calendario" };
            var preContratoEditado = new PreContrato { SecPreContrato = 1, Dias = 15, TipoDias = "Termino" };

            _mockPreContratoRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<PreContrato, bool>>>(), null)).ReturnsAsync(preContratoExistente);
            _mockPreContratoRepo.Setup(repo => repo.Editar(It.IsAny<PreContrato>())).ReturnsAsync(true);

            // Act
            var resultado = await _preContratoService.Editar(preContratoEditado);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(15, resultado.Dias);
            Assert.Equal("Termino", resultado.TipoDias);
            _mockPreContratoRepo.Verify(repo => repo.Editar(It.IsAny<PreContrato>()), Times.Once);
        }

        [Fact]
        public async Task Eliminar_PreContratoExistente_DebeEliminarCorrectamente()
        {
            // Arrange
            var preContratoExistente = new PreContrato { SecPreContrato = 1 };

            _mockPreContratoRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<PreContrato, bool>>>(), null)).ReturnsAsync(preContratoExistente);
            _mockPreContratoRepo.Setup(repo => repo.Eliminar(It.IsAny<PreContrato>())).ReturnsAsync(true);

            // Act
            var resultado = await _preContratoService.Eliminar(1);

            // Assert
            Assert.True(resultado);
            _mockPreContratoRepo.Verify(repo => repo.Eliminar(It.IsAny<PreContrato>()), Times.Once);
        }

        [Fact]
        public async Task GenerarDocumentoWord_PreContratoExistente_DebeRetornarPlaceholder()
        {
            // Arrange
            var preContrato = new PreContrato { SecPreContrato = 1, SecCotizacion = 1, SecCotizacionNavigation = new Cotizacion { Secuencial = 1 } };
            var parrafos = new List<PlantillaPreContratoParrafo>
            {
                new PlantillaPreContratoParrafo { Orden = 1, Contenido = "Parrafo 1" },
                new PlantillaPreContratoParrafo { Orden = 2, Contenido = "Parrafo 2" }
            };

            _mockPreContratoRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<PreContrato, bool>>>(), null)).ReturnsAsync(preContrato);
            _mockPlantillaParrafoRepo.Setup(repo => repo.Consultar(It.IsAny<Expression<Func<PlantillaPreContratoParrafo, bool>>>())).ReturnsAsync(parrafos.AsQueryable());

            // Act
            var resultado = await _preContratoService.GenerarDocumentoWord(1);

            // Assert
            Assert.Equal("Documento Word generado (placeholder).", resultado);
        }
    }
}

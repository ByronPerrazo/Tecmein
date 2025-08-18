using Xunit;
using Moq;
using BLL.Implementacion;
using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using System;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Tecmein.Tests
{
    public class SeguimientoServiceTests
    {
        private readonly Mock<IGenericRepository<Seguimiento>> _mockSeguimientoRepo;
        private readonly Mock<ICotizacionServices> _mockCotizacionServices;
        private readonly Mock<IVisitaServices> _mockVisitaServices;
        private readonly Mock<IGenericRepository<Cotizacion>> _mockCotizacionRepo;
        private readonly SeguimientoServices _seguimientoService;

        public SeguimientoServiceTests()
        {
            _mockSeguimientoRepo = new Mock<IGenericRepository<Seguimiento>>();
            _mockCotizacionServices = new Mock<ICotizacionServices>();
            _mockVisitaServices = new Mock<IVisitaServices>();
            _mockCotizacionRepo = new Mock<IGenericRepository<Cotizacion>>();
            _seguimientoService = new SeguimientoServices(_mockSeguimientoRepo.Object, _mockCotizacionServices.Object, _mockVisitaServices.Object, _mockCotizacionRepo.Object);
        }

        /*
        // Tests commented out as the business logic for client creation has changed.
        // New tests will be required to validate the updated logic (changing Visita stage and Cotizacion.Confirmacion).

        [Fact]
        public async Task Crear_ConAceptacionClienteYClienteNoExistente_DebeCrearCliente()
        {
            // Arrange
                        var cotizacion = new Cotizacion { Secuencial = 1, SecVisita = 1 }; // Only need SecVisita for the mock
            var contactoVisita = new Contactovisita { SecVisita = 1, EstaActivo = 1, SecContactoNavigation = new Contacto { Secuencial = 1, SecConstructora = 1 } };

            _mockContactoVisitaRepo.Setup(repo => repo.Consultar(It.IsAny<Expression<Func<Contactovisita, bool>>>())).ReturnsAsync(new List<Contactovisita> { contactoVisita }.AsQueryable());
            var seguimiento = new Seguimiento { SecCotizacion = 1, AceptacionCliente = true };

            _mockCotizacionServices.Setup(s => s.Detalle(1)).ReturnsAsync(cotizacion);
            _mockClienteServices.Setup(s => s.ObtenerPorIdConstructora(1)).ReturnsAsync((Cliente)null);
            _mockSeguimientoRepo.Setup(repo => repo.Crear(It.IsAny<Seguimiento>())).ReturnsAsync(seguimiento);
            _mockClienteServices.Setup(s => s.Crear(It.IsAny<Cliente>())).ReturnsAsync(new Cliente());

            // Act
            await _seguimientoService.Crear(seguimiento);

            // Assert
            _mockClienteServices.Verify(s => s.Crear(It.Is<Cliente>(c => c.SecConstructora == 1)), Times.Once);
        }

        [Fact]
        public async Task Crear_ConAceptacionClienteYClienteExistente_NoDebeCrearCliente()
        {
            // Arrange
                        var cotizacion = new Cotizacion { Secuencial = 1, SecVisita = 1 }; // Only need SecVisita for the mock
            var contactoVisita = new Contactovisita { SecVisita = 1, EstaActivo = 1, SecContactoNavigation = new Contacto { Secuencial = 1, SecConstructora = 1 } };

            _mockContactoVisitaRepo.Setup(repo => repo.Consultar(It.IsAny<Expression<Func<Contactovisita, bool>>>())).ReturnsAsync(new List<Contactovisita> { contactoVisita }.AsQueryable());
            var seguimiento = new Seguimiento { SecCotizacion = 1, AceptacionCliente = true };
            var clienteExistente = new Cliente { SecConstructora = 1 };

            _mockCotizacionServices.Setup(s => s.Detalle(1)).ReturnsAsync(cotizacion);
            _mockClienteServices.Setup(s => s.ObtenerPorIdConstructora(1)).ReturnsAsync(clienteExistente);
            _mockSeguimientoRepo.Setup(repo => repo.Crear(It.IsAny<Seguimiento>())).ReturnsAsync(seguimiento);

            // Act
            await _seguimientoService.Crear(seguimiento);

            // Assert
            _mockClienteServices.Verify(s => s.Crear(It.IsAny<Cliente>()), Times.Never);
        }

        [Fact]
        public async Task Crear_SinAceptacionCliente_NoDebeCrearCliente()
        {
            // Arrange
                        var cotizacion = new Cotizacion { Secuencial = 1, SecVisita = 1 }; // Only need SecVisita for the mock
            var contactoVisita = new Contactovisita { SecVisita = 1, EstaActivo = 1, SecContactoNavigation = new Contacto { Secuencial = 1, SecConstructora = 1 } };

            _mockContactoVisitaRepo.Setup(repo => repo.Consultar(It.IsAny<Expression<Func<Contactovisita, bool>>>())).ReturnsAsync(new List<Contactovisita> { contactoVisita }.AsQueryable());
            var seguimiento = new Seguimiento { SecCotizacion = 1, AceptacionCliente = false };

            _mockCotizacionServices.Setup(s => s.Detalle(1)).ReturnsAsync(cotizacion);
            _mockSeguimientoRepo.Setup(repo => repo.Crear(It.IsAny<Seguimiento>())).ReturnsAsync(seguimiento);

            // Act
            await _seguimientoService.Crear(seguimiento);

            // Assert
            _mockClienteServices.Verify(s => s.Crear(It.IsAny<Cliente>()), Times.Never);
        }
        */
    }
}

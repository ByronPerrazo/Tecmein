
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
using DAL.DBContext;
using System.IO;
using BLL.DTOs;

namespace Tecmein.Tests
{
    public class ClienteServiceTests
    {
        private readonly Mock<IGenericRepository<Cliente>> _mockClienteRepo;
        private readonly Mock<IGenericRepository<FormatoNumeroCliente>> _mockFormatoRepo;
        private readonly Mock<IGenericRepository<Visita>> _mockVisitaRepo;
        private readonly Mock<IGenericRepository<Cotizacion>> _mockCotizacionRepo;
        private readonly Mock<IGenericRepository<Contrato>> _mockContratoRepo;
        private readonly Mock<IGenericRepository<PlanDePago>> _mockPlanDePagoRepo;
        private readonly Mock<IGenericRepository<Etapa>> _mockEtapaRepo;
        private readonly Mock<IGenericRepository<FormaPago>> _mockFormaPagoRepo;
        private readonly Mock<IConstructoraServices> _mockConstructoraServices;
        private readonly Mock<IStorageServices> _mockStorageServices;
        private readonly Mock<IGenericRepository<Cuota>> _mockCuotaRepo; // Nuevo
        private readonly Mock<TecmeindbContext> _mockDbContext;

        private readonly ClienteServices _clienteService;

        public ClienteServiceTests()
        {
            _mockClienteRepo = new Mock<IGenericRepository<Cliente>>();
            _mockFormatoRepo = new Mock<IGenericRepository<FormatoNumeroCliente>>();
            _mockVisitaRepo = new Mock<IGenericRepository<Visita>>();
            _mockCotizacionRepo = new Mock<IGenericRepository<Cotizacion>>();
            _mockContratoRepo = new Mock<IGenericRepository<Contrato>>();
            _mockPlanDePagoRepo = new Mock<IGenericRepository<PlanDePago>>();
            _mockEtapaRepo = new Mock<IGenericRepository<Etapa>>();
            _mockFormaPagoRepo = new Mock<IGenericRepository<FormaPago>>();
            _mockConstructoraServices = new Mock<IConstructoraServices>();
            _mockStorageServices = new Mock<IStorageServices>();
            _mockCuotaRepo = new Mock<IGenericRepository<Cuota>>(); // Nuevo
            _mockDbContext = new Mock<TecmeindbContext>();

            // Setup DbContext for transaction
            _mockDbContext.Setup(db => db.Database.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Mock<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction>().Object);

            _clienteService = new ClienteServices(
                _mockClienteRepo.Object,
                _mockFormatoRepo.Object,
                _mockVisitaRepo.Object,
                _mockCotizacionRepo.Object,
                _mockContratoRepo.Object,
                _mockPlanDePagoRepo.Object,
                _mockEtapaRepo.Object,
                _mockFormaPagoRepo.Object,
                _mockConstructoraServices.Object,
                _mockStorageServices.Object,
                _mockCuotaRepo.Object, // Nuevo
                _mockDbContext.Object);
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

        // [Fact]
        // public async Task RegistrarClienteHistorico_DebeCrearTodosLosRegistrosYSubirArchivo()
        // {
        //     // Arrange
        //     var registroDTO = new ClienteHistoricoRegistroDTO
        //     {
        //         SecConstructora = 1,
        //         NumeroCliente = "CLI-HIST-001",
        //         FechaFirma = "01/01/2023",
        //         SecFormaPago = 1,
        //         ValorContrato = 1000,
        //         ValorAnticipo = 100,
        //         FechaAnticipo = "01/01/2023",
        //         NumeroCuotas = 10,
        //         FechaPrimeraCuota = "01/02/2023"
        //     };
        //     var archivoStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("Contenido de prueba"));
        //     var nombreArchivo = "contrato.pdf";
        //     var usuarioId = 1;

        //     var constructora = new Constructora { Secuencial = 1, Nombre = "Constructora Histórica" };
        //     var etapaHistorico = new Etapa { Id = 1, Codigo = "HIST", Descripcion = "Histórico" };
        //     var formaPago = new FormaPago { SecFormaPago = 1, Descripcion = "Transferencia" };

        //     _mockClienteRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<Cliente, bool>>>(), null)).ReturnsAsync((Cliente)null);
        //     _mockConstructoraServices.Setup(s => s.ConstructoraPorSecuencial(1)).ReturnsAsync(constructora);
        //     _mockEtapaRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<Etapa, bool>>>(), null)).ReturnsAsync(etapaHistorico);
        //     _mockFormaPagoRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<FormaPago, bool>>>(), null)).ReturnsAsync(formaPago);

        //     _mockVisitaRepo.Setup(repo => repo.Crear(It.IsAny<Visita>())).ReturnsAsync((Visita v) => { v.Secuencial = 1; return v; });
        //     _mockCotizacionRepo.Setup(repo => repo.Crear(It.IsAny<Cotizacion>())).ReturnsAsync((Cotizacion c) => { c.Secuencial = 1; return c; });
        //     _mockClienteRepo.Setup(repo => repo.Crear(It.IsAny<Cliente>())).ReturnsAsync((Cliente cl) => { cl.SecCliente = 1; return cl; });
        //     _mockContratoRepo.Setup(repo => repo.Crear(It.IsAny<Contrato>())).ReturnsAsync((Contrato co) => { co.IdContrato = 1; return co; });
        //     _mockPlanDePagoRepo.Setup(repo => repo.Crear(It.IsAny<PlanDePago>())).ReturnsAsync((PlanDePago pp) => pp);
        //     _mockStorageServices.Setup(s => s.SubirStorage(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("http://url.com/contrato.pdf");

        //     // Act
        //     var resultado = await _clienteService.RegistrarClienteHistorico(registroDTO, archivoStream, nombreArchivo, usuarioId);

        //     // Assert
        //     Assert.NotNull(resultado);
        //     _mockVisitaRepo.Verify(repo => repo.Crear(It.IsAny<Visita>()), Times.Once);
        //     _mockCotizacionRepo.Verify(repo => repo.Crear(It.IsAny<Cotizacion>()), Times.Once);
        //     _mockClienteRepo.Verify(repo => repo.Crear(It.IsAny<Cliente>()), Times.Once);
        //     _mockContratoRepo.Verify(repo => repo.Crear(It.IsAny<Contrato>()), Times.Once);
        //     _mockPlanDePagoRepo.Verify(repo => repo.Crear(It.IsAny<PlanDePago>()), Times.Once);
        //     _mockStorageServices.Verify(s => s.SubirStorage(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        //     _mockDbContext.Verify(db => db.Database.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        //     _mockDbContext.Verify(db => db.Database.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        // }
    }
}

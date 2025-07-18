using Xunit;
using Moq;
using BLL.Implementacion;
using DAL.Interfaces;
using Entity;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.IO;
using BLL.Interfaces;
using System;

namespace Tecmein.Tests
{
    public class EmpresaServiceTests
    {
        private readonly Mock<IGenericRepository<Empresa>> _mockRepo;
        private readonly Mock<IStorageServices> _mockStorageService;
        private readonly Mock<IEmpresaStorageServices> _mockEmpresaStorageServices;
        private readonly EmpresaServices _service;

        public EmpresaServiceTests()
        {
            _mockRepo = new Mock<IGenericRepository<Empresa>>();
            _mockStorageService = new Mock<IStorageServices>();
            _mockEmpresaStorageServices = new Mock<IEmpresaStorageServices>();
            _service = new EmpresaServices(_mockRepo.Object, _mockStorageService.Object, _mockEmpresaStorageServices.Object);
        }

        private List<Empresa> GetTestEmpresas() => new List<Empresa>
        {
            new Empresa { Secuencial = 1, Identificacion = "0999999999", Nombre = "Empresa 1", UrlLogo = "url1", NombreLogo = "logo1.png" },
            new Empresa { Secuencial = 2, Identificacion = "0888888888", Nombre = "Empresa 2", UrlLogo = "url2", NombreLogo = "logo2.png" }
        };

        private List<Empresastorage> GetTestEmpresaStorage() => new List<Empresastorage>
        {
            new Empresastorage { SecEmpresa = 1, CarpetaLogo = "logos" }
        };

        [Fact]
        public async Task Crear_SinLogo_DebeCrearCorrectamente()
        {
            // Arrange
            var nuevaEmpresa = new Empresa { Identificacion = "0777777777", Nombre = "Nueva Empresa" };
            _mockRepo.Setup(r => r.Obtener(It.IsAny<Expression<Func<Empresa, bool>>>(), null)).ReturnsAsync((Empresa)null);
            _mockRepo.Setup(r => r.Crear(It.IsAny<Empresa>())).ReturnsAsync(nuevaEmpresa);

            // Act
            var resultado = await _service.Crear(nuevaEmpresa);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Nueva Empresa", resultado.Nombre);
            _mockStorageService.Verify(s => s.SubirStorage(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Crear_ConLogo_DebeSubirLogoYCrear()
        {
            // Arrange
            var nuevaEmpresa = new Empresa { Identificacion = "0777777777", Nombre = "Nueva Empresa" };
            var logoStream = new MemoryStream();
            _mockRepo.Setup(r => r.Obtener(It.IsAny<Expression<Func<Empresa, bool>>>(), null)).ReturnsAsync((Empresa)null);
            _mockEmpresaStorageServices.Setup(s => s.Consultar()).ReturnsAsync(GetTestEmpresaStorage());
            _mockStorageService.Setup(s => s.SubirStorage(It.IsAny<Stream>(), "logos", It.IsAny<string>()))
                               .ReturnsAsync("http://url.com/new_logo.png");
            _mockRepo.Setup(r => r.Crear(It.IsAny<Empresa>())).ReturnsAsync(nuevaEmpresa);

            // Act
            var resultado = await _service.Crear(nuevaEmpresa, logoStream, "new_logo.png");

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("http://url.com/new_logo.png", resultado.UrlLogo);
            Assert.Equal("new_logo.png", resultado.NombreLogo);
            _mockStorageService.Verify(s => s.SubirStorage(It.IsAny<Stream>(), "logos", "new_logo.png"), Times.Once);
        }

        [Fact]
        public async Task Crear_IdentificacionDuplicada_DebeLanzarExcepcion()
        {
            // Arrange
            var empresaExistente = GetTestEmpresas().First();
            _mockRepo.Setup(r => r.Obtener(It.IsAny<Expression<Func<Empresa, bool>>>(), null)).ReturnsAsync(empresaExistente);

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() => _service.Crear(new Empresa { Identificacion = empresaExistente.Identificacion }));
        }

        [Fact]
        public async Task Editar_SinLogo_DebeEditarCamposCorrectamente()
        {
            // Arrange
            var empresaOriginal = GetTestEmpresas().First();
            var empresaEditada = new Empresa { Secuencial = 1, Nombre = "Nombre Editado" };
            _mockRepo.Setup(r => r.Obtener(It.IsAny<Expression<Func<Empresa, bool>>>(), null)).ReturnsAsync(empresaOriginal);
            _mockRepo.Setup(r => r.Editar(It.IsAny<Empresa>())).ReturnsAsync(true);

            // Act
            var resultado = await _service.Editar(empresaEditada);

            // Assert
            Assert.Equal("Nombre Editado", resultado.Nombre);
            _mockStorageService.Verify(s => s.SubirStorage(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockStorageService.Verify(s => s.EliminarStorage(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Editar_ConLogo_DebeEliminarAnteriorYSubirNuevo()
        {
            // Arrange
            var empresaOriginal = GetTestEmpresas().First(); // Tiene logo1.png
            var empresaEditada = new Empresa { Secuencial = 1, Nombre = "Nombre Editado" };
            var logoStream = new MemoryStream();

            _mockRepo.Setup(r => r.Obtener(It.IsAny<Expression<Func<Empresa, bool>>>(), null)).ReturnsAsync(empresaOriginal);
            _mockRepo.Setup(r => r.Editar(It.IsAny<Empresa>())).ReturnsAsync(true);
            _mockEmpresaStorageServices.Setup(s => s.Consultar()).ReturnsAsync(GetTestEmpresaStorage());
            _mockStorageService.Setup(s => s.SubirStorage(It.IsAny<Stream>(), "logos", It.IsAny<string>()))
                               .ReturnsAsync("http://url.com/logo_editado.png");

            // Act
            var resultado = await _service.Editar(empresaEditada, logoStream, "logo_editado.png");

            // Assert
            Assert.Equal("http://url.com/logo_editado.png", resultado.UrlLogo);
            Assert.Equal("logo_editado.png", resultado.NombreLogo);
            _mockStorageService.Verify(s => s.EliminarStorage("logos", "logo1.png"), Times.Once);
            _mockStorageService.Verify(s => s.SubirStorage(It.IsAny<Stream>(), "logos", "logo_editado.png"), Times.Once);
        }

        [Fact]
        public async Task Editar_EmpresaNoExiste_DebeLanzarExcepcion()
        {
            // Arrange
            _mockRepo.Setup(r => r.Obtener(It.IsAny<Expression<Func<Empresa, bool>>>(), null)).ReturnsAsync((Empresa)null);

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() => _service.Editar(new Empresa { Secuencial = 99 }));
        }

        [Fact]
        public async Task Eliminar_DebeEliminarEmpresaYLogo()
        {
            // Arrange
            var empresaAEliminar = GetTestEmpresas().First();
            _mockRepo.Setup(r => r.Obtener(It.IsAny<Expression<Func<Empresa, bool>>>(), null)).ReturnsAsync(empresaAEliminar);
            _mockRepo.Setup(r => r.Eliminar(It.IsAny<Empresa>())).ReturnsAsync(true);
            _mockEmpresaStorageServices.Setup(s => s.Consultar()).ReturnsAsync(GetTestEmpresaStorage());

            // Act
            var resultado = await _service.Eliminar(1);

            // Assert
            Assert.True(resultado);
            _mockRepo.Verify(r => r.Eliminar(empresaAEliminar), Times.Once);
            _mockStorageService.Verify(s => s.EliminarStorage("logos", "logo1.png"), Times.Once);
        }
    }
}
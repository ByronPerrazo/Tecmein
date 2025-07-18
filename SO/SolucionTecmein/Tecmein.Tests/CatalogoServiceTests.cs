
using Xunit;
using Moq;
using BLL.Implementacion;
using DAL.Interfaces;
using Entity;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BLL.Interfaces;
using System.IO;

namespace Tecmein.Tests
{
    public class CatalogoServiceTests
    {
        private readonly Mock<IGenericRepository<Catalogo>> _mockRepo;
        private readonly Mock<IStorageServices> _mockStorageServices;
        private readonly Mock<IEmpresaStorageServices> _mockEmpresaStorageServices;
        private readonly Mock<IDatosGlobalesServices> _mockDatosGlobalesServices;
        private readonly CatalogoServices _service;

        public CatalogoServiceTests()
        {
            _mockRepo = new Mock<IGenericRepository<Catalogo>>();
            _mockStorageServices = new Mock<IStorageServices>();
            _mockEmpresaStorageServices = new Mock<IEmpresaStorageServices>();
            _mockDatosGlobalesServices = new Mock<IDatosGlobalesServices>();

            _service = new CatalogoServices(
                _mockRepo.Object,
                _mockStorageServices.Object,
                _mockEmpresaStorageServices.Object,
                _mockDatosGlobalesServices.Object
            );

            _mockDatosGlobalesServices.Setup(d => d.SecuencialEmpresaPrincipal).Returns(1);
            _mockDatosGlobalesServices.Setup(d => d.PathCatalogos).Returns("catalogos/");
        }

        private IQueryable<Catalogo> GetTestCatalogos()
        {
            return new List<Catalogo>
            {
                new Catalogo { Secuencial = 1, Nombre = "Catalogo 1", UrlCatalogo = "url1", NombreArchivo = "file1.pdf" },
                new Catalogo { Secuencial = 2, Nombre = "Catalogo 2", UrlCatalogo = "url2", NombreArchivo = "file2.pdf" }
            }.AsQueryable();
        }

        [Fact]
        public async Task CatalogoPorSecuencial_DebeDevolverCatalogoCorrecto()
        {
            // Arrange
            var testCatalogos = GetTestCatalogos();
            _mockRepo.Setup(repo => repo.Consultar(It.IsAny<Expression<Func<Catalogo, bool>>>())).ReturnsAsync(testCatalogos.Where(c => c.Secuencial == 1).AsQueryable());

            // Act
            var resultado = await _service.CatalogoPorSecuencial(1);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Catalogo 1", resultado.Nombre);
        }

        [Fact]
        public async Task CatalogoPorSecuencial_CatalogoNoExistente_DebeDevolverNull()
        {
            // Arrange
            var testCatalogos = GetTestCatalogos();
            _mockRepo.Setup(repo => repo.Consultar(It.IsAny<Expression<Func<Catalogo, bool>>>())).ReturnsAsync(testCatalogos.Where(c => c.Secuencial == 99).AsQueryable());

            // Act
            var resultado = await _service.CatalogoPorSecuencial(99);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public async Task CatalogosPorRol_DebeLanzarNotImplementedException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<NotImplementedException>(() => _service.CatalogosPorRol(1));
        }

        [Fact]
        public async Task Crear_CatalogoValido_DebeCrearCorrectamente()
        {
            // Arrange
            var nuevoCatalogo = new Catalogo { Nombre = "Nuevo Catalogo" };
            var empresaStorage = new Empresastorage { SecEmpresa = 1, CarpetaUsuario = "uploads/" };

            _mockRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<Catalogo, bool>>>(), null)).ReturnsAsync((Catalogo)null);
            _mockEmpresaStorageServices.Setup(s => s.Consultar()).ReturnsAsync(new List<Empresastorage> { empresaStorage });
            _mockStorageServices.Setup(s => s.SubirStorage(It.IsAny<Stream>(), _mockDatosGlobalesServices.Object.PathCatalogos, It.IsAny<string>())).ReturnsAsync("http://url.com/nuevo.pdf");
            _mockRepo.Setup(repo => repo.Crear(It.IsAny<Catalogo>())).ReturnsAsync((Catalogo c) => { c.Secuencial = 3; return c; });
            _mockRepo.Setup(repo => repo.Consultar(It.IsAny<Expression<Func<Catalogo, bool>>>())).ReturnsAsync((Expression<Func<Catalogo, bool>> filtro) => new List<Catalogo> { nuevoCatalogo }.AsQueryable().Where(filtro));

            // Act
            var resultado = await _service.Crear(nuevoCatalogo, new MemoryStream(), "nuevo.pdf");

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Nuevo Catalogo", resultado.Nombre);
            Assert.Equal("http://url.com/nuevo.pdf", resultado.UrlCatalogo);
            Assert.Equal("nuevo.pdf", resultado.NombreArchivo);
            _mockRepo.Verify(repo => repo.Crear(It.IsAny<Catalogo>()), Times.Once);
        }

        [Fact]
        public async Task Crear_ConNombreExistente_DebeLanzarExcepcion()
        {
            // Arrange
            var catalogoExistente = new Catalogo { Nombre = "Catalogo Existente" };
            _mockRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<Catalogo, bool>>>(), null)).ReturnsAsync(catalogoExistente);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<TaskCanceledException>(() => _service.Crear(new Catalogo { Nombre = "Catalogo Existente" }));
            Assert.Equal("El Nombre Ingresado Ya Existe", exception.Message);
        }

        [Fact]
        public async Task Crear_SinConfiguracionFTP_DebeLanzarExcepcion()
        {
            // Arrange
            var nuevoCatalogo = new Catalogo { Nombre = "Nuevo Catalogo" };
            _mockRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<Catalogo, bool>>>(), null)).ReturnsAsync((Catalogo)null);
            _mockEmpresaStorageServices.Setup(s => s.Consultar()).ReturnsAsync(new List<Empresastorage>()); // No FTP config

            // Act & Assert
            var exception = await Assert.ThrowsAsync<TaskCanceledException>(() => _service.Crear(nuevoCatalogo, new MemoryStream(), "test.pdf"));
            Assert.Equal("Error Empresa No ha definido un FTP", exception.Message);
        }

        [Fact]
        public async Task Crear_FalloSubidaStorage_DebeLanzarExcepcion()
        {
            // Arrange
            var nuevoCatalogo = new Catalogo { Nombre = "Nuevo Catalogo" };
            var empresaStorage = new Empresastorage { SecEmpresa = 1, CarpetaUsuario = "uploads/" };

            _mockRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<Catalogo, bool>>>(), null)).ReturnsAsync((Catalogo)null);
            _mockEmpresaStorageServices.Setup(s => s.Consultar()).ReturnsAsync(new List<Empresastorage> { empresaStorage });
            _mockStorageServices.Setup(s => s.SubirStorage(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((string)null); // Simula fallo

            // Act & Assert
            var exception = await Assert.ThrowsAsync<TaskCanceledException>(() => _service.Crear(nuevoCatalogo, new MemoryStream(), "nuevo.pdf"));
            Assert.Equal("Error No se genera una url para el Archivo", exception.Message);
        }

        [Fact]
        public async Task Editar_CatalogoValido_DebeEditarCorrectamente()
        {
            // Arrange
            var catalogoOriginal = new Catalogo { Secuencial = 1, Nombre = "Catalogo Original", UrlCatalogo = "urlOriginal", NombreArchivo = "fileOriginal.pdf", EstaActivo = 1 };
            var catalogoEditadoInput = new Catalogo { Secuencial = 1, Nombre = "Catalogo Editado", EstaActivo = 0 }; // Input to the service
            var empresaStorage = new Empresastorage { SecEmpresa = 1, CarpetaUsuario = "uploads/" };

            Catalogo capturedCatalogo = null; // To capture the object passed to _repositorio.Editar

            _mockRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<Catalogo, bool>>>(), null)).ReturnsAsync(catalogoOriginal);
            _mockEmpresaStorageServices.Setup(s => s.Consultar()).ReturnsAsync(new List<Empresastorage> { empresaStorage });
            _mockStorageServices.Setup(s => s.SubirStorage(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("http://url.com/editado.pdf");
            _mockRepo.Setup(repo => repo.Editar(It.IsAny<Catalogo>()))
                     .Callback<Catalogo>(c => capturedCatalogo = c) // Capture the object
                     .ReturnsAsync(true); // Simula que la edición es exitosa

            // The Consultar mock should return the captured object
            _mockRepo.Setup(repo => repo.Consultar(It.IsAny<Expression<Func<Catalogo, bool>>>()))
                     .ReturnsAsync((Expression<Func<Catalogo, bool>> filtro) =>
                     {
                         // Ensure capturedCatalogo is not null before using it
                         if (capturedCatalogo == null)
                         {
                             // This should not happen if Editar is called first
                             return Enumerable.Empty<Catalogo>().AsQueryable();
                         }
                         return new List<Catalogo> { capturedCatalogo }.AsQueryable().Where(filtro);
                     });

            // Act
            var resultado = await _service.Editar(catalogoEditadoInput, new MemoryStream(), "editado.pdf");

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Catalogo Editado", resultado.Nombre);
            Assert.Equal("http://url.com/editado.pdf", resultado.UrlCatalogo);
            Assert.Equal("editado.pdf", resultado.NombreArchivo);
            _mockRepo.Verify(repo => repo.Editar(It.IsAny<Catalogo>()), Times.Once);
            _mockRepo.Verify(repo => repo.Crear(It.IsAny<Catalogo>()), Times.Never);
        }

        [Fact]
        public async Task Editar_CatalogoNoExistente_DebeLanzarExcepcion()
        {
            // Arrange
            var catalogoInexistente = new Catalogo { Secuencial = 99, Nombre = "Inexistente" };
            _mockRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<Catalogo, bool>>>(), null)).ReturnsAsync((Catalogo)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<TaskCanceledException>(() => _service.Editar(catalogoInexistente));
            Assert.Equal("El Catalogo No Existe", exception.Message);
        }

        [Fact]
        public async Task Eliminar_CatalogoExistente_DebeEliminarCorrectamente()
        {
            // Arrange
            var catalogoAEliminar = new Catalogo { Secuencial = 1, Nombre = "Catalogo 1", NombreArchivo = "file1.pdf" };
            _mockRepo.Setup(repo => repo.Consultar(It.IsAny<Expression<Func<Catalogo, bool>>>())).ReturnsAsync(new List<Catalogo> { catalogoAEliminar }.AsQueryable());
            _mockStorageServices.Setup(s => s.EliminarStorage(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
            _mockRepo.Setup(repo => repo.Eliminar(It.IsAny<Catalogo>())).ReturnsAsync(true);

            // Act
            var resultado = await _service.Eliminar(1);

            // Assert
            Assert.True(resultado);
            _mockStorageServices.Verify(s => s.EliminarStorage("catalogos/", "file1.pdf"), Times.Once);
            _mockRepo.Verify(repo => repo.Eliminar(catalogoAEliminar), Times.Once);
        }

        [Fact]
        public async Task Eliminar_CatalogoNoExistente_DebeDevolverTrue()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.Consultar(It.IsAny<Expression<Func<Catalogo, bool>>>())).ReturnsAsync(new List<Catalogo>().AsQueryable());

            // Act
            var resultado = await _service.Eliminar(99);

            // Assert
            Assert.True(resultado); // Comportamiento actual del servicio
        }

        [Fact]
        public async Task Eliminar_FalloEliminarStorage_DebeLanzarExcepcion()
        {
            // Arrange
            var catalogoAEliminar = new Catalogo { Secuencial = 1, Nombre = "Catalogo 1", NombreArchivo = "file1.pdf" };
            _mockRepo.Setup(repo => repo.Consultar(It.IsAny<Expression<Func<Catalogo, bool>>>())).ReturnsAsync(new List<Catalogo> { catalogoAEliminar }.AsQueryable());
            _mockStorageServices.Setup(s => s.EliminarStorage(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false); // Simula fallo

            // Act & Assert
            var exception = await Assert.ThrowsAsync<TaskCanceledException>(() => _service.Eliminar(1));
            Assert.Equal("No se elmino el archivo del repositorio Web", exception.Message);
        }

        [Fact]
        public async Task Lista_DebeDevolverTodosLosCatalogos()
        {
            // Arrange
            var testCatalogos = GetTestCatalogos();
            _mockRepo.Setup(repo => repo.Consultar(null)).ReturnsAsync(testCatalogos);

            // Act
            var resultado = await _service.Lista();

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
        }
    }
}

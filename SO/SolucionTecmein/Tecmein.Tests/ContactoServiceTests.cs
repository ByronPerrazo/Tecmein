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
    public class ContactoServiceTests
    {
        private readonly Mock<IGenericRepository<Contacto>> _mockRepo;
        private readonly Mock<IGenericRepository<Contactovisita>> _mockContactoVisitaRepo;
        private readonly Mock<IConstructoraServices> _mockConstructoraServices;
        private readonly Mock<IValidacionServices> _mockValidacionServices;
        private readonly ContactoServices _service;

        public ContactoServiceTests()
        {
            _mockRepo = new Mock<IGenericRepository<Contacto>>();
            _mockContactoVisitaRepo = new Mock<IGenericRepository<Contactovisita>>();
            _mockConstructoraServices = new Mock<IConstructoraServices>();
            _mockValidacionServices = new Mock<IValidacionServices>();
            _service = new ContactoServices(
                _mockRepo.Object,
                _mockContactoVisitaRepo.Object,
                _mockConstructoraServices.Object,
                _mockValidacionServices.Object
            );
        }

        private List<Contacto> GetTestContactos() => new List<Contacto>
        {
            new Contacto { Secuencial = 1, Nombres = "Juan", Apellidos = "Perez", Correo = "juan.perez@example.com", Telefono = "0987654321", SecConstructora = 1, EstaActivo = 1 },
            new Contacto { Secuencial = 2, Nombres = "Maria", Apellidos = "Gomez", Correo = "maria.gomez@example.com", Telefono = "022345678", SecConstructora = 2, EstaActivo = 1 }
        };

        private List<Constructora> GetTestConstructoras() => new List<Constructora>
        {
            new Constructora { Secuencial = 1, Nombre = "Constructora A" },
            new Constructora { Secuencial = 2, Nombre = "Constructora B" }
        };

        private List<Contactovisita> GetTestContactoVisitas() => new List<Contactovisita>
        {
            new Contactovisita { SecVisita = 1, SecContacto = 1, EstaActivo = 1 },
            new Contactovisita { SecVisita = 2, SecContacto = 2, EstaActivo = 1 }
        };

        [Fact]
        public async Task Crear_DebeCrearContactoCorrectamente()
        {
            // Arrange
            var nuevaEntidad = new Contacto { Nombres = "Nuevo", Apellidos = "Contacto", Correo = "nuevo@example.com", Telefono = "0991234567", SecConstructora = 1, EstaActivo = 1 };
            var constructora = GetTestConstructoras().First();

            _mockRepo.Setup(r => r.Obtener(It.IsAny<Expression<Func<Contacto, bool>>>(), null)).ReturnsAsync((Contacto)null);
            _mockRepo.Setup(r => r.Crear(It.IsAny<Contacto>())).ReturnsAsync(nuevaEntidad);
            _mockConstructoraServices.Setup(s => s.ConstructoraPorSecuencial(1)).ReturnsAsync(constructora);

            // Act
            var resultado = await _service.Crear(nuevaEntidad);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Nuevo", resultado.Nombres);
            _mockRepo.Verify(r => r.Crear(It.IsAny<Contacto>()), Times.Once);
            _mockValidacionServices.Verify(v => v.ValidarNombre(nuevaEntidad.Nombres, "nombre"), Times.Once);
            _mockValidacionServices.Verify(v => v.ValidarCorreo(nuevaEntidad.Correo), Times.Once);
            _mockValidacionServices.Verify(v => v.ValidarTelefonoEcuador(nuevaEntidad.Telefono), Times.Once);
        }

        [Fact]
        public async Task Crear_ContactoDuplicado_DebeLanzarExcepcion()
        {
            // Arrange
            var entidadExistente = GetTestContactos().First();
            _mockRepo.Setup(r => r.Obtener(It.IsAny<Expression<Func<Contacto, bool>>>(), null)).ReturnsAsync(entidadExistente);
            _mockConstructoraServices.Setup(s => s.ConstructoraPorSecuencial(It.IsAny<int>())).ReturnsAsync(GetTestConstructoras().First());

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() => _service.Crear(entidadExistente));
        }

        [Fact]
        public async Task Crear_ConstructoraNoExiste_DebeLanzarExcepcion()
        {
            // Arrange
            var nuevaEntidad = new Contacto { Nombres = "Nuevo", Apellidos = "Contacto", SecConstructora = 99 };
            _mockRepo.Setup(r => r.Obtener(It.IsAny<Expression<Func<Contacto, bool>>>(), null)).ReturnsAsync((Contacto)null);
            _mockConstructoraServices.Setup(s => s.ConstructoraPorSecuencial(99)).ReturnsAsync((Constructora)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<TaskCanceledException>(() => _service.Crear(nuevaEntidad));
            Assert.Equal("La constructora con secuencial 99 no existe.", exception.Message);
        }

        [Fact]
        public async Task Editar_DebeEditarContactoCorrectamente()
        {
            // Arrange
            var contactoOriginal = GetTestContactos().First();
            var contactoEditado = new Contacto { Secuencial = 1, Nombres = "Juan Editado", Apellidos = "Perez", Correo = "juan.perez.editado@example.com", Telefono = "0987654321", SecConstructora = 1, EstaActivo = 1 };
            var constructora = GetTestConstructoras().First();

            _mockRepo.Setup(r => r.Obtener(It.IsAny<Expression<Func<Contacto, bool>>>(), null)).ReturnsAsync(contactoOriginal);
            _mockRepo.Setup(r => r.Editar(It.IsAny<Contacto>())).ReturnsAsync(true);
            _mockConstructoraServices.Setup(s => s.ConstructoraPorSecuencial(1)).ReturnsAsync(constructora);

            // Act
            var resultado = await _service.Editar(contactoEditado);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Juan Editado", resultado.Nombres);
            _mockRepo.Verify(r => r.Editar(It.IsAny<Contacto>()), Times.Once);
            _mockValidacionServices.Verify(v => v.ValidarNombre(contactoEditado.Nombres, "nombre"), Times.Once);
            _mockValidacionServices.Verify(v => v.ValidarCorreo(contactoEditado.Correo), Times.Once);
            _mockValidacionServices.Verify(v => v.ValidarTelefonoEcuador(contactoEditado.Telefono), Times.Once);
        }

        [Fact]
        public async Task Editar_ContactoNoExiste_DebeLanzarExcepcion()
        {
            // Arrange
            _mockRepo.Setup(r => r.Obtener(It.IsAny<Expression<Func<Contacto, bool>>>(), null)).ReturnsAsync((Contacto)null);
            _mockConstructoraServices.Setup(s => s.ConstructoraPorSecuencial(It.IsAny<int>())).ReturnsAsync(GetTestConstructoras().First());

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() => _service.Editar(new Contacto { Secuencial = 99 }));
        }

        [Fact]
        public async Task Eliminar_DebeEliminarContactoCorrectamente()
        {
            // Arrange
            var contactoAEliminar = GetTestContactos().First();
            _mockRepo.Setup(r => r.Obtener(c => c.Secuencial == 1, null)).ReturnsAsync(contactoAEliminar);
            _mockRepo.Setup(r => r.Eliminar(It.IsAny<Contacto>())).ReturnsAsync(true);

            // Act
            var resultado = await _service.Eliminar(1);

            // Assert
            Assert.True(resultado);
            _mockRepo.Verify(r => r.Eliminar(contactoAEliminar), Times.Once);
        }

        [Fact]
        public async Task Eliminar_ContactoNoExiste_DebeDevolverFalse()
        {
            // Arrange
            _mockRepo.Setup(r => r.Obtener(It.IsAny<Expression<Func<Contacto, bool>>>(), null)).ReturnsAsync((Contacto)null);

            // Act
            var resultado = await _service.Eliminar(99);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public async Task ObtenerContactoPrincipal_DebeDevolverContactoCorrecto()
        {
            // Arrange
            var contactoVisita = GetTestContactoVisitas().First();
            var contactoEsperado = GetTestContactos().First();

            _mockContactoVisitaRepo.Setup(r => r.Obtener(It.IsAny<Expression<Func<Contactovisita, bool>>>(), null)).ReturnsAsync(contactoVisita);
            _mockRepo.Setup(r => r.Obtener(It.IsAny<Expression<Func<Contacto, bool>>>(), null)).ReturnsAsync(contactoEsperado);

            // Act
            var resultado = await _service.ObtenerContactoPrincipal(1);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(contactoEsperado.Secuencial, resultado.Secuencial);
        }

        [Fact]
        public async Task ObtenerContactoPrincipal_ContactoVisitaNoExiste_DebeDevolverNull()
        {
            // Arrange
            _mockContactoVisitaRepo.Setup(r => r.Obtener(It.IsAny<Expression<Func<Contactovisita, bool>>>(), null)).ReturnsAsync((Contactovisita)null);

            // Act
            var resultado = await _service.ObtenerContactoPrincipal(99);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public async Task Lista_DebeDevolverTodosLosContactosConConstructora()
        {
            // Arrange
            var contactos = GetTestContactos();
            var constructoras = GetTestConstructoras();
            contactos[0].SecConstructoraNavigation = constructoras[0];
            contactos[1].SecConstructoraNavigation = constructoras[1];

            var asyncContactos = new TestAsyncEnumerable<Contacto>(contactos);
            _mockRepo.Setup(repo => repo.Consultar(null)).ReturnsAsync(asyncContactos);

            // Act
            var resultado = await _service.Lista();

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
            Assert.True(resultado.All(c => c.SecConstructoraNavigation != null));
        }

        [Fact]
        public async Task ListaPorConstructora_DebeDevolverContactosFiltradosConConstructora()
        {
            // Arrange
            var contactos = GetTestContactos();
            var constructoras = GetTestConstructoras();
            contactos[0].SecConstructoraNavigation = constructoras[0];
            contactos[1].SecConstructoraNavigation = constructoras[1];

            var asyncContactos = new TestAsyncEnumerable<Contacto>(contactos.Where(c => c.SecConstructora == 1));
            _mockRepo.Setup(repo => repo.Consultar(It.IsAny<Expression<Func<Contacto, bool>>>()))
                     .ReturnsAsync(asyncContactos);

            // Act
            var resultado = await _service.ListaPorConstructora(1);

            // Assert
            Assert.NotNull(resultado);
            Assert.Single(resultado);
            Assert.Equal(1, resultado.First().SecConstructora);
            Assert.NotNull(resultado.First().SecConstructoraNavigation);
        }
    }
}
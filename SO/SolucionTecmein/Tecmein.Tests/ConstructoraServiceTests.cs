
using Xunit;
using Moq;
using BLL.Implementacion;
using DAL.Interfaces;
using Entity;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Tecmein.Tests
{
    public class ConstructoraServiceTests
    {
        private readonly Mock<IGenericRepository<Constructora>> _mockRepo;
        private readonly ConstructoraServices _service;

        public ConstructoraServiceTests()
        {
            _mockRepo = new Mock<IGenericRepository<Constructora>>();
            _service = new ConstructoraServices(_mockRepo.Object);
        }

        [Fact]
        public async Task GuardarCambios_ConNuevoNombre_DebeGuardarCorrectamente()
        {
            // Arrange
            var nuevaConstructora = new Constructora { Nombre = "Constructora Nueva", Direccion = "Direccion" };
            _mockRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<Constructora, bool>>>(), null)).ReturnsAsync((Constructora?)null);
            _mockRepo.Setup(repo => repo.Crear(nuevaConstructora)).ReturnsAsync(nuevaConstructora);

            // Act
            var resultado = await _service.GuardarCambios(nuevaConstructora);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Constructora Nueva", resultado.Nombre);
            _mockRepo.Verify(repo => repo.Crear(It.IsAny<Constructora>()), Times.Once);
        }

        [Fact]
        public async Task GuardarCambios_ConNombreExistente_DebeLanzarExcepcion()
        {
            // Arrange
            var constructoraExistente = new Constructora { Nombre = "Constructora Existente" };
            _mockRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<Constructora, bool>>>(), null)).ReturnsAsync(constructoraExistente);

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() => _service.GuardarCambios(constructoraExistente));
        }

        [Fact]
        public async Task Editar_ConDatosValidos_DebeEditarCorrectamente()
        {
            // Arrange
            var constructoraOriginal = new Constructora { Secuencial = 1, Nombre = "Original", Direccion = "Vieja Direccion" };
            var constructoraEditada = new Constructora { Secuencial = 1, Nombre = "Editado", Direccion = "Nueva Direccion" };

            _mockRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<Constructora, bool>>>(), null)).ReturnsAsync(constructoraOriginal);
            _mockRepo.Setup(repo => repo.Editar(It.IsAny<Constructora>())).ReturnsAsync(true);

            // Act
            var resultado = await _service.Editar(constructoraEditada);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Editado", resultado.Nombre);
            Assert.Equal("Nueva Direccion", resultado.Direccion);
            _mockRepo.Verify(repo => repo.Editar(It.Is<Constructora>(c => c.Nombre == "Editado")), Times.Once);
        }

        [Fact]
        public async Task Editar_ConNombreExistente_DebeLanzarExcepcion()
        {
            // Arrange
            var constructoraAEditar = new Constructora { Secuencial = 1, Nombre = "Constructora A" };
            var constructoraBExistente = new Constructora { Secuencial = 2, Nombre = "Constructora B" };
            var datosParaEditar = new Constructora { Secuencial = 1, Nombre = "Constructora B" }; // Intentando renombrar A a B

            _mockRepo.Setup(repo => repo.Obtener(x => x.Secuencial == constructoraAEditar.Secuencial, null)).ReturnsAsync(constructoraAEditar);
            // Simula que ya existe una constructora con el nombre "Constructora B"
            _mockRepo.Setup(repo => repo.Obtener(x => x.Nombre == datosParaEditar.Nombre, null)).ReturnsAsync(constructoraBExistente);

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() => _service.Editar(datosParaEditar));
        }

        [Fact]
        public async Task Editar_ConstructoraNoExistente_DebeLanzarExcepcion()
        {
            // Arrange
            var constructoraInexistente = new Constructora { Secuencial = 99, Nombre = "Inexistente" };
            _mockRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<Constructora, bool>>>(), null)).ReturnsAsync((Constructora?)null);

            // Act & Assert
            await Assert.ThrowsAsync<TaskCanceledException>(() => _service.Editar(constructoraInexistente));
        }

        [Fact]
        public async Task Eliminar_ConstructoraNoExistente_DebeDevolverFalse()
        {
            // Arrange
            int secuencialInexistente = 99;
            _mockRepo.Setup(repo => repo.Consultar(x => x.Secuencial == secuencialInexistente)).ReturnsAsync(new List<Constructora>().AsQueryable());

            // Act
            var resultado = await _service.Eliminar(secuencialInexistente);

            // Assert
            Assert.False(resultado);
        }
    }
}

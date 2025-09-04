using BLL.Implementacion;
using DAL.Interfaces;
using Entity;
using Moq;
using System.Linq.Expressions;

namespace Tecmein.Tests
{
    public class PlantillaPreContratoParrafoServiceTests
    {
        private readonly Mock<IGenericRepository<PlantillaPreContratoParrafo>> _mockRepo;
        private readonly PlantillaPreContratoParrafoServices _service;

        public PlantillaPreContratoParrafoServiceTests()
        {
            _mockRepo = new Mock<IGenericRepository<PlantillaPreContratoParrafo>>();
            _service = new PlantillaPreContratoParrafoServices(_mockRepo.Object);
        }

        [Fact]
        public async Task Crear_ParrafoValido_DebeCrearCorrectamente()
        {
            // Arrange
            var parrafo = new PlantillaPreContratoParrafo { SecPlantillaPreContrato = 1, Orden = 1, Contenido = "Contenido de prueba" };
            _mockRepo.Setup(repo => repo.Crear(It.IsAny<PlantillaPreContratoParrafo>()))
                     .ReturnsAsync((PlantillaPreContratoParrafo p) =>
                     {
                         p.EstaActivo = true; // El servicio lo establece en true
                         return p;
                     });

            // Act
            var resultado = await _service.Crear(parrafo);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Contenido de prueba", resultado.Contenido);
            Assert.True(resultado.EstaActivo);
            _mockRepo.Verify(repo => repo.Crear(It.IsAny<PlantillaPreContratoParrafo>()), Times.Once);
        }

        [Fact]
        public async Task Editar_ParrafoExistente_DebeEditarCorrectamente()
        {
            // Arrange
            var parrafoExistente = new PlantillaPreContratoParrafo { SecPlantillaPreContratoParrafo = 1, SecPlantillaPreContrato = 1, Orden = 1, Contenido = "Original", EstaActivo = true };
            var parrafoEditado = new PlantillaPreContratoParrafo { SecPlantillaPreContratoParrafo = 1, SecPlantillaPreContrato = 1, Orden = 2, Contenido = "Editado", EstaActivo = false };

            _mockRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<PlantillaPreContratoParrafo, bool>>>(), null)).ReturnsAsync(parrafoExistente);
            _mockRepo.Setup(repo => repo.Editar(It.IsAny<PlantillaPreContratoParrafo>())).ReturnsAsync(true);

            // Act
            var resultado = await _service.Editar(parrafoEditado);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Editado", resultado.Contenido);
            Assert.Equal(2, resultado.Orden);
            Assert.False(resultado.EstaActivo);
            _mockRepo.Verify(repo => repo.Editar(It.IsAny<PlantillaPreContratoParrafo>()), Times.Once);
        }

        [Fact]
        public async Task Eliminar_ParrafoExistente_DebeEliminarCorrectamente()
        {
            // Arrange
            var parrafoExistente = new PlantillaPreContratoParrafo { SecPlantillaPreContratoParrafo = 1, SecPlantillaPreContrato = 1, Orden = 1, Contenido = "Para eliminar", EstaActivo = true };

            _mockRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<PlantillaPreContratoParrafo, bool>>>(), null)).ReturnsAsync(parrafoExistente);
            _mockRepo.Setup(repo => repo.Eliminar(It.IsAny<PlantillaPreContratoParrafo>())).ReturnsAsync(true);

            // Act
            var resultado = await _service.Eliminar(1);

            // Assert
            Assert.True(resultado);
            _mockRepo.Verify(repo => repo.Eliminar(It.IsAny<PlantillaPreContratoParrafo>()), Times.Once);
        }
    }
}
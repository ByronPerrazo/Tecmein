using Xunit;
using Moq;
using BLL.Implementacion;
using DAL.Interfaces;
using Entity;
using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;

namespace Tecmein.Tests
{
    public class PlantillaPreContratoServiceTests
    {
        private readonly Mock<IGenericRepository<PlantillaPreContrato>> _mockRepo;
        private readonly PlantillaPreContratoServices _service;

        public PlantillaPreContratoServiceTests()
        {
            _mockRepo = new Mock<IGenericRepository<PlantillaPreContrato>>();
            _service = new PlantillaPreContratoServices(_mockRepo.Object);
        }

        [Fact]
        public async Task Crear_PlantillaValida_DebeCrearCorrectamente()
        {
            // Arrange
            var plantilla = new PlantillaPreContrato { Nombre = "Plantilla Test", NumeracionInicial = "PT-" };
            _mockRepo.Setup(repo => repo.Crear(It.IsAny<PlantillaPreContrato>())).ReturnsAsync(plantilla);

            // Act
            var resultado = await _service.Crear(plantilla);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Plantilla Test", resultado.Nombre);
            Assert.Equal("PT-", resultado.NumeracionInicial);
            Assert.Equal(1, resultado.EstaActivo);
            _mockRepo.Verify(repo => repo.Crear(It.IsAny<PlantillaPreContrato>()), Times.Once);
        }

        [Fact]
        public async Task Editar_PlantillaExistente_DebeEditarCorrectamente()
        {
            // Arrange
            var plantillaExistente = new PlantillaPreContrato { SecPlantillaPreContrato = 1, Nombre = "Original", NumeracionInicial = "OR-", EstaActivo = 1 };
            var plantillaEditada = new PlantillaPreContrato { SecPlantillaPreContrato = 1, Nombre = "Editada", NumeracionInicial = "ED-", EstaActivo = 0 };

            _mockRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<PlantillaPreContrato, bool>>>(), null)).ReturnsAsync(plantillaExistente);
            _mockRepo.Setup(repo => repo.Editar(It.IsAny<PlantillaPreContrato>())).ReturnsAsync(true);

            // Act
            var resultado = await _service.Editar(plantillaEditada);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Editada", resultado.Nombre);
            Assert.Equal("ED-", resultado.NumeracionInicial);
            Assert.Equal(0, resultado.EstaActivo);
            _mockRepo.Verify(repo => repo.Editar(It.IsAny<PlantillaPreContrato>()), Times.Once);
        }

        [Fact]
        public async Task Eliminar_PlantillaExistente_DebeEliminarCorrectamente()
        {
            // Arrange
            var plantillaExistente = new PlantillaPreContrato { SecPlantillaPreContrato = 1 };

            _mockRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<PlantillaPreContrato, bool>>>(), null)).ReturnsAsync(plantillaExistente);
            _mockRepo.Setup(repo => repo.Eliminar(It.IsAny<PlantillaPreContrato>())).ReturnsAsync(true);

            // Act
            var resultado = await _service.Eliminar(1);

            // Assert
            Assert.True(resultado);
            _mockRepo.Verify(repo => repo.Eliminar(It.IsAny<PlantillaPreContrato>()), Times.Once);
        }
    }
}

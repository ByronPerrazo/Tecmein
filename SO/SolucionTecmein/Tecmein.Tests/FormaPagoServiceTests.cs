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
    public class FormaPagoServiceTests
    {
        private readonly Mock<IGenericRepository<FormaPago>> _mockRepo;
        private readonly FormaPagoServices _service;

        public FormaPagoServiceTests()
        {
            _mockRepo = new Mock<IGenericRepository<FormaPago>>();
            _service = new FormaPagoServices(_mockRepo.Object);
        }

        [Fact]
        public async Task Crear_FormaPagoValida_DebeCrearCorrectamente()
        {
            // Arrange
            var formaPago = new FormaPago { Descripcion = "Efectivo" };
            _mockRepo.Setup(repo => repo.Crear(It.IsAny<FormaPago>())).ReturnsAsync(formaPago);

            // Act
            var resultado = await _service.Crear(formaPago);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Efectivo", resultado.Descripcion);
            Assert.Equal(1, resultado.EstaActivo);
            _mockRepo.Verify(repo => repo.Crear(It.IsAny<FormaPago>()), Times.Once);
        }

        [Fact]
        public async Task Editar_FormaPagoExistente_DebeEditarCorrectamente()
        {
            // Arrange
            var formaPagoExistente = new FormaPago { SecFormaPago = 1, Descripcion = "Efectivo", EstaActivo = 1 };
            var formaPagoEditada = new FormaPago { SecFormaPago = 1, Descripcion = "Tarjeta", EstaActivo = 0 };

            _mockRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<FormaPago, bool>>>(), null)).ReturnsAsync(formaPagoExistente);
            _mockRepo.Setup(repo => repo.Editar(It.IsAny<FormaPago>())).ReturnsAsync(true);

            // Act
            var resultado = await _service.Editar(formaPagoEditada);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Tarjeta", resultado.Descripcion);
            Assert.Equal(0, resultado.EstaActivo);
            _mockRepo.Verify(repo => repo.Editar(It.IsAny<FormaPago>()), Times.Once);
        }

        [Fact]
        public async Task Eliminar_FormaPagoExistente_DebeEliminarCorrectamente()
        {
            // Arrange
            var formaPagoExistente = new FormaPago { SecFormaPago = 1 };

            _mockRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<FormaPago, bool>>>(), null)).ReturnsAsync(formaPagoExistente);
            _mockRepo.Setup(repo => repo.Eliminar(It.IsAny<FormaPago>())).ReturnsAsync(true);

            // Act
            var resultado = await _service.Eliminar(1);

            // Assert
            Assert.True(resultado);
            _mockRepo.Verify(repo => repo.Eliminar(It.IsAny<FormaPago>()), Times.Once);
        }
    }
}

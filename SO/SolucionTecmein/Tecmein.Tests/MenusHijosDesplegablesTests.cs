using Xunit;
using Moq;
using BLL.Implementacion;
using DAL.Interfaces;
using Entity;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System;

namespace Tecmein.Tests
{
    public class MenusHijosDesplegablesTests
    {
        private readonly Mock<IGenericRepository<Menu>> _mockMenuRepo;
        private readonly MenusHijosDesplegables _service;

        public MenusHijosDesplegablesTests()
        {
            _mockMenuRepo = new Mock<IGenericRepository<Menu>>();
            _service = new MenusHijosDesplegables(_mockMenuRepo.Object);
        }

        private List<Menu> GetTestMenus() => new List<Menu>
        {
            new Menu { Secuencial = 1, Descripcion = "Parent", SecMenuPadre = null, EsActivo = 1 },
            new Menu { Secuencial = 2, Descripcion = "Child1", SecMenuPadre = 1, EsActivo = 1 },
            new Menu { Secuencial = 3, Descripcion = "Child2", SecMenuPadre = 1, EsActivo = 1 },
            new Menu { Secuencial = 4, Descripcion = "Leaf", SecMenuPadre = 2, EsActivo = 1 },
            new Menu { Secuencial = 5, Descripcion = "Inactive Parent", SecMenuPadre = null, EsActivo = 0 },
            new Menu { Secuencial = 6, Descripcion = "Inactive Child", SecMenuPadre = 5, EsActivo = 1 }
        };

        [Fact]
        public async Task ObtenerMenusHijos_DebeDevolverSoloMenusHijosDeUltimoNivelActivos()
        {
            // Arrange
            var allMenus = GetTestMenus();
            var asyncAllMenus = new TestAsyncEnumerable<Menu>(allMenus);
            _mockMenuRepo.Setup(repo => repo.Consultar(It.IsAny<Expression<Func<Menu, bool>>>()))
                         .ReturnsAsync(asyncAllMenus);

            // Act
            var resultado = await _service.ObtenerMenusHijos();

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(3, resultado.Count); // Expected: Child2, Leaf, Inactive Child
            Assert.Contains(resultado, m => m.Secuencial == 3);
            Assert.Contains(resultado, m => m.Secuencial == 4);
            Assert.Contains(resultado, m => m.Secuencial == 6);
            Assert.DoesNotContain(resultado, m => m.Secuencial == 1);
            Assert.DoesNotContain(resultado, m => m.Secuencial == 2);
            Assert.DoesNotContain(resultado, m => m.Secuencial == 5);
        }

        [Fact]
        public async Task ObtenerMenusHijos_SinMenus_DebeDevolverListaVacia()
        {
            // Arrange
            var emptyMenus = new TestAsyncEnumerable<Menu>(new List<Menu>());
            _mockMenuRepo.Setup(repo => repo.Consultar(It.IsAny<Expression<Func<Menu, bool>>>()))
                         .ReturnsAsync(emptyMenus);

            // Act
            var resultado = await _service.ObtenerMenusHijos();

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado);
        }
    }
}

using BLL.Implementacion;
using DAL.Interfaces;
using Entity;
using Moq;
using System.Linq.Expressions;

namespace Tecmein.Tests
{
    public class MenuServiceTests
    {
        private readonly Mock<IGenericRepository<Menu>> _mockMenuRepo;
        private readonly Mock<IGenericRepository<RolMenu>> _mockRolMenuRepo;
        private readonly Mock<IGenericRepository<Usuario>> _mockUsuarioRepo;
        private readonly MenuServices _service;

        public MenuServiceTests()
        {
            _mockMenuRepo = new Mock<IGenericRepository<Menu>>();
            _mockRolMenuRepo = new Mock<IGenericRepository<RolMenu>>();
            _mockUsuarioRepo = new Mock<IGenericRepository<Usuario>>();
            _service = new MenuServices(_mockMenuRepo.Object, _mockRolMenuRepo.Object, _mockUsuarioRepo.Object);
        }

        private List<Menu> GetTestMenus()
        {
            var menus = new List<Menu>
            {
                new Menu { Secuencial = 1, Descripcion = "Menu Padre 1", SecMenuPadre = null, EsActivo = 1, Icono = "icon1" },
                new Menu { Secuencial = 2, Descripcion = "Menu Hijo 1.1", SecMenuPadre = 1, EsActivo = 1, Icono = "icon2" },
                new Menu { Secuencial = 3, Descripcion = "Menu Hijo 1.2", SecMenuPadre = 1, EsActivo = 1, Icono = "icon3" },
                new Menu { Secuencial = 4, Descripcion = "Menu Padre 2", SecMenuPadre = null, EsActivo = 1, Icono = "icon4" },
                new Menu { Secuencial = 5, Descripcion = "Menu Inactivo", SecMenuPadre = null, EsActivo = 0, Icono = "icon5" }
            };

            foreach (var menu in menus)
            {
                menu.InverseSecMenuPadreNavigation = menus.Where(m => m.SecMenuPadre == menu.Secuencial).ToList();
            }

            return menus;
        }

        private List<RolMenu> GetTestRolMenus() => new List<RolMenu>
        {
            new RolMenu { Secuencial = 1, SecRol = 1, SecMenu = 1, EsActivo = 1 },
            new RolMenu { Secuencial = 2, SecRol = 1, SecMenu = 2, EsActivo = 1 },
            new RolMenu { Secuencial = 3, SecRol = 1, SecMenu = 3, EsActivo = 1 },
            new RolMenu { Secuencial = 4, SecRol = 2, SecMenu = 4, EsActivo = 1 }
        };

        private List<Usuario> GetTestUsuarios() => new List<Usuario>
        {
            new Usuario { Secuencial = 1, SecRol = 1, EsActivo = 1 },
            new Usuario { Secuencial = 2, SecRol = 2, EsActivo = 1 },
            new Usuario { Secuencial = 3, SecRol = 3, EsActivo = 1 }
        };

        [Fact]
        public async Task ObtenerTodosPadre_DebeDevolverSoloMenusPadreActivos()
        {
            // Arrange
            var menus = GetTestMenus();
            var asyncMenus = new TestAsyncEnumerable<Menu>(menus.Where(m => m.SecMenuPadre == null && m.EsActivo == 1));
            _mockMenuRepo.Setup(repo => repo.Consultar(It.IsAny<Expression<Func<Menu, bool>>>()))
                         .ReturnsAsync(asyncMenus);

            // Act
            var resultado = await _service.ObtenerTodosPadre();

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
            Assert.True(resultado.All(m => m.SecMenuPadre == null && m.EsActivo == 1));
        }

        [Fact]
        public async Task ObtieneMenu_UsuarioConPermisos_DebeDevolverMenusCorrectos()
        {
            // Arrange
            var menus = GetTestMenus();
            var rolMenus = GetTestRolMenus();
            var usuarios = GetTestUsuarios();

            _mockUsuarioRepo.Setup(r => r.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>(), null)).ReturnsAsync(usuarios.First(u => u.Secuencial == 1));

            var asyncRolMenus = new TestAsyncEnumerable<RolMenu>(rolMenus.Where(rm => rm.SecRol == 1));
            _mockRolMenuRepo.Setup(r => r.Consultar(It.IsAny<Expression<Func<RolMenu, bool>>>()))
                            .ReturnsAsync(asyncRolMenus);

            var asyncMenus = new TestAsyncEnumerable<Menu>(menus.Where(m => m.EsActivo == 1));
            _mockMenuRepo.Setup(r => r.Consultar(It.IsAny<Expression<Func<Menu, bool>>>()))
                         .ReturnsAsync(asyncMenus);

            // Act
            var resultado = await _service.ObtieneMenu(1);

            // Assert
            Assert.NotNull(resultado);
            Assert.Single(resultado);
            var menuPadre = resultado.First();
            Assert.Equal(1, menuPadre.Secuencial);
            Assert.Equal(2, menuPadre.InverseSecMenuPadreNavigation.Count);
        }

        [Fact]
        public async Task ObtieneMenuTotal_DebeDevolverTodosLosMenus()
        {
            // Arrange
            var menus = GetTestMenus();
            var asyncMenus = new TestAsyncEnumerable<Menu>(menus);
            _mockMenuRepo.Setup(repo => repo.Consultar(null)).ReturnsAsync(asyncMenus);

            // Act
            var resultado = await _service.ObtieneMenuTotal();

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(5, resultado.Count);
        }
    }
}
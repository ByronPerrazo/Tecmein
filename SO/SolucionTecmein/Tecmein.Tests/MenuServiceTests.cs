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
        private readonly Mock<IGenericRepository<RolPermiso>> _mockRolPermisoRepo; // CAMBIO
        private readonly Mock<IGenericRepository<Usuario>> _mockUsuarioRepo;
        private readonly Mock<IGenericRepository<RolMenu>> _mockRolMenuRepo;
        private readonly MenuServices _service;

        public MenuServiceTests()
        {
            _mockMenuRepo = new Mock<IGenericRepository<Menu>>();
            _mockRolPermisoRepo = new Mock<IGenericRepository<RolPermiso>>(); // CAMBIO
            _mockUsuarioRepo = new Mock<IGenericRepository<Usuario>>();
            _mockRolMenuRepo = new Mock<IGenericRepository<RolMenu>>();
            // CAMBIO: Se pasa el nuevo mock al constructor
            _service = new MenuServices(_mockMenuRepo.Object, _mockRolPermisoRepo.Object, _mockUsuarioRepo.Object, _mockRolMenuRepo.Object);
        }

        // --- Datos de Prueba --- //

        private List<Menu> GetTestMenus() => new List<Menu>
        {
            new Menu { Secuencial = 1, Descripcion = "Menu Padre 1", SecMenuPadre = null, EsActivo = 1 },
            new Menu { Secuencial = 2, Descripcion = "Menu Hijo 1.1", SecMenuPadre = 1, EsActivo = 1 },
            new Menu { Secuencial = 3, Descripcion = "Menu Hijo 1.2", SecMenuPadre = 1, EsActivo = 1 },
            new Menu { Secuencial = 4, Descripcion = "Menu Padre 2", SecMenuPadre = null, EsActivo = 1 },
            new Menu { Secuencial = 5, Descripcion = "Menu Inactivo", SecMenuPadre = null, EsActivo = 0 }
        };

        // CAMBIO: Nuevo método para simular la tabla RolPermiso
        private List<RolPermiso> GetTestRolPermisos() => new List<RolPermiso>
        {
            // Rol 1 tiene permiso para ver los menús 1, 2 y 3
            new RolPermiso { SecRol = 1, IdPermiso = "Menu.Ver.MenuPadre1" },
            new RolPermiso { SecRol = 1, IdPermiso = "Menu.Ver.MenuHijo1.1" },
            new RolPermiso { SecRol = 1, IdPermiso = "Menu.Ver.MenuHijo1.2" },
            // Rol 2 tiene permiso para ver el menú 4
            new RolPermiso { SecRol = 2, IdPermiso = "Menu.Ver.MenuPadre2" }
        };

        private List<Usuario> GetTestUsuarios() => new List<Usuario>
        {
            new Usuario { Secuencial = 1, SecRol = 1, EsActivo = 1 },
            new Usuario { Secuencial = 2, SecRol = 2, EsActivo = 1 }
        };

        // --- Pruebas --- //

        [Fact]
        public async Task ObtieneMenu_UsuarioConPermisos_DebeDevolverMenusCorrectos()
        {
            // Arrange
            var menus = GetTestMenus();
            var rolPermisos = GetTestRolPermisos();
            var usuarios = GetTestUsuarios();

            _mockUsuarioRepo.Setup(r => r.Obtener(It.IsAny<Expression<Func<Usuario, bool>>>(), null)).ReturnsAsync(usuarios.First(u => u.Secuencial == 1));

            // CAMBIO: Configurar el mock para _mockRolPermisoRepo
            var asyncRolPermisos = new TestAsyncEnumerable<RolPermiso>(rolPermisos.Where(rp => rp.SecRol == 1));
            _mockRolPermisoRepo.Setup(r => r.Consultar(It.IsAny<Expression<Func<RolPermiso, bool>>>()))
                               .ReturnsAsync(asyncRolPermisos);

            var asyncMenus = new TestAsyncEnumerable<Menu>(menus.Where(m => m.EsActivo == 1));
            _mockMenuRepo.Setup(r => r.Consultar(It.IsAny<Expression<Func<Menu, bool>>>()))
                         .ReturnsAsync(asyncMenus);

            // Act
            var resultado = await _service.ObtieneMenu(1);

            // Assert
            Assert.NotNull(resultado);
            Assert.Single(resultado); // Debe devolver solo el menú padre raíz
            var menuPadre = resultado.First();
            Assert.Equal(1, menuPadre.Secuencial);
            // La lógica interna debe reconstruir la jerarquía
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

        [Fact]
        public async Task Eliminar_MenuConRolMenuAsociado_DebeEliminarAmbos()
        {
            // Arrange
            var menuId = 1;
            var menu = new Menu { Secuencial = menuId, Descripcion = "Menu de Prueba" };
            var rolMenus = new List<RolMenu>
            {
                new RolMenu { Secuencial = 1, SecMenu = menuId, SecRol = 1 },
                new RolMenu { Secuencial = 2, SecMenu = menuId, SecRol = 2 }
            };
            var asyncRolMenus = new TestAsyncEnumerable<RolMenu>(rolMenus);

            _mockMenuRepo.Setup(r => r.Obtener(It.IsAny<Expression<Func<Menu, bool>>>(), null)).ReturnsAsync(menu);
           // _mockRolMenuRepo.Setup(r => r.Consultar(It.IsAny<Expression<Func<RolMenu, bool>>>(), null)).ReturnsAsync(asyncRolMenus);
            _mockMenuRepo.Setup(r => r.Eliminar(It.IsAny<Menu>())).ReturnsAsync(true);
            _mockRolMenuRepo.Setup(r => r.Eliminar(It.IsAny<RolMenu>())).ReturnsAsync(true);

            // Act
            var result = await _service.Eliminar(menuId);

            // Assert
            Assert.True(result);
            _mockRolMenuRepo.Verify(r => r.Eliminar(It.IsAny<RolMenu>()), Times.Exactly(2));
            _mockMenuRepo.Verify(r => r.Eliminar(It.Is<Menu>(m => m.Secuencial == menuId)), Times.Once);
        }
    }
}

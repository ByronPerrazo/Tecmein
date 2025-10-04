
using Xunit;
using Moq;
using BLL.Implementacion;
using DAL.DBContext;
using Entity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Tecmein.Tests
{
    // TODO: Reescribir estas pruebas para usar el nuevo modelo de autorización basado en RolPermiso.
    // La entidad Permisosrol ha sido eliminada.
    /*
    public class AutorizacionServiceTests
    {
        private readonly Mock<TecmeindbContext> _mockContext;
        private readonly Mock<DbSet<Permisosrol>> _mockDbSet;
        private readonly AutorizacionService _service;

        public AutorizacionServiceTests()
        {
            var options = new DbContextOptionsBuilder<TecmeindbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _mockContext = new Mock<TecmeindbContext>(options);
            _mockDbSet = new Mock<DbSet<Permisosrol>>();

            // Configurar el DbSet para que se comporte como una colección en memoria
            var data = new List<Permisosrol>
            {
                new Permisosrol { SecRol = 1, Consultar = 1, Modificar = 1, Eliminar = 0, Activo = 1 },
                new Permisosrol { SecRol = 2, Consultar = 1, Modificar = 0, Eliminar = 0, Activo = 0 }, // Inactivo
                new Permisosrol { SecRol = 3, Consultar = 0, Modificar = 0, Eliminar = 0, Activo = 1 } // Sin permisos
            }.AsQueryable();

            _mockDbSet.As<IQueryable<Permisosrol>>().Setup(m => m.Provider).Returns(data.Provider);
            _mockDbSet.As<IQueryable<Permisosrol>>().Setup(m => m.Expression).Returns(data.Expression);
            _mockDbSet.As<IQueryable<Permisosrol>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _mockDbSet.As<IQueryable<Permisosrol>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            _mockContext.Setup(c => c.Permisosrols).Returns(_mockDbSet.Object);

            _service = new AutorizacionService(_mockContext.Object);
        }

        [Theory]
        [InlineData(1, "Consultar", true)]
        [InlineData(1, "Modificar", true)]
        [InlineData(1, "Eliminar", false)]
        public async Task TienePermiso_RolActivoConPermiso_DebeDevolverTrue(int secRol, string accion, bool esperado)
        {
            // Act
            var resultado = await _service.TienePermiso(secRol, accion);

            // Assert
            Assert.Equal(esperado, resultado);
        }

        [Fact]
        public async Task TienePermiso_RolInactivo_DebeDevolverFalse()
        {
            // Arrange
            int secRol = 2; // Rol inactivo
            string accion = "Consultar";

            // Act
            var resultado = await _service.TienePermiso(secRol, accion);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public async Task TienePermiso_RolSinPermisos_DebeDevolverFalse()
        {
            // Arrange
            int secRol = 3; // Rol sin permisos
            string accion = "Consultar";

            // Act
            var resultado = await _service.TienePermiso(secRol, accion);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public async Task TienePermiso_RolNoExistente_DebeDevolverFalse()
        {
            // Arrange
            int secRol = 99; // Rol que no existe
            string accion = "Consultar";

            // Act
            var resultado = await _service.TienePermiso(secRol, accion);

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public async Task TienePermiso_AccionNoReconocida_DebeDevolverFalse()
        {
            // Arrange
            int secRol = 1;
            string accion = "AccionDesconocida";

            // Act
            var resultado = await _service.TienePermiso(secRol, accion);

            // Assert
            Assert.False(resultado);
        }
    }
    */
}

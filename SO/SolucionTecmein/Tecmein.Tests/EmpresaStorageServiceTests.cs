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
    public class EmpresaStorageServiceTests
    {
        private readonly Mock<IGenericRepository<Empresastorage>> _mockRepo;
        private readonly EmpresaStorageServices _service;

        public EmpresaStorageServiceTests()
        {
            _mockRepo = new Mock<IGenericRepository<Empresastorage>>();
            _service = new EmpresaStorageServices(_mockRepo.Object);
        }

        private List<Empresastorage> GetTestEmpresaStorage() => new List<Empresastorage>
        {
            new Empresastorage { SecEmpresa = 1, CarpetaLogo = "logos_empresa1", SecEmpresaNavigation = new Empresa { Nombre = "Empresa 1" } },
            new Empresastorage { SecEmpresa = 2, CarpetaLogo = "logos_empresa2", SecEmpresaNavigation = new Empresa { Nombre = "Empresa 2" } }
        };

        [Fact]
        public async Task Consultar_DebeDevolverTodasLasConfiguracionesConNavegacion()
        {
            // Arrange
            var testData = new TestAsyncEnumerable<Empresastorage>(GetTestEmpresaStorage());
            _mockRepo.Setup(repo => repo.Consultar(null)).ReturnsAsync(testData);

            // Act
            var resultado = await _service.Consultar();

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
            Assert.NotNull(resultado.First().SecEmpresaNavigation);
        }

        [Fact]
        public async Task ProcesaGuardar_CrearNuevo_DebeLlamarACrear()
        {
            // Arrange
            var nuevaConfig = new Empresastorage { SecEmpresa = 3, CarpetaLogo = "logos_empresa3" };
            _mockRepo.Setup(r => r.Obtener(It.IsAny<Expression<Func<Empresastorage, bool>>>(), null)).ReturnsAsync((Empresastorage)null);
            _mockRepo.Setup(r => r.Crear(It.IsAny<Empresastorage>())).ReturnsAsync(nuevaConfig);

            // Act
            var resultado = await _service.ProcesaGuardar(nuevaConfig);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(3, resultado.SecEmpresa);
            _mockRepo.Verify(r => r.Crear(It.IsAny<Empresastorage>()), Times.Once);
            _mockRepo.Verify(r => r.Editar(It.IsAny<Empresastorage>()), Times.Never);
        }

        [Fact]
        public async Task ProcesaGuardar_ActualizarExistente_DebeLlamarAEditar()
        {
            // Arrange
            var configExistente = GetTestEmpresaStorage().First();
            var configActualizada = new Empresastorage { SecEmpresa = 1, CarpetaLogo = "logos_actualizados" };
            _mockRepo.Setup(r => r.Obtener(It.IsAny<Expression<Func<Empresastorage, bool>>>(), null)).ReturnsAsync(configExistente);
            _mockRepo.Setup(r => r.Editar(It.IsAny<Empresastorage>())).ReturnsAsync(true);

            // Act
            var resultado = await _service.ProcesaGuardar(configActualizada);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("logos_actualizados", resultado.CarpetaLogo);
            _mockRepo.Verify(r => r.Editar(It.IsAny<Empresastorage>()), Times.Once);
            _mockRepo.Verify(r => r.Crear(It.IsAny<Empresastorage>()), Times.Never);
        }

        [Fact]
        public async Task ProcesaGuardar_FalloAlEditar_DebeLanzarExcepcion()
        {
            // Arrange
            var configExistente = GetTestEmpresaStorage().First();
            var configActualizada = new Empresastorage { SecEmpresa = 1, CarpetaLogo = "logos_actualizados" };
            _mockRepo.Setup(r => r.Obtener(It.IsAny<Expression<Func<Empresastorage, bool>>>(), null)).ReturnsAsync(configExistente);
            _mockRepo.Setup(r => r.Editar(It.IsAny<Empresastorage>())).ReturnsAsync(false); // Simular fallo

            // Act & Assert
            var exception = await Assert.ThrowsAsync<TaskCanceledException>(() => _service.ProcesaGuardar(configActualizada));
            Assert.Equal("No se pudo actualizar la configuración de almacenamiento.", exception.Message);
        }
    }
}
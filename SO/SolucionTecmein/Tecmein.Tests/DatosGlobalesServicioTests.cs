using Xunit;
using BLL.Implementacion;

namespace Tecmein.Tests
{
    public class DatosGlobalesServicioTests
    {
        private readonly DatosGlobalesServicio _service;

        public DatosGlobalesServicioTests()
        {
            _service = new DatosGlobalesServicio();
        }

        [Fact]
        public void PathCatalogos_DebeDevolverValorCorrecto()
        {
            // Arrange
            string expectedPath = "Catalogos_PDF";

            // Act
            string actualPath = _service.PathCatalogos;

            // Assert
            Assert.Equal(expectedPath, actualPath);
        }

        [Fact]
        public void SecuencialEmpresaPrincipal_DebeDevolverValorCorrecto()
        {
            // Arrange
            int expectedSecuencial = 1;

            // Act
            int actualSecuencial = _service.SecuencialEmpresaPrincipal;

            // Assert
            Assert.Equal(expectedSecuencial, actualSecuencial);
        }
    }
}

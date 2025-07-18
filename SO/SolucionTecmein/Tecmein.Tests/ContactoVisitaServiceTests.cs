using Xunit;
using Moq;
using BLL.Implementacion;
using DAL.Interfaces;
using Entity;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BLL.Interfaces;

namespace Tecmein.Tests
{
    public class ContactoVisitaServiceTests
    {
        private readonly Mock<IGenericRepository<Contactovisita>> _mockRepo;
        private readonly Mock<IContactoServices> _mockContactoServices;
        private readonly ContactoVisitaServices _service;

        public ContactoVisitaServiceTests()
        {
            _mockRepo = new Mock<IGenericRepository<Contactovisita>>();
            _mockContactoServices = new Mock<IContactoServices>();
            _service = new ContactoVisitaServices(_mockRepo.Object, _mockContactoServices.Object);
        }

        private List<Contactovisita> GetTestContactoVisitas(List<Contacto> contactos)
        {
            return new List<Contactovisita>
            {
                new Contactovisita { Secuencial = 1, SecVisita = 10, SecContacto = 1, EstaActivo = 1, SecContactoNavigation = contactos.FirstOrDefault(c => c.Secuencial == 1) },
                new Contactovisita { Secuencial = 2, SecVisita = 20, SecContacto = 2, EstaActivo = 1, SecContactoNavigation = contactos.FirstOrDefault(c => c.Secuencial == 2) }
            };
        }

        private List<Contacto> GetTestContactos()
        {
            return new List<Contacto>
            {
                new Contacto { Secuencial = 1, Nombres = "Contacto 1" },
                new Contacto { Secuencial = 2, Nombres = "Contacto 2" },
                new Contacto { Secuencial = 3, Nombres = "Nuevo Contacto" },
                new Contacto { Secuencial = 99, Nombres = "Contacto Actualizado" }
            };
        }

        [Fact]
        public async Task ListaPorContacto_DebeDevolverContactosVisitaFiltrados()
        {
            // Arrange
            var contactos = GetTestContactos();
            var testData = GetTestContactoVisitas(contactos);
            var asyncTestData = new TestAsyncEnumerable<Contactovisita>(testData.Where(c => c.SecContacto == 1));

            _mockRepo.Setup(repo => repo.Consultar(It.IsAny<Expression<Func<Contactovisita, bool>>>()))
                     .ReturnsAsync(asyncTestData);

            // Act
            var resultado = await _service.ListaPorContacto(1);

            // Assert
            Assert.NotNull(resultado);
            Assert.Single(resultado);
            Assert.Equal(1, resultado.First().SecContacto);
            Assert.NotNull(resultado.First().SecContactoNavigation);
        }

        [Fact]
        public async Task ListaContactoVisita_DebeDevolverTodosLosContactosVisitaConContacto()
        {
            // Arrange
            var contactos = GetTestContactos();
            var testData = GetTestContactoVisitas(contactos);
            var asyncTestData = new TestAsyncEnumerable<Contactovisita>(testData);

            _mockRepo.Setup(repo => repo.Consultar(null)).ReturnsAsync(asyncTestData);

            // Act
            var resultado = await _service.ListaContactoVisita();

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
            Assert.True(resultado.All(cv => cv.SecContactoNavigation != null));
        }
    }
}

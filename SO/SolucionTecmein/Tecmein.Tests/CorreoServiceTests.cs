using Xunit;
using Moq;
using BLL.Implementacion;
using DAL.Interfaces;
using Entity;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Net.Mail;
using BLL.Interfaces;
using System;

namespace Tecmein.Tests
{
    public class CorreoServiceTests
    {
        private readonly Mock<IGenericRepository<Empresacorreo>> _mockRepo;
        private readonly Mock<ISmtpClientWrapper> _mockSmtpClientWrapper;
        private readonly CorreoServices _service;

        public CorreoServiceTests()
        {
            _mockRepo = new Mock<IGenericRepository<Empresacorreo>>();
            _mockSmtpClientWrapper = new Mock<ISmtpClientWrapper>();
            _service = new CorreoServices(_mockRepo.Object, _mockSmtpClientWrapper.Object);
        }

        [Fact]
        public async Task EnvioCorreo_DebeEnviarCorreoExitosamenteYDevolverTrue()
        {
            // Arrange
            var empresaCorreo = new Empresacorreo
            {
                SecEmpresa = 1,
                Email = "test@example.com",
                Clave = "password",
                Host = "smtp.example.com",
                Puerto = 587
            };

            _mockRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<Empresacorreo, bool>>>(), It.IsAny<string>()))
                     .ReturnsAsync(empresaCorreo);

            _mockSmtpClientWrapper.Setup(s => s.SendMailAsync(
                It.IsAny<MailMessage>(), It.IsAny<string>(), It.IsAny<int>(), 
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _service.EnvioCorreo("recipient@example.com", "Asunto", "Mensaje");

            // Assert
            Assert.True(resultado);
            _mockSmtpClientWrapper.Verify(s => s.SendMailAsync(
                It.Is<MailMessage>(m => m.To.First().Address == "recipient@example.com"),
                empresaCorreo.Host,
                (int)empresaCorreo.Puerto,
                empresaCorreo.Email,
                empresaCorreo.Clave,
                true),
                Times.Once);
        }

        [Fact]
        public async Task EnvioCorreo_ConfiguracionNoEncontrada_DebeLanzarInvalidOperationException()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<Empresacorreo, bool>>>(), It.IsAny<string>()))
                     .ReturnsAsync((Empresacorreo)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _service.EnvioCorreo("recipient@example.com", "Asunto", "Mensaje"));
            Assert.Equal("Configuración de correo empresarial no encontrada.", exception.Message);
        }

        [Fact]
        public async Task EnvioCorreo_FalloEnSmtpClient_DebePropagarExcepcion()
        {
            // Arrange
            var empresaCorreo = new Empresacorreo
            {
                SecEmpresa = 1, Email = "test@example.com", Clave = "password", 
                Host = "smtp.example.com", Puerto = 587
            };
            var smtpException = new SmtpException("Fallo en SMTP");

            _mockRepo.Setup(repo => repo.Obtener(It.IsAny<Expression<Func<Empresacorreo, bool>>>(), It.IsAny<string>()))
                     .ReturnsAsync(empresaCorreo);

            _mockSmtpClientWrapper.Setup(s => s.SendMailAsync(
                It.IsAny<MailMessage>(), It.IsAny<string>(), It.IsAny<int>(), 
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ThrowsAsync(smtpException);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<SmtpException>(() => 
                _service.EnvioCorreo("recipient@example.com", "Asunto", "Mensaje"));
            Assert.Equal("Fallo en SMTP", exception.Message);
        }
    }
}
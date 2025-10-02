using System;

namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class ConstructoraVM
    {
        public int Secuencial { get; set; }
        public string? Nombre { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? Correo { get; set; }
        public string? Atencion { get; set; }
        public string? Administrador { get; set; }
        public string? TelefonoAdministrador { get; set; }
        public string? CorreoAdministrador { get; set; }
        public short? EstaActivo { get; set; }
    }
}
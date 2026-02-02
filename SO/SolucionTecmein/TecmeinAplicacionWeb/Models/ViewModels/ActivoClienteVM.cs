using System;
using System.ComponentModel.DataAnnotations;

namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class ActivoClienteVM
    {
        public int IdActivoCliente { get; set; }
        public int SecCliente { get; set; }
        public string? NombreCliente { get; set; } // Para mostrar en la UI
        public int? SecEquipo { get; set; }
        public string Descripcion { get; set; } = null!;
        public DateTime FechaInstalacion { get; set; }
        public string? FechaInstalacionString { get; set; } // Para manejo en UI
        public int? SecContratoOrigen { get; set; }
        public string? ContratoOrigenNumero { get; set; } // Para mostrar en la UI
    }
}

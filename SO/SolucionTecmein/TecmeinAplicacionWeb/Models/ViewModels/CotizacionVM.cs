using System.Collections.Generic;

namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class CotizacionVM
    {
        public int Secuencial { get; set; }
        public int SecVisita { get; set; }
        public string NombreObra { get; set; } // Para mostrar en la lista
        public string NombreContacto { get; set; } // Para mostrar en la lista
        public bool EnviadoProveedor { get; set; }
        public bool EnviadoCliente { get; set; }
        public bool Confirmacion { get; set; }
        public decimal Subtotal { get; set; }
        public decimal ValorImpuestos { get; set; }
        public decimal TotalConImpuestos { get; set; }
        public decimal ValorIVA { get; set; }
        public decimal ValorImportacion { get; set; }
        public int EstaActivo { get; set; }
        public string FechaRegistro { get; set; }
        public string FechaModificacion { get; set; }
        public int? SecUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public int? SecCotizacionOriginal { get; set; }
        public int? SecUsuarioModifica { get; set; }
        public string NombreUsuarioModifica { get; set; }
        public List<CotizaciondetalleVM> Cotizaciondetalles { get; set; }
        public List<ImpuestoCotizacionVM> ImpuestoCotizaciones { get; set; }
    }
}

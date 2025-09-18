using System;
using System.Collections.Generic;

namespace TecmeinWebApp.Models.ViewModel
{
    public class TipoDocumentoVM
    {
        public int SecTipoDocumento { get; set; }
        public string? Codigo { get; set; }
        public string? Descripcion { get; set; }
        public bool? EstaActivo { get; set; }
        public string? FechaRegistro { get; set; } // Changed to string for display purposes
    }
}
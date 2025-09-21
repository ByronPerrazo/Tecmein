using System;
using System.Collections.Generic;

namespace TecmeinAplicacionWeb.Models.ViewModels
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
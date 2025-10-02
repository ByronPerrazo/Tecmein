using System;
using System.IO;

namespace BLL.DTOs
{
    public class ContratoCreacionDTO
    {
        // Identificadores para ambos flujos
        public int? IdCotizacion { get; set; } // Para flujo desde Pre-Contrato
        public int? SecCliente { get; set; }   // Para flujo Directo/Histórico

        // Datos comunes del Contrato
        public string NombreProyecto { get; set; } // Añadido para el nombre de la obra
        public DateTime FechaFirma { get; set; }
        public int IdUsuarioCarga { get; set; }

        // Archivo
        public Stream ArchivoStream { get; set; }
        public string NombreArchivo { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace BLL.DTOs
{
    public class PreContratoConPagosDTO : PreContratoGeneratorDTO
    {
        public int SecPreContrato { get; set; } // Añadido para identificar el pre-contrato a actualizar
        public string ContenidoHtml { get; set; }
        public List<CompromisoPagoDTO> CompromisosDePago { get; set; }

        public PreContratoConPagosDTO()
        {
            CompromisosDePago = new List<CompromisoPagoDTO>();
        }
    }
}

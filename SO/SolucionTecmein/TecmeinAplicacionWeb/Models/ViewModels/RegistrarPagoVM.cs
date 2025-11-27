using Microsoft.AspNetCore.Http; // Added for IFormFile

namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class RegistrarPagoVM
    {
        public int IdPlanDePago { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public IFormFile? ComprobanteFile { get; set; } // Added for file upload
        public string? ComprobanteUrl { get; set; }
        public string? ComprobanteNombre { get; set; }
        public int RegistradoPorUsuarioId { get; set; } // Se asignará desde el controlador/claims
    }
}

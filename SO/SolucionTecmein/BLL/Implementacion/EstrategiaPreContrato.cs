using BLL.Interfaces;
using Entity;

namespace BLL.Implementacion
{
    public class EstrategiaPreContrato : IEstrategiaGeneradorDocumento
    {
        public string CodigoTipoDocumento => "PRECONTRATO";

        private readonly IPreContratoGeneratorService _preContratoGeneratorService;

        public EstrategiaPreContrato(IPreContratoGeneratorService preContratoGeneratorService)
        {
            _preContratoGeneratorService = preContratoGeneratorService;
        }

        public async Task<byte[]> Generar(int secPlantilla, object datos)
        {
            // Mapeamos el objeto 'datos' anónimo/dinámico a PreContratoConPagosDTO si es posible,
            // o lo tratamos como un objeto de datos genérico.
            // Para mantener la compatibilidad con el GeneratorService, creamos el DTO necesario.

            var preContratoData = new BLL.DTOs.PreContratoConPagosDTO();

            // Intentamos extraer las propiedades por reflexión de 'datos'
            var type = datos.GetType();

            preContratoData.SecCotizacion = (int?)type.GetProperty("SecCotizacion")?.GetValue(datos) ?? 0;
            preContratoData.SecTipoDocumento = (int?)type.GetProperty("SecTipoDocumento")?.GetValue(datos) ?? 0;
            preContratoData.Dias = (int?)type.GetProperty("Dias")?.GetValue(datos) ?? (int?)type.GetProperty("DiasDeEntrega")?.GetValue(datos) ?? 0;
            preContratoData.TipoDias = type.GetProperty("TipoDias")?.GetValue(datos)?.ToString();
            preContratoData.PeriodoMantenimiento = type.GetProperty("PeriodoMantenimiento")?.GetValue(datos)?.ToString();
            preContratoData.AniosGarantia = (int?)type.GetProperty("AniosGarantia")?.GetValue(datos) ?? 0;
            preContratoData.MesesGarantia = (int?)type.GetProperty("MesesGarantia")?.GetValue(datos) ?? 0;
            preContratoData.PolizaGarantia = type.GetProperty("PolizaGarantia")?.GetValue(datos)?.ToString();

            var compromisos = type.GetProperty("CompromisosDePago")?.GetValue(datos) as IEnumerable<PreContratoCompromisoPago>;
            if (compromisos != null)
            {
                preContratoData.CompromisosDePago = compromisos.Select(c => new BLL.DTOs.CompromisoPagoDTO
                {
                    Tipo = c.Tipo,
                    Monto = c.Monto,
                    FechaVencimiento = c.FechaVencimiento
                }).ToList();
            }

            // Delegamos la generación al motor unificado
            return await _preContratoGeneratorService.GenerarVistaPreviaDocx(preContratoData);
        }
    }

}

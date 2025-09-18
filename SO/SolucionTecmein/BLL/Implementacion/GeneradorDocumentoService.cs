using BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Implementacion
{
    public class GeneradorDocumentoService : IGeneradorDocumentoService
    {
        private readonly IEnumerable<IEstrategiaGeneradorDocumento> _estrategias;

        public GeneradorDocumentoService(IEnumerable<IEstrategiaGeneradorDocumento> estrategias)
        {
            _estrategias = estrategias;
        }

        public async Task<byte[]> GenerarDocumento(string codigoTipoDocumento, int secPlantilla, object datos)
        {
            var estrategia = _estrategias.FirstOrDefault(e => e.CodigoTipoDocumento.Equals(codigoTipoDocumento, StringComparison.OrdinalIgnoreCase));

            if (estrategia == null)
            {
                throw new NotImplementedException($"No se encontró una estrategia de generación para el tipo de documento '{codigoTipoDocumento}'.");
            }

            return await estrategia.Generar(secPlantilla, datos);
        }
    }
}

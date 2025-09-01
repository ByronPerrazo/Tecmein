using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Implementacion
{
    public class PreContratoServices : IPreContratoServices
    {
        private readonly IGenericRepository<PreContrato> _repositorio;
        private readonly ICotizacionServices _cotizacionServices;
        private readonly IGenericRepository<PlantillaPreContratoParrafo> _repositorioParrafo;

        public PreContratoServices(IGenericRepository<PreContrato> repositorio, ICotizacionServices cotizacionServices, IGenericRepository<PlantillaPreContratoParrafo> repositorioParrafo)
        {
            _repositorio = repositorio;
            _cotizacionServices = cotizacionServices;
            _repositorioParrafo = repositorioParrafo;
        }

        public async Task<List<PreContrato>> Lista()
        {
            IQueryable<PreContrato> query = await _repositorio.Consultar();
            return query.ToList();
        }

        public async Task<PreContrato> Obtener(int secPreContrato)
        {
            return await _repositorio.Obtener(p => p.SecPreContrato == secPreContrato);
        }

        public async Task<PreContrato> Crear(PreContrato entidad)
        {
            if (entidad == null) throw new ArgumentNullException(nameof(entidad));

            var cotizacion = await _cotizacionServices.Detalle(entidad.SecCotizacion);
            if (cotizacion == null)
            {
                throw new Exception("La cotización especificada no existe.");
            }

            entidad.FechaRegistro = DateTime.Now;
            var preContratoCreado = await _repositorio.Crear(entidad);
            return preContratoCreado;
        }

        public async Task<PreContrato> Editar(PreContrato entidad)
        {
            if (entidad == null) throw new ArgumentNullException(nameof(entidad));

            var preContratoExistente = await _repositorio.Obtener(p => p.SecPreContrato == entidad.SecPreContrato);
            if (preContratoExistente == null)
            {
                throw new Exception("El pre-contrato no existe.");
            }

            preContratoExistente.Dias = entidad.Dias;
            preContratoExistente.TipoDias = entidad.TipoDias;
            preContratoExistente.ValorContrato = entidad.ValorContrato;
            preContratoExistente.AniosGarantia = entidad.AniosGarantia;
            preContratoExistente.MesesGarantia = entidad.MesesGarantia;
            preContratoExistente.PeriodoMantenimiento = entidad.PeriodoMantenimiento;
            preContratoExistente.PolizaGarantia = entidad.PolizaGarantia;
            preContratoExistente.ValorAnticipo = entidad.ValorAnticipo;
            preContratoExistente.FechaAnticipo = entidad.FechaAnticipo;
            preContratoExistente.FormaPago = entidad.FormaPago;
            preContratoExistente.NumeroCuotas = entidad.NumeroCuotas;
            preContratoExistente.FechaPrimeraCuota = entidad.FechaPrimeraCuota;
            preContratoExistente.EstaActivo = entidad.EstaActivo;

            await _repositorio.Editar(preContratoExistente);
            return preContratoExistente;
        }

        public async Task<bool> Eliminar(int secPreContrato)
        {
            var preContrato = await _repositorio.Obtener(p => p.SecPreContrato == secPreContrato);
            if (preContrato == null)
            {
                throw new Exception("El pre-contrato no existe.");
            }

            return await _repositorio.Eliminar(preContrato);
        }

        public async Task<string> GenerarDocumentoWord(int secPreContrato)
        {
            var preContrato = await _repositorio.Obtener(p => p.SecPreContrato == secPreContrato);
            if (preContrato == null)
            {
                throw new Exception("Pre-contrato no encontrado.");
            }

            // Lógica para obtener los párrafos de la plantilla
            var parrafos = (await _repositorioParrafo.Consultar(p => p.SecPlantillaPreContrato == preContrato.SecCotizacion && p.EstaActivo == true)).OrderBy(p => p.Orden).ToList();

            // TODO: Implementar la lógica real de generación de documento Word
            // Esto implicaría usar una librería como DocX, Open XML SDK, o similar.
            // Por ahora, retornamos un placeholder.
            return "Documento Word generado (placeholder).";
        }
    }
}

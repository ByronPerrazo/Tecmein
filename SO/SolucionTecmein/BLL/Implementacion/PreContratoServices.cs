using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.DTOs;

namespace BLL.Implementacion
{
    public class PreContratoServices : IPreContratoServices
    {
        private readonly IGenericRepository<PreContrato> _repositorio;
        private readonly ICotizacionServices _cotizacionServices;
        private readonly IGenericRepository<PreContratoParrafo> _repositorioPreContratoParrafo;
        private readonly IPreContratoGeneratorService _preContratoGeneratorService;

        public PreContratoServices(
            IGenericRepository<PreContrato> repositorio,
            ICotizacionServices cotizacionServices,
            IGenericRepository<PreContratoParrafo> repositorioPreContratoParrafo,
            IPreContratoGeneratorService preContratoGeneratorService)
        {
            _repositorio = repositorio;
            _cotizacionServices = cotizacionServices;
            _repositorioPreContratoParrafo = repositorioPreContratoParrafo;
            _preContratoGeneratorService = preContratoGeneratorService;
        }

        public async Task<List<PreContrato>> Lista()
        {
            IQueryable<PreContrato> query = await _repositorio.Consultar(p => p.EstaActivo == true);
            return await query.Include(p => p.SecCotizacionNavigation)
                              .ThenInclude(c => c.SecVisitaNavigation)
                              .Include(p => p.SecUsuarioCreaNavigation)
                              .ToListAsync();
        }

        public async Task<PreContrato> Obtener(int secPreContrato)
        {
            IQueryable<PreContrato> query = await _repositorio.Consultar();
            var preContrato = await query.Include(p => p.SecCotizacionNavigation)
                                             .ThenInclude(c => c.SecVisitaNavigation)
                                             .ThenInclude(v => v.SecEmpresaNavigation)
                                           .Include(p => p.SecUsuarioCreaNavigation)
                                           .Include(p => p.SecFormaPagoNavigation)
                                           .Include(p => p.SecPlantillaPreContratoNavigation)
                                           .FirstOrDefaultAsync(p => p.SecPreContrato == secPreContrato);
            return preContrato;
        }

        public async Task<string> ObtenerContenidoPrevisualizado(int secPreContrato)
        {
            var preContrato = await Obtener(secPreContrato);
            if (preContrato == null)
            {
                throw new Exception("Pre-contrato no encontrado para previsualización.");
            }

            var dto = new PreContratoGeneratorDTO
            {
                SecCotizacion = preContrato.SecCotizacion,
                SecFormaPago = preContrato.SecFormaPago,
                SecPlantillaPreContrato = preContrato.SecPlantillaPreContrato,
                ValorContrato = preContrato.ValorContrato,
                ValorAnticipo = preContrato.ValorAnticipo,
                FechaAnticipo = preContrato.FechaAnticipo,
                NumeroCuotas = preContrato.NumeroCuotas,
                FechaPrimeraCuota = preContrato.FechaPrimeraCuota,
                Dias = preContrato.Dias,
                TipoDias = preContrato.TipoDias,
                PeriodoMantenimiento = preContrato.PeriodoMantenimiento,
                AniosGarantia = preContrato.AniosGarantia,
                MesesGarantia = preContrato.MesesGarantia,
                PolizaGarantia = preContrato.PolizaGarantia
            };

            return await _preContratoGeneratorService.GenerarVistaPreviaHtml(dto);
        }

        public async Task<PreContrato> Crear(PreContrato entidad)
        {
            if (entidad == null) throw new ArgumentNullException(nameof(entidad));

            ValidatePreContratoFields(entidad);

            var cotizacion = await _cotizacionServices.Detalle(entidad.SecCotizacion);
            if (cotizacion == null)
            {
                throw new Exception("La cotización especificada no existe.");
            }

            entidad.FechaRegistro = DateTime.Now;
            entidad.EstaActivo = true;
            entidad.Version = 1;
            entidad.Estado = "Borrador";

            var preContratoCreado = await _repositorio.Crear(entidad);
            if (preContratoCreado.SecPreContrato == 0)
            {
                throw new Exception("No se pudo crear el pre-contrato.");
            }
            
            return preContratoCreado;
        }

        public async Task<PreContrato> Editar(PreContrato entidad)
        {
            if (entidad == null) throw new ArgumentNullException(nameof(entidad));

            ValidatePreContratoFields(entidad);

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
            preContratoExistente.SecFormaPago = entidad.SecFormaPago;
            preContratoExistente.NumeroCuotas = entidad.NumeroCuotas;
            preContratoExistente.FechaPrimeraCuota = entidad.FechaPrimeraCuota;
            preContratoExistente.EstaActivo = entidad.EstaActivo;
            preContratoExistente.SecPlantillaPreContrato = entidad.SecPlantillaPreContrato;


            await _repositorio.Editar(preContratoExistente);
            return preContratoExistente;
        }

        private void ValidatePreContratoFields(PreContrato entidad)
        {
            if (entidad.Dias < 0)
            {
                throw new InvalidOperationException("El número de días no puede ser negativo.");
            }
            if (entidad.MesesGarantia < 0)
            {
                throw new InvalidOperationException("El número de meses de garantía no puede ser negativo.");
            }
            if (entidad.AniosGarantia < 0)
            {
                throw new InvalidOperationException("El número de años de garantía no puede ser negativo.");
            }
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
        
        public async Task<PreContrato> ObtenerUltimaVersion(int secCotizacion)
        {
            var preContratos = await _repositorio.Consultar(p => p.SecCotizacion == secCotizacion);
            return await preContratos.OrderByDescending(p => p.Version).FirstOrDefaultAsync();
        }

        public async Task<PreContrato> CrearDesdeCotizacion(int cotizacionId, int secUsuario)
        {
            var preContratoExistente = await _repositorio.Obtener(p => p.SecCotizacion == cotizacionId && p.EstaActivo == true);
            if (preContratoExistente != null)
            {
                return preContratoExistente;
            }

            var cotizacion = await _cotizacionServices.Detalle(cotizacionId);
            if (cotizacion == null)
            {
                throw new Exception("La cotización especificada no existe.");
            }

            var nuevoPreContrato = new PreContrato
            {
                SecCotizacion = cotizacionId,
                SecUsuarioCrea = secUsuario,
                Version = 1,
                Estado = "Borrador",
                EstaActivo = true,
                FechaRegistro = DateTime.Now,
                Dias = 0,
                TipoDias = "N/A",
                ValorContrato = (decimal)(cotizacion.Subtotal + cotizacion.ValorImpuestos),
                AniosGarantia = 0,
                MesesGarantia = 0,
                PeriodoMantenimiento = "N/A",
                PolizaGarantia = "N/A",
                ValorAnticipo = 0,
                NumeroCuotas = 0
            };

            var preContratoCreado = await _repositorio.Crear(nuevoPreContrato);
            return preContratoCreado;
        }
        
        public async Task<PreContrato> GuardarDesdeEditor(int cotizacionId, string contenidoHtml, int usuarioId)
        {
            var ultimaVersion = await ObtenerUltimaVersion(cotizacionId);

            // Si existe una versión anterior, comparamos el contenido
            if (ultimaVersion != null)
            {
                var parrafoActual = await ObtenerPrimerParrafo(ultimaVersion.SecPreContrato);
                var contenidoActual = parrafoActual?.Contenido ?? "";

                // Si el contenido no ha cambiado, no hacemos nada y devolvemos la versión existente para evitar data redundante.
                if (string.Equals(contenidoActual, contenidoHtml, StringComparison.Ordinal))
                {
                    return ultimaVersion;
                }

                // Si el contenido cambió, desactivamos la versión anterior.
                ultimaVersion.EstaActivo = false;
                await _repositorio.Editar(ultimaVersion);
            }

            // Creamos el nuevo pre-contrato con la nueva versión, copiando los datos de la anterior.
            var nuevoPreContrato = new PreContrato
            {
                SecCotizacion = cotizacionId,
                SecUsuarioCrea = usuarioId,
                Version = (ultimaVersion?.Version ?? 0) + 1,
                Estado = "Guardado",
                EstaActivo = true,
                FechaRegistro = DateTime.Now,
                
                // Copiamos los datos de la versión anterior para no perderlos en la nueva versión.
                SecPlantillaPreContrato = ultimaVersion?.SecPlantillaPreContrato ?? 0,
                SecFormaPago = ultimaVersion?.SecFormaPago,
                ValorContrato = ultimaVersion?.ValorContrato ?? 0,
                ValorAnticipo = ultimaVersion?.ValorAnticipo ?? 0,
                FechaAnticipo = ultimaVersion?.FechaAnticipo,
                NumeroCuotas = ultimaVersion?.NumeroCuotas ?? 0,
                FechaPrimeraCuota = ultimaVersion?.FechaPrimeraCuota,
                Dias = ultimaVersion?.Dias ?? 0,
                TipoDias = ultimaVersion?.TipoDias,
                PeriodoMantenimiento = ultimaVersion?.PeriodoMantenimiento,
                AniosGarantia = ultimaVersion?.AniosGarantia ?? 0,
                MesesGarantia = ultimaVersion?.MesesGarantia ?? 0,
                PolizaGarantia = ultimaVersion?.PolizaGarantia
            };

            var preContratoCreado = await _repositorio.Crear(nuevoPreContrato);

            // Creamos el nuevo párrafo con el contenido actualizado
            var nuevoParrafo = new PreContratoParrafo
            {
                SecPreContrato = preContratoCreado.SecPreContrato,
                Contenido = contenidoHtml,
                Orden = 1
            };
            await _repositorioPreContratoParrafo.Crear(nuevoParrafo);

            return preContratoCreado;
        }

        public async Task<List<PreContrato>> ObtenerHistorial(int secPreContrato)
        {
            var preContratoActual = await _repositorio.Obtener(p => p.SecPreContrato == secPreContrato);
            if (preContratoActual == null)
            {
                return new List<PreContrato>();
            }

            var query = await _repositorio.Consultar(p => p.SecCotizacion == preContratoActual.SecCotizacion);

            return await query.Include(p => p.SecUsuarioCreaNavigation)
                                .OrderByDescending(p => p.Version)
                                .ToListAsync();
        }

        public async Task<PreContratoParrafo> ObtenerPrimerParrafo(int secPreContrato)
        {
            var query = await _repositorioPreContratoParrafo.Consultar(p => p.SecPreContrato == secPreContrato);
            return await query.OrderBy(p => p.Orden).FirstOrDefaultAsync();
        }

        public async Task<PreContrato> CrearDesdeModal(PreContrato entidad, int usuarioId)
        {
            var ultimaVersion = await ObtenerUltimaVersion(entidad.SecCotizacion);
            if (ultimaVersion != null)
            {
                ultimaVersion.EstaActivo = false;
                await _repositorio.Editar(ultimaVersion);
            }

            entidad.SecUsuarioCrea = usuarioId;
            entidad.Version = (ultimaVersion?.Version ?? 0) + 1;
            entidad.Estado = "Borrador";
            entidad.EstaActivo = true;
            entidad.FechaRegistro = DateTime.Now;

            var preContratoCreado = await _repositorio.Crear(entidad);
            if (preContratoCreado == null || preContratoCreado.SecPreContrato == 0)
            {
                throw new Exception("No se pudo crear el pre-contrato desde el modal.");
            }

            // --- INICIO DE LA LÓGICA AÑADIDA ---
            var dto = new PreContratoGeneratorDTO
            {
                SecCotizacion = preContratoCreado.SecCotizacion,
                SecFormaPago = preContratoCreado.SecFormaPago,
                SecPlantillaPreContrato = preContratoCreado.SecPlantillaPreContrato,
                ValorContrato = preContratoCreado.ValorContrato,
                ValorAnticipo = preContratoCreado.ValorAnticipo,
                FechaAnticipo = preContratoCreado.FechaAnticipo,
                NumeroCuotas = preContratoCreado.NumeroCuotas,
                FechaPrimeraCuota = preContratoCreado.FechaPrimeraCuota,
                Dias = preContratoCreado.Dias,
                TipoDias = preContratoCreado.TipoDias,
                PeriodoMantenimiento = preContratoCreado.PeriodoMantenimiento,
                AniosGarantia = preContratoCreado.AniosGarantia,
                MesesGarantia = preContratoCreado.MesesGarantia,
                PolizaGarantia = preContratoCreado.PolizaGarantia
            };

            string contenidoHtml = await _preContratoGeneratorService.GenerarVistaPreviaHtml(dto);

            var nuevoParrafo = new PreContratoParrafo
            {
                SecPreContrato = preContratoCreado.SecPreContrato,
                Contenido = contenidoHtml,
                Orden = 1
            };
            await _repositorioPreContratoParrafo.Crear(nuevoParrafo);
            // --- FIN DE LA LÓGICA AÑADIDA ---

            return preContratoCreado;
        }

        public Task<string> GenerarDocumentoWord(int secPreContrato)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ActualizarContenido(int secPreContrato, string contenidoHtml)
        {
            throw new NotImplementedException();
        }
    }
}
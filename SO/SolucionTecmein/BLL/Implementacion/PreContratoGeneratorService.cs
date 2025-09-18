using BLL.DTOs;
using BLL.Interfaces;
using DAL.DBContext;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Implementacion
{
    public class PreContratoGeneratorService : IPreContratoGeneratorService
    {
        private readonly TecmeindbContext _context;
        private readonly IGenericRepository<DiccionarioParametro> _repositorioDiccionario;

        public PreContratoGeneratorService(TecmeindbContext context, IGenericRepository<DiccionarioParametro> repositorioDiccionario)
        {
            _context = context;
            _repositorioDiccionario = repositorioDiccionario;
        }

        public async Task<string> GenerarVistaPreviaHtml(PreContratoGeneratorDTO preContratoData)
        {
            var plantilla = await _context.PlantillaPreContratos
                                          .Include(p => p.PlantillaPreContratoParrafos)
                                          .AsNoTracking()
                                          .FirstOrDefaultAsync(p => p.SecPlantillaPreContrato == preContratoData.SecPlantillaPreContrato);

            if (plantilla == null || !plantilla.PlantillaPreContratoParrafos.Any())
            {
                throw new InvalidOperationException("La plantilla seleccionada no tiene contenido o no existe.");
            }
            
            var contenidoOriginal = string.Join("", plantilla.PlantillaPreContratoParrafos.OrderBy(p => p.Orden).Select(p => p.Contenido));
            var datosParaReemplazar = await RecopilarDatosDeReemplazo(preContratoData);
            
            var contenidoProcesado = new StringBuilder(contenidoOriginal);
            foreach (var kvp in datosParaReemplazar)
            {
                contenidoProcesado.Replace(kvp.Key, kvp.Value);
            }

            return contenidoProcesado.ToString();
        }

        public async Task<PlaceholderDataDTO> ObtenerDatosParaPlaceholders(PreContratoGeneratorDTO preContratoData)
        {
            var datosReemplazo = await RecopilarDatosDeReemplazo(preContratoData);
            var cotizacionData = await _context.Cotizacion.FindAsync(preContratoData.SecCotizacion);

            var dto = new PlaceholderDataDTO
            {
                ValorContrato = datosReemplazo.GetValueOrDefault("{{valor_contrato}}", ""),
                ValorAnticipo = datosReemplazo.GetValueOrDefault("{{valor_anticipo}}", ""),
                FechaAnticipo = datosReemplazo.GetValueOrDefault("{{fecha_anticipo}}", ""),
                NumeroCuotas = datosReemplazo.GetValueOrDefault("{{numero_cuotas}}", ""),
                FechaPrimeraCuota = datosReemplazo.GetValueOrDefault("{{fecha_primera_cuota}}", ""),
                DiasEntrega = datosReemplazo.GetValueOrDefault("{{dias}}", ""),
                TipoDias = datosReemplazo.GetValueOrDefault("{{tipo_dias}}", ""),
                PeriodoMantenimiento = datosReemplazo.GetValueOrDefault("{{periodo_mantenimiento}}", ""),
                AniosGarantia = datosReemplazo.GetValueOrDefault("{{anios_garantia}}", ""),
                MesesGarantia = datosReemplazo.GetValueOrDefault("{{meses_garantia}}", ""),
                PolizaGarantia = datosReemplazo.GetValueOrDefault("{{poliza_garantia}}", ""),
                Cotizacion = new CotizacionPlaceholderDTO
                {
                    Numero = cotizacionData?.Secuencial.ToString() ?? "",
                    Fecha = cotizacionData?.FechaRegistro?.ToString("dd/MM/yyyy") ?? "",
                    Total = (cotizacionData?.TotalConImpuestos)?.ToString("N2") ?? ""
                },
                Cliente = new ClientePlaceholderDTO
                {
                    Nombre = datosReemplazo.GetValueOrDefault("{{cliente_nombre}}", ""),
                    Direccion = datosReemplazo.GetValueOrDefault("{{cliente_direccion}}", ""),
                    Telefono = datosReemplazo.GetValueOrDefault("{{cliente_telefono}}", ""),
                    Correo = datosReemplazo.GetValueOrDefault("{{cliente_correo}}", ""),
                    Administrador = datosReemplazo.GetValueOrDefault("{{cliente_representante_legal}}", "")
                }
            };

            return dto;
        }

        private async Task<Dictionary<string, string>> RecopilarDatosDeReemplazo(PreContratoGeneratorDTO preContratoData)
        {
            if (preContratoData.SecCotizacion == 0)
            {
                throw new ArgumentException("Se debe seleccionar una cotización.");
            }

            var cotizacion = await _context.Cotizacion
                .Include(c => c.SecVisitaNavigation).ThenInclude(v => v.SecEmpresaNavigation)
                .Include(c => c.SecVisitaNavigation).ThenInclude(v => v.Contactovisita).ThenInclude(cv => cv.SecContactoNavigation).ThenInclude(con => con.SecConstructoraNavigation).ThenInclude(cs => cs.Cliente)
                .Include(c => c.SecUsuarioNavigation)
                .Include(c => c.Cotizaciondetalles)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Secuencial == preContratoData.SecCotizacion) ?? throw new InvalidOperationException("No se encontraron datos para la cotización seleccionada.");

            var visita = cotizacion.SecVisitaNavigation;
            var empresa = visita?.SecEmpresaNavigation;
            var contactoVisita = visita?.Contactovisita?.FirstOrDefault()?.SecContactoNavigation;
            var constructora = contactoVisita?.SecConstructoraNavigation;
            var usuarioCreaCotizacion = cotizacion.SecUsuarioNavigation;
            var formaPago = preContratoData.SecFormaPago.HasValue ? await _context.FormasPago.FindAsync(preContratoData.SecFormaPago.Value) : null;

            var diccionario = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var parametros = await (await _repositorioDiccionario.Consultar(p => p.EstaActivo)).ToListAsync();

            void AddToDict(string key, string value)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    diccionario[key] = value;
                }
            }

            foreach (var parametro in parametros)
            {
                string valor = "";
                switch (parametro.Parametro.ToLower())
                {
                    case "{{valor_contrato}}": valor = preContratoData.ValorContrato.ToString("N2"); break;
                    case "{{valor_anticipo}}": valor = preContratoData.ValorAnticipo.ToString("N2"); break;
                    case "{{fecha_anticipo}}": valor = preContratoData.FechaAnticipo?.ToString("dd/MM/yyyy"); break;
                    case "{{numero_cuotas}}": valor = preContratoData.NumeroCuotas.ToString(); break;
                    case "{{fecha_primera_cuota}}": valor = preContratoData.FechaPrimeraCuota?.ToString("dd/MM/yyyy"); break;
                    case "{{dias}}": valor = preContratoData.Dias.ToString(); break;
                    case "{{tipo_dias}}": valor = preContratoData.TipoDias; break;
                    case "{{periodo_mantenimiento}}": valor = preContratoData.PeriodoMantenimiento; break;
                    case "{{anios_garantia}}": valor = preContratoData.AniosGarantia.ToString(); break;
                    case "{{meses_garantia}}": valor = preContratoData.MesesGarantia.ToString(); break;
                    case "{{poliza_garantia}}": valor = preContratoData.PolizaGarantia; break;
                    case "{{forma_pago}}": valor = formaPago?.Descripcion; break;
                    case "{{cotizacion_numero}}": valor = cotizacion.Secuencial.ToString(); break;
                    case "{{cotizacion_fecha}}": valor = cotizacion.FechaRegistro?.ToString("dd/MM/yyyy"); break;
                    case "{{cotizacion_subtotal}}": valor = cotizacion.Subtotal.ToString("N2"); break;
                    case "{{cotizacion_impuestos}}": valor = cotizacion.ValorImpuestos.ToString("N2"); break;
                    case "{{cotizacion_total}}": valor = cotizacion.TotalConImpuestos.ToString("N2"); break;
                    case "{{cotizacion_creado_por}}": valor = usuarioCreaCotizacion?.Nombre; break;
                    case "{{proyecto_nombre}}": valor = visita?.Nombre; break;
                    case "{{visita_nombre_obra}}": valor = visita?.Nombre; break;
                    case "{{visita_fecha}}": valor = visita?.FechaRegistro?.ToString("dd/MM/yyyy"); break;
                    case "{{empresa_nombre}}": valor = empresa?.Nombre; break;
                    case "{{empresa_identificacion}}": valor = empresa?.Identificacion; break;
                    case "{{empresa_direccion}}": valor = empresa?.Direccion; break;
                    case "{{empresa_telefono}}": valor = empresa?.Telefono; break;
                    case "{{nombres contrato}}": valor = constructora?.Nombre; break;
                    case "{{cliente_nombre}}": valor = constructora?.Nombre; break;
                    case "{{cliente_numero}}": valor = constructora?.Cliente?.NumeroCliente; break;
                    case "{{cliente_direccion}}": valor = constructora?.Direccion; break;
                    case "{{cliente_telefono}}": valor = constructora?.Telefono; break;
                    case "{{cliente_correo}}": valor = constructora?.Correo; break;
                    case "{{cliente_representante_legal}}": valor = constructora?.Administrador; break;
                    case "{{contacto_nombre}}": valor = (contactoVisita?.Nombres + " " + contactoVisita?.Apellidos).Trim(); break;
                    case "{{contacto_cargo}}": valor = contactoVisita?.Titulo; break;
                    case "{{contacto_telefono}}": valor = contactoVisita?.Telefono; break;
                    case "{{contacto_correo}}": valor = contactoVisita?.Correo; break;
                    case "{{tabla_detalle_cotizacion}}":
                        var tablaDetalleHtml = new StringBuilder();
                        tablaDetalleHtml.Append("<table border='1' style='width:100%; border-collapse: collapse;'>");
                        tablaDetalleHtml.Append("<tr><th style='padding: 8px; text-align: left;'>Detalle</th><th style='padding: 8px; text-align: right;'>Total</th></tr>");
                        if (cotizacion.Cotizaciondetalles != null && cotizacion.Cotizaciondetalles.Any())
                        {
                            foreach (var item in cotizacion.Cotizaciondetalles)
                            {
                                tablaDetalleHtml.AppendFormat("<tr><td style='padding: 8px;'>{0}</td><td style='padding: 8px; text-align: right;'>{1:N2}</td></tr>", item.DetalleEquipo, item.Total);
                            }
                        }
                        else
                        {
                            tablaDetalleHtml.Append("<tr><td colspan='2' style='padding: 8px; text-align: center;'>No hay detalles disponibles.</td></tr>");
                        }
                        tablaDetalleHtml.Append("</table>");
                        valor = tablaDetalleHtml.ToString();
                        break;
                    case "{{fecha_actual}}": valor = DateTime.Now.ToString("dd/MM/yyyy"); break;
                    case "{{fecha_actual_larga}}": valor = DateTime.Now.ToLongDateString(); break;
                }
                AddToDict(parametro.Parametro, valor);
            }

            return diccionario;
        }

        // El método de depuración ya no es necesario, se elimina.
    }
}

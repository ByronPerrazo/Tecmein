using BLL.DTOs;
using BLL.Interfaces;
using DAL.DBContext;
using DAL.Interfaces;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Entity;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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

        public async Task<byte[]> GenerarVistaPreviaDocx(PreContratoGeneratorDTO preContratoData)
        {
            // 1. Obtener TipoDocumento para ver si tiene plantilla explícita
            var tipoDocumento = await _context.TipoDocumentos.FindAsync(preContratoData.SecTipoDocumento);

            PlantillaPreContrato? plantilla = null;

            if (tipoDocumento != null && tipoDocumento.SecPlantilla.HasValue)
            {
                // Estrategia Prioritaria: Cargar la plantilla vinculada explícitamente
                plantilla = await _context.PlantillaPreContratos
                                          .Include(p => p.PlantillaPreContratoParrafos)
                                          .AsNoTracking()
                                          .FirstOrDefaultAsync(p => p.SecPlantillaPreContrato == tipoDocumento.SecPlantilla && p.EstaActivo == 1);
            }

            if (plantilla == null)
            {
                // Estrategia Fallback: Buscar plantilla por relación inversa (legacy)
                plantilla = await _context.PlantillaPreContratos
                                          .Include(p => p.PlantillaPreContratoParrafos)
                                          .AsNoTracking()
                                          .FirstOrDefaultAsync(p => p.SecTipoDocumento == preContratoData.SecTipoDocumento && p.EstaActivo == 1);
            }

            if (plantilla == null || !plantilla.PlantillaPreContratoParrafos.Any())
            {
                throw new InvalidOperationException("La plantilla seleccionada no tiene contenido o no existe.");
            }

            var parrafoPlantilla = plantilla.PlantillaPreContratoParrafos.OrderBy(p => p.Orden).FirstOrDefault();
            if (parrafoPlantilla?.Contenido == null)
            {
                throw new InvalidOperationException("El párrafo de la plantilla no contiene un documento DOCX.");
            }

            var (datosTexto, datosTabla) = await RecopilarDatosDeReemplazo(preContratoData, parrafoPlantilla.Contenido);
            
            byte[] docBytes = parrafoPlantilla.Contenido;

            using (var ms = new MemoryStream())
            {
                ms.Write(docBytes, 0, docBytes.Length);
                ms.Seek(0, SeekOrigin.Begin);

                using (var wordDoc = WordprocessingDocument.Open(ms, true))
                {
                    // 1. Reemplazo de Tablas (DOM)
                    ReplaceTablePlaceholders(wordDoc.MainDocumentPart, datosTabla);

                    // 2. Reemplazo de Texto (DOM - "Split Run" safe)
                    // Ahora que usamos lógica DOM para texto también, podemos hacerlo en la misma sesión
                    ProcessTextReplacements(wordDoc.MainDocumentPart, datosTexto);
                    foreach (var headerPart in wordDoc.MainDocumentPart.HeaderParts) ProcessTextReplacements(headerPart, datosTexto);
                    foreach (var footerPart in wordDoc.MainDocumentPart.FooterParts) ProcessTextReplacements(footerPart, datosTexto);
                    
                    wordDoc.Save();
                }
                return ms.ToArray();
            }
        } 


        private void ProcessTextReplacements(OpenXmlPart part, Dictionary<string, string> replacements)
        {
            if (part == null || part.RootElement == null) return;

            // Estrategia "DOM Reconstruction":
            // Iteramos por todos los párrafos. Si el texto completo del párrafo contiene un placeholder,
            // reconstruimos el párrafo con el texto reemplazado.
            // Esto soluciona el problema de "Split Runs" (donde "{{key}}" se divide en múltiples nodos XML).
            
            var paragraphs = part.RootElement.Descendants<Paragraph>().ToList();

            foreach (var paragraph in paragraphs)
            {
                string text = paragraph.InnerText;
                bool modified = false;

                foreach (var replacement in replacements)
                {
                    if (text.IndexOf(replacement.Key, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        // Usamos Regex para reemplazo case-insensitive
                        string pattern = Regex.Escape(replacement.Key);
                        text = Regex.Replace(text, pattern, replacement.Value ?? "", RegexOptions.IgnoreCase);
                        modified = true;
                    }
                }

                if (modified)
                {
                    // Guardar propiedades del párrafo anterior (alineación, estilo, etc.)
                    var pPr = paragraph.ParagraphProperties?.CloneNode(true);

                    // Guardar propiedades del primer Run para intentar preservar fuente/tamaño
                    var rPr = paragraph.Descendants<Run>().FirstOrDefault()?.RunProperties?.CloneNode(true);

                    paragraph.RemoveAllChildren();

                    if (pPr != null) paragraph.AppendChild(pPr);

                    var newRun = new Run();
                    if (rPr != null) newRun.AppendChild(rPr);
                    
                    // Manejo básico de saltos de línea en el valor de reemplazo
                    if (text.Contains("\n") || text.Contains("\r"))
                    {
                         var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                         for(int i=0; i<lines.Length; i++)
                         {
                             newRun.AppendChild(new Text(lines[i]) { Space = SpaceProcessingModeValues.Preserve });
                             if(i < lines.Length - 1) newRun.AppendChild(new Break());
                         }
                    }
                    else
                    {
                        newRun.AppendChild(new Text(text) { Space = SpaceProcessingModeValues.Preserve });
                    }
                    
                    paragraph.AppendChild(newRun);
                }
            }
        }

        private void ReplaceTablePlaceholders(MainDocumentPart mainPart, Dictionary<string, Table> tableReplacements)
        {
            var body = mainPart.Document.Body;

            foreach (var replacement in tableReplacements)
            {
                var placeholder = replacement.Key;
                var table = replacement.Value;

                // Buscar el párrafo que contiene el placeholder (case-insensitive)
                // Usamos loop para reemplazar TODAS las ocurrencias de la tabla, no solo la primera
                var paragraph = body.Descendants<Paragraph>()
                                    .FirstOrDefault(p => p.InnerText.IndexOf(placeholder, StringComparison.OrdinalIgnoreCase) >= 0);

                while (paragraph != null)
                {
                    // Limpiar el contenido del párrafo
                    paragraph.RemoveAllChildren();

                    // Cloning table for multiple insertions
                    var tableClone = table.CloneNode(true);

                    // Insertar la tabla después del párrafo
                    body.InsertAfter(tableClone, paragraph);
                    
                    // Eliminar el párrafo vacío del placeholder
                    paragraph.Remove();

                    // Buscar el siguiente (si hay más)
                    paragraph = body.Descendants<Paragraph>()
                                    .FirstOrDefault(p => p.InnerText.IndexOf(placeholder, StringComparison.OrdinalIgnoreCase) >= 0);
                }

                if (paragraph != null)
                {
                    // Limpiar el contenido del párrafo (eliminar texto restante si comparte párrafo)
                    // Ojo: Si el párrafo tiene texto crucial además del placeholder, esto lo borra. 
                    // Se asume según guía que el placeholder va en línea propia.
                    paragraph.RemoveAllChildren();

                    // Insertar la tabla después del párrafo
                    body.InsertAfter(table, paragraph);
                    
                    // Eliminar el párrafo vacío del placeholder
                    paragraph.Remove();
                }
            }
        }

        private async Task<(Dictionary<string, string> Textos, Dictionary<string, Table> Tablas)> RecopilarDatosDeReemplazo(PreContratoGeneratorDTO preContratoData, byte[] plantillaContenido)
        {
            if (preContratoData.SecCotizacion == 0) throw new ArgumentException("Se debe seleccionar una cotización.");

            var cotizacion = await _context.Cotizacion
                .Include(c => c.SecVisitaNavigation).ThenInclude(v => v.SecEmpresaNavigation)
                .Include(c => c.SecVisitaNavigation).ThenInclude(v => v.Contactovisita).ThenInclude(cv => cv.SecContactoNavigation).ThenInclude(con => con.SecConstructoraNavigation).ThenInclude(cs => cs.Cliente)
                .Include(c => c.SecVisitaNavigation).ThenInclude(v => v.SecProvinciaNavigation)
                .Include(c => c.SecVisitaNavigation).ThenInclude(v => v.SecCantonNavigation)
                .Include(c => c.SecVisitaNavigation).ThenInclude(v => v.SecParroquiaNavigation)
                .Include(c => c.SecUsuarioNavigation)
                .Include(c => c.Cotizaciondetalles)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Secuencial == preContratoData.SecCotizacion) ?? throw new InvalidOperationException("No se encontraron datos para la cotización seleccionada.");

            var visita = cotizacion.SecVisitaNavigation;
            var empresa = visita?.SecEmpresaNavigation;
            var contactoVisita = visita?.Contactovisita?.FirstOrDefault()?.SecContactoNavigation;
            var constructora = contactoVisita?.SecConstructoraNavigation;
            
            var equiposVisita = await _context.Equiposvisita
                .AsNoTracking()
                .Where(ev => ev.SecVisita == cotizacion.SecVisita)
                .ToListAsync();

            // Diccionarios de resultados
            var dicTexto = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var dicTablas = new Dictionary<string, Table>(StringComparer.OrdinalIgnoreCase);
            
            var parametros = await _repositorioDiccionario.Consultar(p => p.EstaActivo);
            var listaParametros = await parametros.ToListAsync();

            foreach (var param in listaParametros)
            {
                string key = $"{{{{{param.Parametro}}}}}";
                string valor = "";
                
                switch (param.Parametro.ToLower())
                {
                    // --- Generación de Tablas ---
                    case "tablaespecificacionesequipo":
                        dicTablas[key] = GenerarTablaEspecificaciones(equiposVisita);
                        continue; 
                    case "detalleinstalacionyducto":
                        dicTablas[key] = GenerarTablaInstalacion(equiposVisita);
                        continue;
                    case "tabladepagosconfechasytotales":
                        dicTablas[key] = GenerarTablaPagos(preContratoData); // Usamos la data del DTO por ahora, luego de la BD si aplica
                        continue;

                    // --- Datos de Texto ---
                    case "diasdeentrega": valor = preContratoData.Dias.ToString(); break;
                    case "aniosgarantia": valor = preContratoData.AniosGarantia.ToString(); break;
                    case "mesesgarantia": valor = preContratoData.MesesGarantia.ToString(); break;
                    case "periodomantenimiento": valor = preContratoData.PeriodoMantenimiento; break;
                    case "polizagarantia": valor = preContratoData.PolizaGarantia; break;
                    case "totalcontrato": valor = cotizacion.TotalConImpuestos.ToString("N2"); break;
                    case "totalcontratoenterosenletras":
                        var totalEnLetras = (int)Math.Truncate(cotizacion.TotalConImpuestos);
                        valor = totalEnLetras.ToWords(new System.Globalization.CultureInfo("es")).ToUpper();
                        break;
                    case "centavoscontrato":
                        var centavos = (int)Math.Round((cotizacion.TotalConImpuestos - Math.Truncate(cotizacion.TotalConImpuestos)) * 100);
                        valor = centavos.ToString("00");
                        break;
                    case "nombreproyecto": valor = visita?.Nombre; break;
                    case "nombreproyectoenmayusculas": valor = visita?.Nombre?.ToUpper(); break;
                    case "nombreprovincia": valor = visita?.SecProvinciaNavigation?.Nombre; break;
                    case "nombrecanton": valor = visita?.SecCantonNavigation?.Nombre; break;
                    case "nombreparroquia": valor = visita?.SecParroquiaNavigation?.Nombre; break;
                    case "direccionproyecto": valor = visita?.Direccion; break;
                    case "empresanombre": valor = empresa?.Nombre; break;
                    case "empresaid": valor = empresa?.Identificacion; break;
                    case "empresa_direccion": valor = empresa?.Direccion; break;
                    case "empresa_telefono": valor = empresa?.Telefono; break;
                    case "nombrecompletocliente": valor = constructora?.Nombre; break;
                    case "identificacioncliente": valor = constructora?.Ruc ?? constructora?.Cliente?.NumeroCliente; break;
                    case "clientedireccion": valor = constructora?.Direccion; break;
                    case "clientetelefono": valor = constructora?.Telefono; break;
                    case "clientecorreo": valor = constructora?.Correo; break;
                    case "clienterepresentantelegal": valor = constructora?.Administrador; break;
                    case "emailcontacto": valor = contactoVisita?.Correo; break;
                    case "identificacioncontacto": valor = contactoVisita?.SecConstructoraNavigation?.Ruc ?? "No disponible"; break;
                    case "marcaequipo": valor = equiposVisita.FirstOrDefault()?.Marca; break;
                    case "numerodeparadas": valor = equiposVisita.FirstOrDefault()?.NumeroParadas.ToString(); break;
                    case "cantidad": valor = equiposVisita.FirstOrDefault()?.Cantidad.ToString(); break;
                    case "numerohojasdocumentogenerado":
                        // Intentamos obtener el número de páginas de las propiedades del documento
                        // Nota: Word calcula esto al abrir, pero la plantilla suele traer el valor de su última versión.
                        using (var msHojas = new MemoryStream(plantillaContenido))
                        using (var wordDocHojas = WordprocessingDocument.Open(msHojas, false))
                        {
                            valor = wordDocHojas.ExtendedFilePropertiesPart?.Properties?.Pages?.Text ?? "1";
                        }
                        break;
                    
                    case "fechafirmacontratoenletras": valor = DateTime.Now.ToString("dd 'de' MMMM 'de' yyyy", new System.Globalization.CultureInfo("es-ES")); break;
                    case "fecha_actual": valor = DateTime.Now.ToString("dd/MM/yyyy"); break;
                    case "fecha_actual_larga": valor = DateTime.Now.ToString("dd 'de' MMMM 'de' yyyy", new System.Globalization.CultureInfo("es-ES")); break;
                }
                
                dicTexto[key] = valor ?? "";
            }

            return (dicTexto, dicTablas);
        }

        private Table GenerarTablaEspecificaciones(List<Equiposvisita> equipos)
        {
            var table = CreateBaseTable();
            AddHeaderRow(table, "Cantidad", "Tipo Equipo", "Marca", "Motor", "Capacidad", "Paradas");

            foreach (var e in equipos)
            {
                AddRow(table, 
                    e.Cantidad.ToString(), 
                    e.TipoEquipo ?? "", 
                    e.Marca ?? "", 
                    e.TipoMotor ?? "", 
                    e.Capacidad.ToString(), 
                    e.NumeroParadas.ToString());
            }
            return table;
        }

        private Table GenerarTablaInstalacion(List<Equiposvisita> equipos)
        {
            var table = CreateBaseTable();
            AddHeaderRow(table, "Ducto", "Medidas", "Recorrido (m)", "Foso (m)", "Sala Máq.");

            foreach (var e in equipos)
            {
                AddRow(table, 
                    e.TipoDucto ?? "-", 
                    e.MedidasAfducto ?? "-", 
                    e.Recorrido.HasValue ? e.Recorrido.Value.ToString() : "-", 
                    e.Foso.HasValue ? e.Foso.Value.ToString() : "-", 
                    e.SalaMaquinas ?? "-");
            }
            return table;
        }

        private Table GenerarTablaPagos(PreContratoGeneratorDTO data)
        {
            var table = CreateBaseTable();
            AddHeaderRow(table, "Detalle", "Monto", "Fecha Vencimiento");

             if (data is BLL.DTOs.PreContratoConPagosDTO dataConPagos && dataConPagos.CompromisosDePago != null && dataConPagos.CompromisosDePago.Any())
             {
                 foreach(var pago in dataConPagos.CompromisosDePago)
                 {
                     AddRow(table, pago.Tipo, pago.Monto.ToString("N2"), pago.FechaVencimiento.ToString("dd 'de' MMMM 'de' yyyy", new System.Globalization.CultureInfo("es-ES"))); 
                 }
                 // Agregar fila de Total
                 decimal total = dataConPagos.CompromisosDePago.Sum(p => p.Monto);
                 AddRowTotal(table, "TOTAL:", total.ToString("N2"), "");
             }
             else
             {
                 AddRow(table, "Anticipo", "---", "---");
                 AddRow(table, "Saldo contra entrega", "---", "---");
             }

            return table;
        }

        private void AddRowTotal(Table table, string label, string amount, string date)
        {
            var tr = new TableRow();
            
            // Label
            var tcLabel = new TableCell(new Paragraph(new Run(new Text(label) { Space = SpaceProcessingModeValues.Preserve })));
            tcLabel.Append(new TableCellProperties(new GridSpan { Val = 1 })); 
            tcLabel.GetFirstChild<Paragraph>().GetFirstChild<Run>().RunProperties = new RunProperties(new Bold());
            tr.Append(tcLabel);

            // Amount
            var tcAmount = new TableCell(new Paragraph(new Run(new Text(amount) { Space = SpaceProcessingModeValues.Preserve })));
            tcAmount.Append(new TableCellProperties(new GridSpan { Val = 1 })); 
            tcAmount.GetFirstChild<Paragraph>().GetFirstChild<Run>().RunProperties = new RunProperties(new Bold());
            tr.Append(tcAmount);

            // Date (Empty)
            tr.Append(new TableCell(new Paragraph(new Run(new Text(date) { Space = SpaceProcessingModeValues.Preserve }))));

            table.Append(tr);
        }

        // --- Helpers de OpenXML ---

        private Table CreateBaseTable()
        {
            var table = new Table();
            var tblPr = new TableProperties(
                new TableStyle { Val = "TableGrid" }, // Estilo básico de Word
                new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct }, // 100%
                new TableBorders(
                    new TopBorder { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder { Val = BorderValues.Single, Size = 4 },
                    new RightBorder { Val = BorderValues.Single, Size = 4 },
                    new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
                    new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 }
                )
            );
            table.AppendChild(tblPr);
            return table;
        }

        private void AddHeaderRow(Table table, params string[] headers)
        {
            var tr = new TableRow();
            foreach (var header in headers)
            {
                var tc = new TableCell(new Paragraph(new Run(new Text(header) { Space = SpaceProcessingModeValues.Preserve })));
                tc.Append(new TableCellProperties(
                    new Shading { Val = ShadingPatternValues.Clear, Fill = "E0E0E0" }, // Fondo gris
                    new TableCellWidth { Type = TableWidthUnitValues.Auto }
                ));
                // Poner texto en negrita
                tc.GetFirstChild<Paragraph>().GetFirstChild<Run>().RunProperties = new RunProperties(new Bold());
                tr.Append(tc);
            }
            table.Append(tr);
        }

        private void AddRow(Table table, params string[] values)
        {
            var tr = new TableRow();
            foreach (var val in values)
            {
                tr.Append(new TableCell(new Paragraph(new Run(new Text(val ?? "") { Space = SpaceProcessingModeValues.Preserve }))));
            }
            table.Append(tr);
        }

        public Task<PlaceholderDataDTO> ObtenerDatosParaPlaceholders(PreContratoGeneratorDTO preContratoData)
        {
            // Este método era para la vista previa HTML. Ya no es prioritario, pero se mantiene para compatibilidad
            // si alguna UI lo llama. Implementación simplificada.
            return Task.FromResult(new PlaceholderDataDTO());
        }
    }
}
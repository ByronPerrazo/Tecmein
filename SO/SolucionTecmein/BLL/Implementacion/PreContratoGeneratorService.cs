using BLL.ContractEngine;
using BLL.DTOs;
using BLL.Interfaces;
using DAL.DBContext;
using DAL.Interfaces;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Entity;
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
        private readonly IEnumerable<IPlaceholderProvider> _providers;

        public PreContratoGeneratorService(TecmeindbContext context, IEnumerable<IPlaceholderProvider> providers)
        {
            _context = context;
            _providers = providers;
        }

        public async Task<byte[]> GenerarVistaPreviaDocx(PreContratoGeneratorDTO preContratoData)
        {
            var tipoDocumento = await _context.TipoDocumentos.FindAsync(preContratoData.SecTipoDocumento);
            PlantillaPreContrato? plantilla = null;

            if (tipoDocumento != null && tipoDocumento.SecPlantilla.HasValue)
            {
                plantilla = await _context.PlantillaPreContratos
                                          .Include(p => p.PlantillaPreContratoParrafos)
                                          .AsNoTracking()
                                          .FirstOrDefaultAsync(p => p.SecPlantillaPreContrato == tipoDocumento.SecPlantilla && p.EstaActivo == 1);
            }

            if (plantilla == null)
            {
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

            // Recopilar datos usando los proveedores modulares
            var (datosTexto, datosTabla) = await RecopilarDatosModulares(preContratoData);
            
            byte[] docBytes = parrafoPlantilla.Contenido;

            using (var ms = new MemoryStream())
            {
                ms.Write(docBytes, 0, docBytes.Length);
                ms.Seek(0, SeekOrigin.Begin);

                using (var wordDoc = WordprocessingDocument.Open(ms, true))
                {
                    // 1. Reemplazo de Tablas (Procesar todo el cuerpo incluyendo tablas anidadas)
                    ReplaceTablePlaceholders(wordDoc.MainDocumentPart, datosTabla);

                    // 2. Reemplazo de Texto (Cuerpo, Encabezados y Pies de página)
                    var allParts = new List<OpenXmlPart> { wordDoc.MainDocumentPart };
                    allParts.AddRange(wordDoc.MainDocumentPart.HeaderParts);
                    allParts.AddRange(wordDoc.MainDocumentPart.FooterParts);

                    foreach (var part in allParts)
                    {
                        ProcessTextReplacements(part, datosTexto);
                    }
                    
                    wordDoc.Save();
                }
                return ms.ToArray();
            }
        }

        private async Task<(Dictionary<string, string> Textos, Dictionary<string, Table> Tablas)> RecopilarDatosModulares(PreContratoGeneratorDTO data)
        {
            var dicTexto = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var dicTablas = new Dictionary<string, Table>(StringComparer.OrdinalIgnoreCase);

            // Carga Única de Datos (Optimización: Evitar N+1 y consultas redundantes)
            var cotizacion = await _context.Cotizacion
                .Include(c => c.Cotizaciondetalles)
                .Include(c => c.SecVisitaNavigation).ThenInclude(v => v.SecEmpresaNavigation)
                .Include(c => c.SecVisitaNavigation).ThenInclude(v => v.SecProvinciaNavigation)
                .Include(c => c.SecVisitaNavigation).ThenInclude(v => v.SecCantonNavigation)
                .Include(c => c.SecVisitaNavigation).ThenInclude(v => v.SecParroquiaNavigation)
                .Include(c => c.SecVisitaNavigation)
                    .ThenInclude(v => v.Contactovisita)
                        .ThenInclude(cv => cv.SecContactoNavigation)
                            .ThenInclude(con => con.SecConstructoraNavigation)
                                .ThenInclude(cs => cs.Cliente)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Secuencial == data.SecCotizacion);

            if (cotizacion == null)
            {
                throw new InvalidOperationException($"No se encontró la cotización con secuencial {data.SecCotizacion}");
            }

            var context = new BLL.ContractEngine.ContractEngineContext
            {
                Data = data,
                Cotizacion = cotizacion
            };

            foreach (var provider in _providers)
            {
                await provider.ResolveAsync(dicTexto, dicTablas, context);
            }

            return (dicTexto, dicTablas);
        }

        private void ProcessTextReplacements(OpenXmlPart part, Dictionary<string, string> replacements)
        {
            if (part == null || part.RootElement == null) return;

            // 1. Simplificar los Runs: Unir textos adyacentes con el mismo formato
            // Esto resuelve el problema de Word dividiendo "{{placeholder}}" en varios runs.
            SimplifyRuns(part.RootElement);

            // 2. Realizar el reemplazo a nivel de Text nodes (Preserva Formato)
            var textNodes = part.RootElement.Descendants<Text>().ToList();
            
            foreach (var textNode in textNodes)
            {
                string text = textNode.Text;
                if (string.IsNullOrEmpty(text)) continue;

                bool modified = false;
                foreach (var replacement in replacements)
                {
                    // Crear un patrón que permita espacios opcionales dentro de las llaves
                    // Ejemplo: {{ nombre }} coincidirá con {{nombre}}
                    string keyContent = replacement.Key.Trim('{', '}');
                    string pattern = @"\{\{\s*" + Regex.Escape(keyContent) + @"\s*\}\}";
                    
                    if (Regex.IsMatch(text, pattern, RegexOptions.IgnoreCase))
                    {
                        text = Regex.Replace(text, pattern, replacement.Value ?? "", RegexOptions.IgnoreCase);
                        modified = true;
                    }
                }

                if (modified)
                {
                    // Si hay saltos de línea en el reemplazo, debemos manejarlo con <Break/>
                    if (text.Contains("\n") || text.Contains("\r"))
                    {
                        InsertTextWithBreaks(textNode, text);
                    }
                    else
                    {
                        textNode.Text = text;
                    }
                }
            }
        }

        private void SimplifyRuns(OpenXmlElement element)
        {
            var paragraphs = element.Descendants<Paragraph>().ToList();
            foreach (var paragraph in paragraphs)
            {
                var runs = paragraph.Elements<Run>().ToList();
                for (int i = runs.Count - 2; i >= 0; i--)
                {
                    Run currentRun = runs[i];
                    Run nextRun = runs[i + 1];

                    var currentText = currentRun.GetFirstChild<Text>();
                    var nextText = nextRun.GetFirstChild<Text>();

                    if (currentText == null || nextText == null) continue;

                    // Si ambos tienen las mismas propiedades, o si el texto combinado parece un placeholder
                    // forzamos el merge para evitar que Word rompa {{marcador}} en múltiples nodos.
                    bool sameProps = CompareProperties(currentRun.RunProperties, nextRun.RunProperties);
                    bool isPlaceholderSplit = (currentText.Text.Contains("{") || nextText.Text.Contains("}"));

                    if (sameProps || isPlaceholderSplit)
                    {
                        currentText.Text += nextText.Text;
                        currentText.Space = SpaceProcessingModeValues.Preserve;
                        nextRun.Remove();
                        runs.RemoveAt(i + 1);
                    }
                }
            }
        }

        private bool CompareProperties(RunProperties? p1, RunProperties? p2)
        {
            if (p1 == null && p2 == null) return true;
            if (p1 == null || p2 == null) return false;
            
            // Ignoramos diferencias menores como idiomas o correcciones ortográficas
            // que Word inserta automáticamente y rompen los marcadores.
            string xml1 = p1.OuterXml.Replace("w:lang", "lang").Replace("w:noProof", "np");
            string xml2 = p2.OuterXml.Replace("w:lang", "lang").Replace("w:noProof", "np");
            
            return xml1 == xml2;
        }

        private void InsertTextWithBreaks(Text textNode, string fullText)
        {
            var parent = textNode.Parent;
            if (parent == null) return;

            var run = (Run)parent;
            string[] lines = fullText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            
            run.RemoveAllChildren<Text>();
            run.RemoveAllChildren<Break>();

            for (int i = 0; i < lines.Length; i++)
            {
                run.AppendChild(new Text(lines[i]) { Space = SpaceProcessingModeValues.Preserve });
                if (i < lines.Length - 1)
                {
                    run.AppendChild(new Break());
                }
            }
        }

        private void ReplaceTablePlaceholders(MainDocumentPart mainPart, Dictionary<string, Table> tableReplacements)
        {
            var body = mainPart.Document.Body;
            if (body == null) return;

            foreach (var replacement in tableReplacements)
            {
                var placeholder = replacement.Key;
                var tableToInsert = replacement.Value;

                // Buscar párrafos que contengan el placeholder
                var targetParagraphs = body.Descendants<Paragraph>()
                    .Where(p => p.InnerText.Contains(placeholder, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                foreach (var paragraph in targetParagraphs)
                {
                    // Insertar la tabla después del párrafo
                    paragraph.Parent?.InsertAfter(tableToInsert.CloneNode(true), paragraph);
                    // Eliminar el párrafo que servía de placeholder
                    paragraph.Remove();
                }
            }
        }

        public Task<PlaceholderDataDTO> ObtenerDatosParaPlaceholders(PreContratoGeneratorDTO preContratoData)
        {
            return Task.FromResult(new PlaceholderDataDTO());
        }
    }
}
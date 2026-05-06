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

            foreach (var provider in _providers)
            {
                await provider.ResolveAsync(dicTexto, dicTablas, data);
            }

            return (dicTexto, dicTablas);
        }

        private void ProcessTextReplacements(OpenXmlPart part, Dictionary<string, string> replacements)
        {
            if (part == null || part.RootElement == null) return;

            // Buscamos en todos los párrafos, incluso los que están dentro de celdas de tablas
            var paragraphs = part.RootElement.Descendants<Paragraph>().ToList();

            foreach (var paragraph in paragraphs)
            {
                string text = paragraph.InnerText;
                if (string.IsNullOrEmpty(text)) continue;

                bool modified = false;

                foreach (var replacement in replacements)
                {
                    if (text.Contains(replacement.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        // Usamos Regex para reemplazo case-insensitive y robusto
                        string pattern = Regex.Escape(replacement.Key);
                        text = Regex.Replace(text, pattern, replacement.Value ?? "", RegexOptions.IgnoreCase);
                        modified = true;
                    }
                }

                if (modified)
                {
                    // Preservar propiedades de párrafo
                    var pPr = paragraph.ParagraphProperties?.CloneNode(true);
                    var rPr = paragraph.Descendants<Run>().FirstOrDefault()?.RunProperties?.CloneNode(true);

                    paragraph.RemoveAllChildren();
                    if (pPr != null) paragraph.AppendChild(pPr);

                    var newRun = new Run();
                    if (rPr != null) newRun.AppendChild(rPr);

                    // Manejo de saltos de línea
                    string[] lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                    for (int i = 0; i < lines.Length; i++)
                    {
                        newRun.AppendChild(new Text(lines[i]) { Space = SpaceProcessingModeValues.Preserve });
                        if (i < lines.Length - 1) newRun.AppendChild(new Break());
                    }
                    
                    paragraph.AppendChild(newRun);
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
using BLL.Interfaces;
using Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace BLL.Implementacion
{
    public class EstrategiaPreContrato : IEstrategiaGeneradorDocumento
    {
        public string CodigoTipoDocumento => "PRECONTRATO";
        private const string TablaPagosPlaceholder = "{{TablaDePagosConFechasYTotales}}";

        private readonly IPlantillaPreContratoServices _plantillaPreContratoServices;
        private readonly IPlantillaPreContratoParrafoServices _plantillaPreContratoParrafoServices;

        public EstrategiaPreContrato(
            IPlantillaPreContratoServices plantillaPreContratoServices,
            IPlantillaPreContratoParrafoServices plantillaPreContratoParrafoServices)
        {
            _plantillaPreContratoServices = plantillaPreContratoServices;
            _plantillaPreContratoParrafoServices = plantillaPreContratoParrafoServices;
        }

        public async Task<byte[]> Generar(int secPlantilla, object datos)
        {
            PlantillaPreContrato plantilla = await _plantillaPreContratoServices.Obtener(secPlantilla);
            if (plantilla == null)
                throw new Exception($"Plantilla de Pre-Contrato con ID {secPlantilla} no encontrada.");

            var parrafoConContenido = (await _plantillaPreContratoParrafoServices.Lista(secPlantilla))
                .FirstOrDefault(p => p.Contenido != null && p.Contenido.Length > 0);
            if (parrafoConContenido == null)
                throw new Exception($"No se encontró contenido de plantilla para la plantilla {secPlantilla}.");

            byte[] templateBytes = parrafoConContenido.Contenido;

            using (MemoryStream memStream = new MemoryStream())
            {
                await memStream.WriteAsync(templateBytes, 0, templateBytes.Length);

                using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(memStream, true))
                {
                    // 1. Handle complex placeholders like tables first
                    HandleComplexPlaceholders(wordDocument, datos);

                    // 2. Prepare dictionary for simple text replacements
                    var simpleReplacements = new Dictionary<string, string>();
                    Type dataType = datos.GetType();
                    foreach (PropertyInfo prop in dataType.GetProperties())
                    {
                        // Only process simple types or strings, ignore collections
                        if (!typeof(IEnumerable).IsAssignableFrom(prop.PropertyType) || prop.PropertyType == typeof(string))
                        {
                            string placeholder = "{{" + prop.Name + "}}";
                            object value = prop.GetValue(datos);
                            simpleReplacements[placeholder] = value?.ToString() ?? string.Empty;
                        }
                    }

                    // 3. Perform simple text replacements everywhere
                    await ReplaceInPart(wordDocument.MainDocumentPart, simpleReplacements);
                    foreach (var headerPart in wordDocument.MainDocumentPart.HeaderParts)
                    {
                        await ReplaceInPart(headerPart, simpleReplacements);
                    }
                    foreach (var footerPart in wordDocument.MainDocumentPart.FooterParts)
                    {
                        await ReplaceInPart(footerPart, simpleReplacements);
                    }
                }
                return memStream.ToArray();
            }
        }

        private void HandleComplexPlaceholders(WordprocessingDocument wordDoc, object datos)
        {
            // Find payment commitments property
            var compromisosProp = datos.GetType().GetProperty("CompromisosDePago");
            if (compromisosProp != null)
            {
                var compromisos = compromisosProp.GetValue(datos) as IEnumerable<PreContratoCompromisoPago>;
                if (compromisos != null && compromisos.Any())
                {
                    // Find the paragraph containing the table placeholder in the main body
                    var body = wordDoc.MainDocumentPart.Document.Body;
                    var paraToReplace = body.Descendants<Paragraph>()
                        .FirstOrDefault(p => p.InnerText.Contains(TablaPagosPlaceholder));

                    if (paraToReplace != null)
                    {
                        // Create the table
                        Table table = CreatePaymentTable(compromisos);
                        // Replace paragraph with the table
                        paraToReplace.Parent.InsertBefore(table, paraToReplace);
                        paraToReplace.Remove();
                    }
                }
            }
        }

        private Table CreatePaymentTable(IEnumerable<PreContratoCompromisoPago> compromisos)
        {
            Table table = new Table();

            // Set table properties for borders
            TableProperties props = new TableProperties(
                new TableBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },
                    new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 },
                    new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 6 }
                )
            );
            table.AppendChild(props);

            // Create header row
            TableRow headerRow = new TableRow();
            headerRow.Append(CreateTableCell("Tipo", true), CreateTableCell("Fecha de Vencimiento", true), CreateTableCell("Monto", true));
            table.Append(headerRow);

            // Create data rows
            foreach (var item in compromisos)
            {
                TableRow dataRow = new TableRow();
                dataRow.Append(
                    CreateTableCell(item.Tipo),
                    CreateTableCell(item.FechaVencimiento.ToString("dd/MM/yyyy")),
                    CreateTableCell(item.Monto.ToString("C")) // "C" for currency format
                );
                table.Append(dataRow);
            }
            return table;
        }

        private TableCell CreateTableCell(string text, bool isHeader = false)
        {
            var paragraph = new Paragraph(new Run(new Text(text)));
            if (isHeader)
            {
                var runProperties = new RunProperties(new Bold());
                paragraph.GetFirstChild<Run>().PrependChild(runProperties);
            }
            return new TableCell(paragraph);
        }

        private async Task ReplaceInPart(OpenXmlPart part, Dictionary<string, string> replacements)
        {
            string docText;
            using (var reader = new StreamReader(part.GetStream()))
            {
                docText = await reader.ReadToEndAsync();
            }

            bool updated = false;
            foreach (var replacement in replacements)
            {
                if (docText.Contains(replacement.Key))
                {
                    docText = docText.Replace(replacement.Key, replacement.Value);
                    updated = true;
                }
            }

            if (updated)
            {
                using (var writer = new StreamWriter(part.GetStream(FileMode.Create)))
                {
                    await writer.WriteAsync(docText);
                }
            }
        }
    }
}

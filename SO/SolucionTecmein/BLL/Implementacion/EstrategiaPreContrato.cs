
using BLL.Interfaces;
using Entity;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using HtmlAgilityPack;

namespace BLL.Implementacion
{
    public class EstrategiaPreContrato : IEstrategiaGeneradorDocumento
    {
        public string CodigoTipoDocumento => "PRECONTRATO";

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
            // Phase 1: Fetching Template and Paragraphs
            PlantillaPreContrato plantilla = await _plantillaPreContratoServices.Obtener(secPlantilla);

            if (plantilla == null)
            {
                throw new Exception($"Plantilla de Pre-Contrato con ID {secPlantilla} no encontrada.");
            }

            var parrafos = await _plantillaPreContratoParrafoServices.Lista(secPlantilla);

            if (parrafos == null || !parrafos.Any())
            {
                Console.WriteLine($"No se encontraron párrafos para la plantilla {secPlantilla}.");
            }

            // Phase 2: Parameter Replacement
            StringBuilder documentContentBuilder = new StringBuilder();
            Type dataType = datos.GetType();

            foreach (var p in parrafos.OrderBy(p => p.Orden)) // Assuming 'Orden' property exists
            {
                string processedContent = p.Contenido;

                // Replace placeholders using reflection
                foreach (PropertyInfo prop in dataType.GetProperties())
                {
                    string placeholder = "{{" + prop.Name + "}}";
                    object value = prop.GetValue(datos);
                    processedContent = processedContent.Replace(placeholder, value?.ToString() ?? string.Empty);
                }
                documentContentBuilder.AppendLine(processedContent);
            }

            // Phase 3: Word Document Generation using Open-XML-SDK
            using (MemoryStream memStream = new MemoryStream())
            {
                using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(memStream, WordprocessingDocumentType.Document, true))
                {
                    MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    Body body = mainPart.Document.AppendChild(new Body());

                    HtmlDocument htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml("<body>" + documentContentBuilder.ToString() + "</body>");

                    foreach (HtmlNode node in htmlDoc.DocumentNode.SelectNodes("//body//*"))
                    {
                        if (node.NodeType == HtmlNodeType.Element)
                        {
                            Paragraph p = new Paragraph();
                            Run r = new Run();
                            RunProperties rp = new RunProperties();
                            
                            switch (node.Name.ToLower())
                            {
                                case "h1":
                                    rp.Append(new Bold());
                                    rp.Append(new FontSize() { Val = "32" }); // 16pt
                                    r.Append(rp);
                                    r.Append(new Text(node.InnerText));
                                    p.Append(r);
                                    body.Append(p);
                                    break;
                                case "p":
                                    r.Append(new Text(node.InnerText));
                                    p.Append(r);
                                    body.Append(p);
                                    break;
                                case "b":
                                case "strong":
                                    rp.Append(new Bold());
                                    r.Append(rp);
                                    r.Append(new Text(node.InnerText));
                                    p.Append(r);
                                    body.Append(p);
                                    break;
                                case "i":
                                case "em":
                                    rp.Append(new Italic());
                                    r.Append(rp);
                                    r.Append(new Text(node.InnerText));
                                    p.Append(r);
                                    body.Append(p);
                                    break;
                                case "u":
                                    rp.Append(new Underline() { Val = UnderlineValues.Single });
                                    r.Append(rp);
                                    r.Append(new Text(node.InnerText));
                                    p.Append(r);
                                    body.Append(p);
                                    break;
                            }
                        }
                    }
                }
                return memStream.ToArray();
            }
        }
    }
}

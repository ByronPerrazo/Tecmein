using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;
using OpenXmlPowerTools;
using System.Drawing.Imaging;

namespace BLL.Implementacion
{
    public class PlantillaPreContratoServices : IPlantillaPreContratoServices
    {
        private readonly IGenericRepository<PlantillaPreContrato> _repositorio;
        private readonly IGenericRepository<PlantillaPreContratoParrafo> _repositorioParrafos;
        private readonly IGenericRepository<DiccionarioParametro> _repositorioDiccionario;

        public PlantillaPreContratoServices(
            IGenericRepository<PlantillaPreContrato> repositorio,
            IGenericRepository<PlantillaPreContratoParrafo> repositorioParrafos,
            IGenericRepository<DiccionarioParametro> repositorioDiccionario)
        {
            _repositorio = repositorio;
            _repositorioParrafos = repositorioParrafos;
            _repositorioDiccionario = repositorioDiccionario;
        }

        public async Task<List<PlantillaPreContrato>> Lista()
        {
            IQueryable<PlantillaPreContrato> query = await _repositorio.Consultar(includeProperties: "SecTipoDocumentoNavigation");
            return query.ToList();
        }

        public async Task<PlantillaPreContrato> Obtener(int secPlantillaPreContrato)
        {
            return await _repositorio.Obtener(p => p.SecPlantillaPreContrato == secPlantillaPreContrato, incluirPropiedades: "SecTipoDocumentoNavigation");
        }

        public async Task<PlantillaPreContrato> Crear(PlantillaPreContrato entidad)
        {
            entidad.FechaRegistro = DateTime.Now;
            entidad.EstaActivo = 1;
            return await _repositorio.Crear(entidad);
        }

        public async Task<PlantillaPreContrato> Editar(PlantillaPreContrato entidad)
        {
            var plantillaExistente = await _repositorio.Obtener(p => p.SecPlantillaPreContrato == entidad.SecPlantillaPreContrato);
            if (plantillaExistente == null) throw new Exception("La plantilla de pre-contrato no existe.");

            plantillaExistente.Nombre = entidad.Nombre;
            plantillaExistente.NumeracionInicial = entidad.NumeracionInicial;
            plantillaExistente.EstaActivo = entidad.EstaActivo;
            plantillaExistente.SecTipoDocumento = entidad.SecTipoDocumento;

            await _repositorio.Editar(plantillaExistente);
            return plantillaExistente;
        }

        public async Task<bool> Eliminar(int secPlantillaPreContrato)
        {
            var plantilla = await _repositorio.Obtener(p => p.SecPlantillaPreContrato == secPlantillaPreContrato);
            if (plantilla == null) throw new Exception("La plantilla de pre-contrato no existe.");
            return await _repositorio.Eliminar(plantilla);
        }

        public async Task<(bool Exito, List<string> Advertencias)> CargarParrafosDesdeWordAsync(int secPlantillaPreContrato, Stream archivoStream)
        {
            var advertencias = new List<string>();
            try
            {
                byte[] byteArray;
                using (var memoryStream = new MemoryStream())
                {
                    await archivoStream.CopyToAsync(memoryStream);
                    byteArray = memoryStream.ToArray();
                }

                string htmlContent;
                using (var wDoc = WordprocessingDocument.Open(new MemoryStream(byteArray), true)) // true para modo editable
                {
                    var settings = new HtmlConverterSettings()
                    {
                        ImageHandler = imageInfo =>
                        {
                            var extension = imageInfo.ContentType.Split('/')[1].ToLower();
                            ImageFormat imageFormat = ImageFormat.Png; // Default
                            if (extension == "gif") imageFormat = ImageFormat.Gif;
                            else if (extension == "bmp") imageFormat = ImageFormat.Bmp;
                            else if (extension == "jpeg" || extension == "jpg") imageFormat = ImageFormat.Jpeg;
                            else if (extension == "tiff") imageFormat = ImageFormat.Tiff;

                            byte[] imageBytes;
                            using (var ms = new MemoryStream())
                            {
                                imageInfo.Bitmap.Save(ms, imageFormat);
                                imageBytes = ms.ToArray();
                            }

                            var base64 = Convert.ToBase64String(imageBytes);
                            return new XElement("img",
                                new XAttribute("src", $"data:image/{extension};base64,{base64}"),
                                imageInfo.ImgStyleAttribute);
                        }
                    };
                    var htmlElement = HtmlConverter.ConvertToHtml(wDoc, settings);
                    htmlContent = htmlElement.ToString();
                }

                var placeholdersValidos = (await _repositorioDiccionario.Consultar())
                                          .Select(p => p.Parametro)
                                          .ToHashSet();

                var placeholdersEncontrados = Regex.Matches(htmlContent, @"{{(.*?)}}");
                foreach (Match match in placeholdersEncontrados)
                {
                    if (!placeholdersValidos.Contains(match.Value))
                    {
                        advertencias.Add($"El placeholder '{{match.Value}}' no es válido.");
                    }
                }

                var nuevoParrafo = new PlantillaPreContratoParrafo
                {
                    SecPlantillaPreContrato = secPlantillaPreContrato,
                    Contenido = htmlContent,
                    Orden = 1,
                    EstaActivo = true
                };
                await _repositorioParrafos.Crear(nuevoParrafo);

                return (true, advertencias);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar y convertir el archivo Word a HTML: " + ex.Message, ex);
            }
        }

        public async Task<string> MaquetarContenidoAsync(string htmlContent, int secPlantillaPreContrato)
        {
            // 1. Obtener la lista de placeholders válidos desde la base de datos
            var placeholders = await _repositorioDiccionario.Consultar();
            var placeholderInfo = placeholders.Select(p => $"'{p.Parametro}': {p.Descripcion}").ToList();
            string placeholderList = string.Join("\n- ", placeholderInfo);

            // 2. Construir el prompt para el MCP/LLM
            string prompt = @$"Eres un asistente experto en la creación de plantillas de contratos.
Tu tarea es tomar un bloque de HTML que representa un contrato y reemplazar los datos específicos (nombres, RUCs, fechas, valores) por los placeholders apropiados de la siguiente lista.
No debes alterar la estructura HTML, solo reemplazar el texto de los datos.

Lista de placeholders disponibles:
- {placeholderList}

Ahora, procesa el siguiente contenido HTML:
---
{htmlContent}
---";

            // 3. Simular la llamada al servicio del MCP/LLM
            // TODO: En una implementación real, aquí se haría la llamada al API del modelo de lenguaje grande.
            // Por ejemplo: string resultadoDelLLM = await _mcpService.GenerarTextoAsync(prompt);
            
            // Para esta demostración, simulamos el resultado reemplazando un valor conocido del ejemplo del usuario.
            string resultadoSimulado = htmlContent.Replace("COLOMA ROMAN", "{{Nombre_Proyecto}}")
                                                  .Replace("1791733649001", "{{Proyecto_identificacion}}");


            // 4. Devolver el contenido modificado
            return await Task.FromResult(resultadoSimulado);
        }

        public async Task<bool> MaquetarParrafoAsync(int secPlantillaPreContratoParrafo)
        {
            var parrafo = await _repositorioParrafos.Obtener(p => p.SecPlantillaPreContratoParrafo == secPlantillaPreContratoParrafo);
            if (parrafo == null) throw new Exception("Párrafo de plantilla no encontrado.");

            string htmlContentOriginal = parrafo.Contenido;
            int secPlantilla = parrafo.SecPlantillaPreContrato;

            // Reutilizar la lógica de maquetado que ya tenemos
            string htmlContentMaquetado = await MaquetarContenidoAsync(htmlContentOriginal, secPlantilla);

            // Actualizar el contenido del párrafo y guardarlo
            parrafo.Contenido = htmlContentMaquetado;
            await _repositorioParrafos.Editar(parrafo);

            return true;
        }
    }
}
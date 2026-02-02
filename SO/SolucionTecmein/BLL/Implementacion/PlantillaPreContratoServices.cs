using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging; // Añadido para Open XML SDK

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
            byte[] documentoBytes;
            string textoDocumento = ""; // Para extraer placeholders

            try
            {
                // Leer el stream del archivo directamente en un array de bytes
                using (var memoryStream = new MemoryStream())
                {
                    await archivoStream.CopyToAsync(memoryStream);
                    documentoBytes = memoryStream.ToArray();
                    memoryStream.Position = 0; // Reset para OpenXml

                    // Extraer texto del DOCX para identificar placeholders usando Open XML SDK
                    // No se necesita el 'using' DocumentFormat.OpenXml.Wordprocessing; aquí para InnerText
                    using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(memoryStream, false)) // false para solo lectura
                    {
                        textoDocumento = wordDoc.MainDocumentPart?.Document.Body?.InnerText ?? "";
                    }
                }

                // --- Lógica para detectar y añadir placeholders al DiccionarioParametro ---
                var parametrosExistentes = (await _repositorioDiccionario.Consultar())
                                           .ToDictionary(p => p.Parametro, p => p.Secuencial, StringComparer.OrdinalIgnoreCase);

                var placeholdersEncontrados = Regex.Matches(textoDocumento, @"{{(.*?)}}"); // Usar textoDocumento
                foreach (Match match in placeholdersEncontrados)
                {
                    string placeholderSinCorchetes = match.Groups[1].Value.Trim();

                    if (!string.IsNullOrEmpty(placeholderSinCorchetes) && !parametrosExistentes.ContainsKey(placeholderSinCorchetes))
                    {
                        var nuevoParametro = new DiccionarioParametro
                        {
                            Parametro = placeholderSinCorchetes,
                            Descripcion = $"Generado automáticamente desde plantilla ({DateTime.Now:yyyy-MM-dd HH:mm})",
                            EstaActivo = true
                        };
                        await _repositorioDiccionario.Crear(nuevoParametro);
                        parametrosExistentes.Add(placeholderSinCorchetes, nuevoParametro.Secuencial);
                        advertencias.Add($"Placeholder '{placeholderSinCorchetes}' agregado automáticamente al diccionario.");
                    }
                }
                // --- Fin Lógica placeholders ---

                var nuevoParrafo = new PlantillaPreContratoParrafo
                {
                    SecPlantillaPreContrato = secPlantillaPreContrato,
                    Contenido = documentoBytes, // Guardar los bytes del DOCX
                    Orden = 1,
                    EstaActivo = true
                };
                await _repositorioParrafos.Crear(nuevoParrafo);

                return (true, advertencias);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al procesar el archivo Word: " + ex.Message, ex);
            }
        }
    }
}
using BLL.Interfaces;
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
    public class ContratoService : IContratoService
    {
        private readonly IGenericRepository<Contrato> _repoContrato;
        private readonly IGenericRepository<Cotizacion> _repoCotizacion;
        private readonly IGenericRepository<PreContrato> _repoPreContrato; // Inyectado
        private readonly IStorageServices _storageService; // Inyectado

        public ContratoService(IGenericRepository<Contrato> repoContrato, IGenericRepository<Cotizacion> repoCotizacion, IGenericRepository<PreContrato> repoPreContrato, IStorageServices storageService)
        {
            _repoContrato = repoContrato;
            _repoCotizacion = repoCotizacion;
            _repoPreContrato = repoPreContrato; // Asignado
            _storageService = storageService; // Asignado
        }

        public async Task<Contrato> Crear(Contrato entidad, Stream archivoStream = null, string nombreArchivo = "")
        {
            try
            {
                // 1. Crear el registro para obtener el ID
                entidad.NombreArchivo = "";
                entidad.RutaArchivo = "";
                Contrato contrato_creado = await _repoContrato.Crear(entidad);
                if (contrato_creado.IdContrato == 0)
                    throw new TaskCanceledException("No se pudo crear el contrato");

                // 2. Si hay un archivo, subirlo a Firebase
                if (archivoStream != null)
                {
                    string carpetaDestino = $"contratos/{contrato_creado.IdContrato}";
                    string urlArchivo = await _storageService.SubirStorage(archivoStream, carpetaDestino, nombreArchivo);
                    
                    // 3. Actualizar el registro con la URL del archivo
                    contrato_creado.NombreArchivo = nombreArchivo;
                    contrato_creado.RutaArchivo = urlArchivo;
                    await _repoContrato.Editar(contrato_creado);
                }

                return contrato_creado;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> Editar(Contrato entidad)
        {
            // Lógica para editar, recordando la restricción de administrador
            throw new NotImplementedException();
        }

        public async Task<bool> Eliminar(int id)
        {
            var contrato = await _repoContrato.Obtener(c => c.IdContrato == id);
            if (contrato == null)
                return false;

            // Eliminar el archivo de Firebase antes de eliminar el registro
            if (!string.IsNullOrEmpty(contrato.NombreArchivo))
            {
                string carpetaDestino = $"contratos/{id}";
                await _storageService.EliminarStorage(carpetaDestino, contrato.NombreArchivo);
            }

            return await _repoContrato.Eliminar(contrato);
        }

        public async Task<List<Contrato>> Listar()
        {
            IQueryable<Contrato> query = await _repoContrato.Consultar();
            return query.Include(c => c.IdCotizacionNavigation).Include(c => c.IdCotizacionNavigation.SecVisitaNavigation).Include(c => c.IdUsuarioCargaNavigation).ToList();
        }

        public async Task<Contrato> Obtener(int id)
        {
            return await _repoContrato.Obtener(c => c.IdContrato == id);
        }

        public async Task<List<PreContrato>> ListarPreContratosParaContrato()
        { 
            IQueryable<PreContrato> query = await _repoPreContrato.Consultar(p => p.Estado == "Aprobado" && p.EstaActivo);
            return query.Include(p => p.SecCotizacionNavigation)
                        .ThenInclude(c => c.SecVisitaNavigation)
                        .ToList();
        }
    }
}

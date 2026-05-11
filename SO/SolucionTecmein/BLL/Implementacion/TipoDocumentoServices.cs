using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;

namespace BLL.Implementacion
{
    public class TipoDocumentoServices : ITipoDocumentoServices
    {
        private readonly IGenericRepository<TipoDocumento> _repositorio;

        public TipoDocumentoServices(IGenericRepository<TipoDocumento> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<List<TipoDocumento>> Lista()
        {
            IQueryable<TipoDocumento> query = await _repositorio.Consultar();
            return query.Include(t => t.SecPlantillaNavigation).ToList();
        }

        public async Task<TipoDocumento> Crear(TipoDocumento entidad)
        {
            try
            {
                entidad.FechaRegistro = DateTime.Now;
                TipoDocumento tipoDocumento_creado = await _repositorio.Crear(entidad);
                if (tipoDocumento_creado.SecTipoDocumento == 0)
                    throw new Exception("No se pudo crear el tipo de documento");

                return tipoDocumento_creado;
            }
            catch
            {
                throw;
            }
        }

        public async Task<TipoDocumento> Editar(TipoDocumento entidad)
        {
            try
            {
                TipoDocumento tipoDocumento_encontrado = await _repositorio.Obtener(c => c.SecTipoDocumento == entidad.SecTipoDocumento);
                if (tipoDocumento_encontrado == null)
                    throw new Exception("El tipo de documento no existe");

                tipoDocumento_encontrado.Codigo = entidad.Codigo;
                tipoDocumento_encontrado.Descripcion = entidad.Descripcion;
                tipoDocumento_encontrado.EstaActivo = entidad.EstaActivo;
                tipoDocumento_encontrado.SecPlantilla = entidad.SecPlantilla;


                bool respuesta = await _repositorio.Editar(tipoDocumento_encontrado);
                if (!respuesta)
                    throw new Exception("No se pudo modificar el tipo de documento");

                return tipoDocumento_encontrado;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int secTipoDocumento)
        {
            try
            {
                TipoDocumento tipoDocumento_encontrado = await _repositorio.Obtener(c => c.SecTipoDocumento == secTipoDocumento);
                if (tipoDocumento_encontrado == null)
                    throw new Exception("El tipo de documento no existe");

                bool respuesta = await _repositorio.Eliminar(tipoDocumento_encontrado);
                return respuesta;
            }
            catch
            {
                throw;
            }
        }

        public async Task<TipoDocumento> Obtener(int secTipoDocumento)
        {
            TipoDocumento tipoDocumento_encontrado = await _repositorio.Obtener(c => c.SecTipoDocumento == secTipoDocumento);
            return tipoDocumento_encontrado;
        }

        public async Task<TipoDocumento> ObtenerPorCodigo(string codigo)
        {
            TipoDocumento tipoDocumento_encontrado = await _repositorio.Obtener(td => td.Codigo == codigo);
            return tipoDocumento_encontrado;
        }
    }
}

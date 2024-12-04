using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;

namespace BLL.Implementacion
{
    public class ContactoServices : IContactoServices
    {
        public readonly IGenericRepository<Contacto> _repositorio;
        public readonly IConstructoraServices _constructoraServices;

        public ContactoServices(IGenericRepository<Contacto> repositorio,
                                IConstructoraServices constructoraServices)
        {
            _repositorio = repositorio;
            _constructoraServices = constructoraServices;
        }

        public async Task<Contacto> ContactoPorSecuencial(int secuencial)
        {
            var query = await _repositorio.Obtener(x => x.Secuencial == secuencial);
            return query;

        }

        public async Task<Contacto> Crear(Contacto entidad)
        {
            if (await _repositorio.Obtener(x => x.Nombres == entidad.Nombres && x.Apellidos == entidad.Apellidos) != null)
                throw new TaskCanceledException($"Error Nombre Contacto Ya Registrado");

            var regitroGuardado = await _repositorio.Crear(entidad);

            regitroGuardado
                .SecConstructoraNavigation
                 = await _constructoraServices
                         .ConstructoraPorSecuencial(regitroGuardado.SecConstructora);

            return regitroGuardado;
        }

        public async Task<Contacto> Editar(Contacto entidad)
        {
            var registro
                = await _repositorio
                        .Obtener(x => x.Secuencial == entidad.Secuencial)
                  ?? throw new TaskCanceledException("Registro No Existe");

            if (registro != null)
            {
                registro.SecConstructora = entidad.SecConstructora;
                registro.Titulo = entidad.Titulo;
                registro.Nombres = entidad.Nombres;
                registro.Apellidos = entidad.Apellidos;
                registro.Telefono = entidad.Telefono;
                registro.Correo = entidad.Correo;
                registro.EstaActivo = entidad.EstaActivo;
                var regitroGuardado = await _repositorio.Editar(registro);
            }

            var obtenido
                = await _repositorio
                        .Obtener(x => x.Secuencial == entidad.Secuencial);

            obtenido.SecConstructoraNavigation
                = await _constructoraServices
                        .ConstructoraPorSecuencial(obtenido.SecConstructora);

            return obtenido;
        }

        public async Task<bool> Eliminar(int secuencial)
        {
            try
            {
                var seElimino = false;
                var registro
                    = await _repositorio
                             .Consultar(x => x.Secuencial == secuencial);

                var constructora = registro.FirstOrDefault();
                if (constructora != null)
                {
                    await _repositorio.Eliminar(constructora);
                    seElimino = true;
                }
                return seElimino;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Contacto>> Lista()
        {
            var query = await _repositorio.Consultar();
            var queryIncludes = query.Include(x => x.SecConstructoraNavigation)
                                     .ToList();
            return [.. queryIncludes];
        }

        public async Task<List<Contacto>> ListaPorConstructora(int secConstructora)
        {
            var query = await _repositorio.Consultar();
            var queryIncludes = query
                                .Where(a => a.SecConstructora == secConstructora)
                                .Include(x => x.SecConstructoraNavigation)
                                .ToList();
            return queryIncludes;
        }
    }
}

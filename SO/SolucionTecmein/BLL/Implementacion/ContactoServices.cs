using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;

namespace BLL.Implementacion
{
    public class ContactoServices : IContactoServices
    {
        public readonly IGenericRepository<Contacto> _repositorio;
        public readonly IGenericRepository<Contactovisita> _repositorioContactoVisita;
        public readonly IConstructoraServices _constructoraServices;
        public readonly IValidacionServices _validacionServices;

        public ContactoServices(IGenericRepository<Contacto> repositorio,
                                IGenericRepository<Contactovisita> repositorioContactoVisita,
                                IConstructoraServices constructoraServices,
                                IValidacionServices validacionServices)
        {
            _repositorio = repositorio;
            _repositorioContactoVisita = repositorioContactoVisita;
            _constructoraServices = constructoraServices;
            _validacionServices = validacionServices;
        }

        public async Task<Contacto> ObtenerPorId(int secuencial)
        {
            return await _repositorio.Obtener(c => c.Secuencial == secuencial, "SecConstructoraNavigation");
        }

        public async Task<Contacto> ContactoPorSecuencial(int secuencial)
        {
            var query = await _repositorio.Obtener(x => x.Secuencial == secuencial);
            return query;

        }

        public async Task<Contacto> Crear(Contacto entidad)
        {
            // Validaciones comunes
            _validacionServices.ValidarNombre(entidad.Nombres, "nombre");
            _validacionServices.ValidarNombre(entidad.Apellidos, "apellido");
            _validacionServices.ValidarCorreo(entidad.Correo);
            _validacionServices.ValidarTelefonoEcuador(entidad.Telefono);

            // Validaciones específicas de Contacto
            if (entidad.SecConstructora <= 0)
            {
                throw new TaskCanceledException("La constructora es obligatoria.");
            }
            var constructoraExistente = await _constructoraServices.ConstructoraPorSecuencial(entidad.SecConstructora);
            if (constructoraExistente == null)
            {
                throw new TaskCanceledException($"La constructora con secuencial {entidad.SecConstructora} no existe.");
            }

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
            // Validaciones comunes
            _validacionServices.ValidarNombre(entidad.Nombres, "nombre");
            _validacionServices.ValidarNombre(entidad.Apellidos, "apellido");
            _validacionServices.ValidarCorreo(entidad.Correo);
            _validacionServices.ValidarTelefonoEcuador(entidad.Telefono);

            // Validaciones específicas de Contacto
            if (entidad.SecConstructora <= 0)
            {
                throw new TaskCanceledException("La constructora es obligatoria.");
            }
            var constructoraExistente = await _constructoraServices.ConstructoraPorSecuencial(entidad.SecConstructora);
            if (constructoraExistente == null)
            {
                throw new TaskCanceledException($"La constructora con secuencial {entidad.SecConstructora} no existe.");
            }

            var registro
                = await _repositorio
                        .Obtener(x => x.Secuencial == entidad.Secuencial)
                  ?? throw new TaskCanceledException("Registro No Existe");

            registro.SecConstructora = entidad.SecConstructora;
            registro.Titulo = entidad.Titulo;
            registro.Nombres = entidad.Nombres;
            registro.Apellidos = entidad.Apellidos;
            registro.Telefono = entidad.Telefono;
            registro.Correo = entidad.Correo;
            registro.EstaActivo = entidad.EstaActivo;

            bool seEdito = await _repositorio.Editar(registro);
            if (!seEdito)
                throw new TaskCanceledException("No se pudo editar el contacto.");

            registro.SecConstructoraNavigation
                = await _constructoraServices
                        .ConstructoraPorSecuencial(registro.SecConstructora);

            return registro;
        }

        public async Task<bool> Eliminar(int secuencial)
        {
            var registro
                = await _repositorio
                         .Obtener(x => x.Secuencial == secuencial);

            if (registro == null)
            {
                return false;
            }
            return await _repositorio.Eliminar(registro);
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

        public async Task<List<Contacto>> Lista()
        {
            var query = await _repositorio.Consultar();
            var queryIncludes = query.Include(x => x.SecConstructoraNavigation)
                                     .ToList();
            return [.. queryIncludes];
        }

        public async Task<Contacto> ObtenerContactoPrincipal(int secuencialVisita)
        {
            var contactoVisita = await _repositorioContactoVisita.Obtener(cv => cv.SecVisita == secuencialVisita && cv.EstaActivo == 1);

            if (contactoVisita != null)
            {
                return await _repositorio.Obtener(c => c.Secuencial == contactoVisita.SecContacto);
            }
            return null;
        }
    }
}

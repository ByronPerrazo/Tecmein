using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;

namespace BLL.Implementacion
{
    public class ContactoVisitaServices : IContactoVisitaServices
    {

        public readonly IGenericRepository<Contactovisita> _repositorio;
        public readonly IContactoServices _contactoServices;
        public readonly IVisitaServices _visitaServices;

        public ContactoVisitaServices(IGenericRepository<Contactovisita> repositorio
            , IContactoServices contactoServices
            , IVisitaServices visitaServices)
        {
            _repositorio = repositorio;
            _contactoServices = contactoServices;
            _visitaServices = visitaServices;
        }

        public async Task<Contactovisita?> ContactoVisitaPorVisita(int secVisita)
        {
            var query = await _repositorio
                              .Obtener(x => x.SecVisita == secVisita);
            if (query != null)
            {
                query.SecContactoNavigation =
                    await _contactoServices
                          .ContactoPorSecuencial(query.SecContacto);
            }
            return query;
        }

        public async Task<Contactovisita?> ProcesaGuardarContactoVisita(Contactovisita entidad)
        {
            Contactovisita? registroGuardado = null;

            if (await _repositorio.Obtener(x => x.SecVisita == entidad.SecVisita) == null)
            {
                registroGuardado = await _repositorio.Crear(entidad);

                registroGuardado
                    .SecContactoNavigation
                     = await _contactoServices
                     .ContactoPorSecuencial(entidad.SecContacto);
            }

            if (registroGuardado == null)
            {
                var registro
                    = _repositorio
                    .Consultar(x =>
                               x.SecVisita == entidad.SecVisita)
                    .Result
                    .Include(x => x.SecContactoNavigation)
                    .FirstOrDefault();

                if (registro != null)
                {    
                    var contactoSeleccionado = 
                    _contactoServices.ContactoPorSecuencial(entidad.SecContacto)
                    ?? throw new TaskCanceledException($"Registro Secuencial Contacto {entidad.SecContacto} No Existe");

                    registro.SecContacto = entidad.SecContacto;
                    registro.SecContactoNavigation = await contactoSeleccionado;

                    if (await _repositorio.Editar(registro))
                        registroGuardado = registro;
                }
            }

            return registroGuardado;
        }

        public async Task<Contactovisita> CrearContactoVisita(Contactovisita entidad)
        {
            if (await _repositorio.Obtener(x => x.SecVisita == entidad.SecVisita && x.SecContacto == entidad.SecContacto) != null)
                throw new TaskCanceledException($"Error Nombre Contacto Ya Registrado");

            entidad.EstaActivo = 1;

            var regitroGuardado = await _repositorio.Crear(entidad);


            regitroGuardado
                .SecContactoNavigation
                 = await _contactoServices
                 .ContactoPorSecuencial(entidad.SecContacto);


            return regitroGuardado;
        }

        public async Task<Contactovisita> EditarContactoVisita(Contactovisita entidad)
        {
            var registro
                = await _repositorio.Obtener(x =>
                                             x.SecVisita == entidad.SecVisita &&
                                             x.SecContacto == entidad.SecContacto)
                  ?? throw new TaskCanceledException("Registro No Existe");

            if (registro != null)
            {
                registro.SecContacto = entidad.SecContacto;
                var regitroGuardado = await _repositorio.Editar(registro);
            }

            var obtenido
                = await _repositorio
                        .Obtener(x => x.Secuencial == entidad.Secuencial);

            obtenido.SecContactoNavigation
                = await _contactoServices
                        .ContactoPorSecuencial(entidad.SecContacto);

            return obtenido;
        }

        public async Task<bool> EliminarContactoVisita(int secuencial)
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

        public async Task<List<Contactovisita>> ListaContactoVisita()
        {
            var query = await _repositorio.Consultar();
            var queryIncludes = query.Include(x => x.SecContactoNavigation)
                                     .ToList();
            return [.. queryIncludes];
        }

        public async Task<List<Contactovisita>> ListaPorContacto(int secContacto)
        {
            var query = await _repositorio.Consultar();
            var queryIncludes = query
                                .Where(a => a.SecContacto == secContacto)
                                .Include(x => x.SecContactoNavigation)
                                .ToList();
            return queryIncludes;
        }
    }
}

using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;

namespace BLL.Implementacion
{
    public class ContactoVisitaServices : IContactoVisitaServices
    {
        private readonly IGenericRepository<Contactovisita> _repositorio;
        private readonly IContactoServices _contactoServices;

        public ContactoVisitaServices(IGenericRepository<Contactovisita> repositorio, IContactoServices contactoServices)
        {
            _repositorio = repositorio;
            _contactoServices = contactoServices;
        }

        public async Task<Contactovisita?> ContactoVisitaPorVisita(int secVisita)
        {
            var contactoVisita = await _repositorio.Obtener(x => x.SecVisita == secVisita);
            if (contactoVisita != null)
            {
                contactoVisita.SecContactoNavigation = await _contactoServices.ContactoPorSecuencial(contactoVisita.SecContacto);
            }
            return contactoVisita;
        }

        public async Task<Contactovisita?> ProcesaGuardarContactoVisita(Contactovisita entidad)
        {
            var contactoAsociado = await _contactoServices.ContactoPorSecuencial(entidad.SecContacto)
                                   ?? throw new TaskCanceledException($"El contacto con Secuencial {entidad.SecContacto} no existe.");

            var registroExistente = await _repositorio.Obtener(x => x.SecVisita == entidad.SecVisita);

            if (registroExistente == null)
            {
                // Crear
                entidad.EstaActivo = 1;
                var nuevoRegistro = await _repositorio.Crear(entidad);
                nuevoRegistro.SecContactoNavigation = contactoAsociado;
                return nuevoRegistro;
            }
            else
            {
                // Editar
                registroExistente.SecContacto = entidad.SecContacto;
                bool seEdito = await _repositorio.Editar(registroExistente);
                if (seEdito)
                {
                    registroExistente.SecContactoNavigation = contactoAsociado;
                    return registroExistente;
                }
            }
            return null;
        }

        public async Task<Contactovisita> CrearContactoVisita(Contactovisita entidad)
        {
            if (await _repositorio.Obtener(x => x.SecVisita == entidad.SecVisita && x.SecContacto == entidad.SecContacto) != null)
                throw new TaskCanceledException("El contacto ya está registrado para esta visita.");

            entidad.EstaActivo = 1;
            var registroGuardado = await _repositorio.Crear(entidad);
            registroGuardado.SecContactoNavigation = await _contactoServices.ContactoPorSecuencial(entidad.SecContacto);
            return registroGuardado;
        }

        public async Task<Contactovisita> EditarContactoVisita(Contactovisita entidad)
        {
            var registro = await _repositorio.Obtener(x => x.Secuencial == entidad.Secuencial)
                           ?? throw new TaskCanceledException("El registro de ContactoVisita no existe.");

            registro.SecContacto = entidad.SecContacto;
            await _repositorio.Editar(registro);

            registro.SecContactoNavigation = await _contactoServices.ContactoPorSecuencial(entidad.SecContacto);
            return registro;
        }

        public async Task<bool> EliminarContactoVisita(int secuencial)
        {
            var registro = await _repositorio.Obtener(x => x.Secuencial == secuencial);
            if (registro == null)
            {
                return false;
            }
            return await _repositorio.Eliminar(registro);
        }

        public async Task<List<Contactovisita>> ListaContactoVisita()
        {
            var query = await _repositorio.Consultar();
            return await query.Include(x => x.SecContactoNavigation).ToListAsync();
        }

        public async Task<List<Contactovisita>> ListaPorContacto(int secContacto)
        {
            var query = await _repositorio.Consultar(x => x.SecContacto == secContacto);
            return await query.Include(x => x.SecContactoNavigation).ToListAsync();
        }
    }
}

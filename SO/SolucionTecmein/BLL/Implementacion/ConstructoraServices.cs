using BLL.Interfaces;
using DAL.Interfaces;
using Entity;

namespace BLL.Implementacion
{
    public class ConstructoraServices : IConstructoraServices
    {
        private readonly IGenericRepository<Constructora> _repositorio;

        public ConstructoraServices(IGenericRepository<Constructora> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<List<Constructora>> Lista()
        {
            var query = await _repositorio.Consultar();
            return [.. query];
        }
        public async Task<Constructora> ConstructoraPorSecuencial(int secuencial)
        {
            var query = await _repositorio.Obtener(x=> x.Secuencial == secuencial);
            return query;
        }

        public async Task<Constructora> GuardarCambios(Constructora entidad)
        {
            if (await _repositorio.Obtener(x => x.Nombre == entidad.Nombre) != null)
                throw new TaskCanceledException($"Error Nombre Constructora Ya Registrada");

            var regitroGuardado = await _repositorio.Crear(entidad);

            return regitroGuardado;
            
        }

        public async Task<Constructora> Editar(Constructora entidad)
        {
            var registro 
                = await _repositorio.Obtener(x => x.Secuencial == entidad.Secuencial)
                  ?? throw new TaskCanceledException("Registro No Existe");

            if (registro != null){

                registro.Nombre = entidad.Nombre;
                registro.Direccion = entidad.Direccion;
                registro.Telefono = entidad.Telefono;   
                registro.Correo = entidad.Correo;   
                registro.Atencion = entidad.Atencion;
                registro.Administrador = entidad.Administrador; 
                registro.TelefonoAdministrador = entidad.TelefonoAdministrador;
                registro.CorreoAdministrador = entidad.CorreoAdministrador;
                registro.EstaActivo = entidad.EstaActivo;
                var regitroGuardado = await _repositorio.Editar(entidad);

            }
            return await _repositorio.Obtener(x=> x.Secuencial == entidad.Secuencial);
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
    }
}

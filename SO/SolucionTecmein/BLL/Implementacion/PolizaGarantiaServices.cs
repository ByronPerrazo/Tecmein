using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;

namespace BLL.Implementacion
{
    public class PolizaGarantiaServices : IPolizaGarantiaServices
    {
        private readonly IGenericRepository<PolizaGarantia> _repositorio;

        public PolizaGarantiaServices(IGenericRepository<PolizaGarantia> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<List<PolizaGarantia>> Lista()
        {
            IQueryable<PolizaGarantia> query = await _repositorio.Consultar();
            return await query.Where(p => p.EstaActivo == true).ToListAsync();
        }

        public async Task<PolizaGarantia> Crear(PolizaGarantia entidad)
        {
            if (entidad == null) throw new ArgumentNullException(nameof(entidad));

            entidad.EstaActivo = true;
            var polizaCreada = await _repositorio.Crear(entidad);
            if (polizaCreada.Secuencial == 0)
            {
                throw new Exception("No se pudo crear la póliza de garantía.");
            }
            return polizaCreada;
        }

        public async Task<PolizaGarantia> Editar(PolizaGarantia entidad)
        {
            if (entidad == null) throw new ArgumentNullException(nameof(entidad));

            var polizaExistente = await _repositorio.Obtener(p => p.Secuencial == entidad.Secuencial);
            if (polizaExistente == null)
            {
                throw new Exception("La póliza de garantía no existe.");
            }

            polizaExistente.Descripcion = entidad.Descripcion;
            polizaExistente.EstaActivo = entidad.EstaActivo;

            bool seEdito = await _repositorio.Editar(polizaExistente);
            if (!seEdito)
            {
                throw new Exception("No se pudo editar la póliza de garantía.");
            }
            return polizaExistente;
        }

        public async Task<bool> Eliminar(int secuencial)
        {
            var poliza = await _repositorio.Obtener(p => p.Secuencial == secuencial);
            if (poliza == null)
            {
                throw new Exception("La póliza de garantía no existe.");
            }

            poliza.EstaActivo = false; // Soft delete
            bool seElimino = await _repositorio.Editar(poliza);
            return seElimino;
        }
    }
}
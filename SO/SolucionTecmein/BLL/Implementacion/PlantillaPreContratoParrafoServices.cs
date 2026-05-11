using BLL.Interfaces;
using DAL.Interfaces;
using Entity;

namespace BLL.Implementacion
{
    public class PlantillaPreContratoParrafoServices : IPlantillaPreContratoParrafoServices
    {
        private readonly IGenericRepository<PlantillaPreContratoParrafo> _repositorio;

        public PlantillaPreContratoParrafoServices(IGenericRepository<PlantillaPreContratoParrafo> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<List<PlantillaPreContratoParrafo>> Lista(int secPlantillaPreContrato)
        {
            IQueryable<PlantillaPreContratoParrafo> query = await _repositorio.Consultar(p => p.SecPlantillaPreContrato == secPlantillaPreContrato);
            return query.ToList();
        }

        public async Task<PlantillaPreContratoParrafo> Obtener(int secPlantillaPreContratoParrafo)
        {
            return await _repositorio.Obtener(p => p.SecPlantillaPreContratoParrafo == secPlantillaPreContratoParrafo);
        }

        public async Task<PlantillaPreContratoParrafo> Crear(PlantillaPreContratoParrafo entidad)
        {
            entidad.EstaActivo = true;
            return await _repositorio.Crear(entidad);
        }

        public async Task<PlantillaPreContratoParrafo> Editar(PlantillaPreContratoParrafo entidad)
        {
            var parrafoExistente = await _repositorio.Obtener(p => p.SecPlantillaPreContratoParrafo == entidad.SecPlantillaPreContratoParrafo);
            if (parrafoExistente == null) throw new Exception("El párrafo de plantilla de pre-contrato no existe.");

            parrafoExistente.SecPlantillaPreContrato = entidad.SecPlantillaPreContrato;
            parrafoExistente.Orden = entidad.Orden;
            parrafoExistente.Contenido = entidad.Contenido;
            parrafoExistente.EstaActivo = entidad.EstaActivo;

            bool resultado = await _repositorio.Editar(parrafoExistente);
            if (!resultado)
            {
                throw new Exception("No se pudo editar el párrafo.");
            }
            return parrafoExistente;
        }

        public async Task<bool> Eliminar(int secPlantillaPreContratoParrafo)
        {
            var parrafo = await _repositorio.Obtener(p => p.SecPlantillaPreContratoParrafo == secPlantillaPreContratoParrafo);
            if (parrafo == null) throw new Exception("El párrafo de plantilla de pre-contrato no existe.");
            return await _repositorio.Eliminar(parrafo);
        }
    }
}

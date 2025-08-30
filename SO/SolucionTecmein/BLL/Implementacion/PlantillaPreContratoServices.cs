using BLL.Interfaces;
using DAL.Interfaces;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Implementacion
{
    public class PlantillaPreContratoServices : IPlantillaPreContratoServices
    {
        private readonly IGenericRepository<PlantillaPreContrato> _repositorio;

        public PlantillaPreContratoServices(IGenericRepository<PlantillaPreContrato> repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<List<PlantillaPreContrato>> Lista()
        {
            IQueryable<PlantillaPreContrato> query = await _repositorio.Consultar();
            return query.ToList();
        }

        public async Task<PlantillaPreContrato> Obtener(int secPlantillaPreContrato)
        {
            return await _repositorio.Obtener(p => p.SecPlantillaPreContrato == secPlantillaPreContrato);
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

            await _repositorio.Editar(plantillaExistente);
            return plantillaExistente;
        }

        public async Task<bool> Eliminar(int secPlantillaPreContrato)
        {
            var plantilla = await _repositorio.Obtener(p => p.SecPlantillaPreContrato == secPlantillaPreContrato);
            if (plantilla == null) throw new Exception("La plantilla de pre-contrato no existe.");
            return await _repositorio.Eliminar(plantilla);
        }
    }
}

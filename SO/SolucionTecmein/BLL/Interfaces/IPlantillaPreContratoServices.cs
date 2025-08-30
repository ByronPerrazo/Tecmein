using Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IPlantillaPreContratoServices
    {
        Task<List<PlantillaPreContrato>> Lista();
        Task<PlantillaPreContrato> Obtener(int secPlantillaPreContrato);
        Task<PlantillaPreContrato> Crear(PlantillaPreContrato entidad);
        Task<PlantillaPreContrato> Editar(PlantillaPreContrato entidad);
        Task<bool> Eliminar(int secPlantillaPreContrato);
    }
}
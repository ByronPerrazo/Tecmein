using Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IPlantillaPreContratoParrafoServices
    {
        Task<List<PlantillaPreContratoParrafo>> Lista(int secPlantillaPreContrato);
        Task<PlantillaPreContratoParrafo> Obtener(int secPlantillaPreContratoParrafo);
        Task<PlantillaPreContratoParrafo> Crear(PlantillaPreContratoParrafo entidad);
        Task<PlantillaPreContratoParrafo> Editar(PlantillaPreContratoParrafo entidad);
        Task<bool> Eliminar(int secPlantillaPreContratoParrafo);
    }
}
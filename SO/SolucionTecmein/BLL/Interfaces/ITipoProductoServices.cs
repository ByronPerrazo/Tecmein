using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ITipoProductoServices
    {
        Task<List<TipoProducto>> Lista();
        Task<TipoProducto> Crea(TipoProducto entidad);
        Task<TipoProducto> TipoProductoPorSecuencial(int secuencial);
        Task<TipoProducto> Editar(TipoProducto entidad);
        Task<bool> Eliminar(int secuencial);
    }
}

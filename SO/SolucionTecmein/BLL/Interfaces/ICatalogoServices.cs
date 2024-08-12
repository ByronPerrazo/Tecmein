using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ICatalogoServices
    {
        Task<List<Catalogo>> Lista();
        Task<List<Catalogo>> CatalogosPorRol(int secuencialRol);
        Task<Catalogo> CatalogoPorSecuencial(int secuencialCatalogo);
        Task<Catalogo> Crear(Catalogo entidad, Stream? archivo = null, string nombreArchivo = "");
        Task<Catalogo> Editar(Catalogo entidad, Stream? archivo = null, string nombreArchivo = "");
        Task<bool> Eliminar(int secuencial);
    }
}

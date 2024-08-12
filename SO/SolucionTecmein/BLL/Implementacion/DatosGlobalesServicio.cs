using BLL.Interfaces;

namespace BLL.Implementacion
{
    public class DatosGlobalesServicio : IDatosGlobalesServices
    {
        private string _pathCatalogo = "Catalogos_PDF";

        private int _secuencialEmpresa = 1;


        public string PathCatalogos => _pathCatalogo;
        public int SecuencialEmpresaPrincipal => _secuencialEmpresa;
    }
}

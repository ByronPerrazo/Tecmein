using System.Collections.Generic;
using TecmeinAplicacionWeb.Models.ViewModels;

namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class UsuarioEditarVM
    {
        public UsuarioVM Usuario { get; set; }
        public List<RolVM> ListaRoles { get; set; }
    }
}

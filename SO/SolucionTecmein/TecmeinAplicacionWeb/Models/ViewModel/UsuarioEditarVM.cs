using System.Collections.Generic;

namespace TecmeinWebApp.Models.ViewModel
{
    public class UsuarioEditarVM
    {
        public UsuarioVM Usuario { get; set; }
        public List<RolVM> ListaRoles { get; set; }
    }
}

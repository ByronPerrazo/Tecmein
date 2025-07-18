using System.Collections.Generic;

namespace TecmeinWebApp.Models.ViewModel
{
    public class MenuEditarVM
    {
        public MenuVM Menu { get; set; }
        public List<MenuVM> ListaMenusPadre { get; set; }
        public List<string> IconosDisponibles { get; set; }
    }
}

using System.Collections.Generic;

namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class MenuEditarVM
    {
        public MenuVM Menu { get; set; }
        public List<MenuVM> ListaMenusPadre { get; set; }
        public List<string> IconosDisponibles { get; set; }
    }
}

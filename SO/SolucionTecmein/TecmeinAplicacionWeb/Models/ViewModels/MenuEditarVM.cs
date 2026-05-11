namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class MenuEditarVM
    {
        public MenuVM Menu { get; set; } = new();
        public List<MenuVM> ListaMenusPadre { get; set; } = new();
        public List<string> IconosDisponibles { get; set; } = new();
    }
}

namespace AplicacionWeb.Models.ViewModel
{
    public class MenuVM
    {
        public string Descripcion { get; set; } = null!;


        public string Icono { get; set; } = null!;

        public string Controlador { get; set; } = null!;

        public int PaginaAccion { get; set; }

        public bool EstaActivo { get; set; }

        public virtual ICollection<MenuVM>? SubMenu { get; set; }

    }
}

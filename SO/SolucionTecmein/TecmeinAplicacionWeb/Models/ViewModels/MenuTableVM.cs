namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class MenuTableVM
    {
        public int Secuencial { get; set; }
        public string? Descripcion { get; set; }
        public string? DescripcionMenuPadre { get; set; }
        public string? Icono { get; set; }
        public string? Controlador { get; set; }
        public string? PaginaAccion { get; set; }
        public short? EsActivo { get; set; }
        public bool MostrarEnMenu { get; set; }
    }
}
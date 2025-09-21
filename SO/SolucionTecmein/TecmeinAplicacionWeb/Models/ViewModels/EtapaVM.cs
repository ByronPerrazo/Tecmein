namespace TecmeinAplicacionWeb.Models.ViewModels
{
    public class EtapaVM
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public int Orden { get; set; }
        public bool EstaActivo { get; set; }
    }
}

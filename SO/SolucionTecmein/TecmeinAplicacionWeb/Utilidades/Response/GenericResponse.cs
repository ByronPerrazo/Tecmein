namespace TecmeinWebApp.Utilidades.Response
{
    public class GenericResponse<TObject>
    {
        public bool Estado { get; set; }
        public string? Mensajes { get; set; }
        public TObject? Objeto { get; set; }
        public List<TObject>? ListaObjeto { get; set; }
    }
}

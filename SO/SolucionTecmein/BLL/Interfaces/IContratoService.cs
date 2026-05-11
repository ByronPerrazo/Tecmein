using BLL.DTOs;
using Entity;

namespace BLL.Interfaces
{
    public interface IContratoService
    {
        Task<List<Contrato>> Listar();
        Task<Contrato> Crear(ContratoCreacionDTO dto);
        Task<Contrato> Editar(Contrato entidad, string nombreProyecto, Stream archivoStream, string nombreArchivo);
        Task<Contrato> Obtener(int id);
        Task<bool> Eliminar(int id);

        Task<Contrato> ObtenerParaEdicion(int idContrato);

        // Método para obtener los pre-contratos que pueden convertirse en contrato
        Task<List<PreContrato>> ListarPreContratosParaContrato();

        // Nuevo método para obtener contratos por cliente
        Task<List<Contrato>> ObtenerContratosPorCliente(int secCliente);
    }
}

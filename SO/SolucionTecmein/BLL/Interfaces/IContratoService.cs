using BLL.DTOs;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IContratoService
    {
        Task<List<Contrato>> Listar();
        Task<Contrato> Crear(Contrato entidad, Stream archivoStream = null, string nombreArchivo = "");
        Task<bool> Editar(Contrato entidad);
        Task<Contrato> Obtener(int id);
        Task<bool> Eliminar(int id);

        // Método para obtener los pre-contratos que pueden convertirse en contrato
        Task<List<PreContrato>> ListarPreContratosParaContrato();
    }
}

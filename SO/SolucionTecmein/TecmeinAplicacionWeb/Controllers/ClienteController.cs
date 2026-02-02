using AutoMapper;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TecmeinAplicacionWeb.Models.ViewModels;
using System.Text.Json;
using System.Text.Json.Serialization;
using TecmeinWebApp.Utilidades.Response;
using TecmeinWebApp.Utilidades.ViewComponents;

namespace TecmeinAplicacionWeb.Controllers
{
    public class ClienteController : Controller
    {
        private readonly IClienteServices _clienteService;
        private readonly IMapper _mapper;
        private readonly IConstructoraServices _constructoraServices; // Nuevo

        public ClienteController(IClienteServices clienteService, IMapper mapper, IConstructoraServices constructoraServices)
        {
            _clienteService = clienteService;
            _mapper = mapper;
            _constructoraServices = constructoraServices; // Asignación
        }

        [ValidatePermission("VER_MENU")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> Lista()
        {
            var gResponse = new GenericResponse<List<ClienteVM>>();
            try
            {
                var listaClientes = await _clienteService.Listar();
                var listaVm = _mapper.Map<List<ClienteVM>>(listaClientes);
                gResponse.Estado = true;
                gResponse.Objeto = listaVm;
            }
            catch (System.Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }
            return StatusCode(200, gResponse);
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> ListarConstructoras()
        {
            var gResponse = new GenericResponse<List<ConstructoraVM>>();
            try
            {
                var todasConstructoras = await _constructoraServices.Lista();
                var todosClientes = await _clienteService.Listar();

                var constructorasConCliente = new HashSet<int>(todosClientes.Select(c => c.SecConstructora));

                var constructorasNoClientes = todasConstructoras
                    .Where(c => !constructorasConCliente.Contains(c.Secuencial))
                    .ToList();

                gResponse.Estado = true;
                gResponse.Objeto = _mapper.Map<List<ConstructoraVM>>(constructorasNoClientes);
            }
            catch (System.Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }
            return StatusCode(200, gResponse);
        }

        [HttpPost]
        [ValidatePermission("CREAR")]
        public async Task<IActionResult> Crear([FromBody] ClienteVM vmCliente)
        {
            var gResponse = new GenericResponse<ClienteVM>();
            try
            {
                var cliente = _mapper.Map<Entity.Cliente>(vmCliente);
                var clienteCreado = await _clienteService.Crear(cliente);
                gResponse.Estado = true;
                gResponse.Objeto = _mapper.Map<ClienteVM>(clienteCreado);
            }
            catch (System.Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }
            return StatusCode(200, gResponse);
        }

        [HttpPut]
        [ValidatePermission("ACTUALIZAR")]
        public async Task<IActionResult> Editar([FromBody] ClienteVM vmCliente)
        {
            var gResponse = new GenericResponse<ClienteVM>();
            try
            {
                var cliente = _mapper.Map<Entity.Cliente>(vmCliente);
                var clienteEditado = await _clienteService.Editar(cliente);
                gResponse.Estado = true;
                gResponse.Objeto = _mapper.Map<ClienteVM>(clienteEditado);
            }
            catch (System.Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }
            return StatusCode(200, gResponse);
        }

        [HttpDelete]
        [ValidatePermission("ELIMINAR")]
        public async Task<IActionResult> Eliminar(int secCliente)
        {
            var gResponse = new GenericResponse<string>();
            try
            {
                // Antes de eliminar, verificar si el cliente tiene contratos asociados
                var contratos = await _clienteService.ObtenerContratosPorCliente(secCliente); // Necesitamos este método en IClienteServices
                if (contratos != null && contratos.Any())
                {
                    gResponse.Estado = false;
                    gResponse.Mensajes = "No se puede eliminar el cliente porque tiene contratos asociados.";
                    return StatusCode(StatusCodes.Status400BadRequest, gResponse);
                }

                var resultado = await _clienteService.Eliminar(secCliente);
                gResponse.Estado = resultado;
            }
            catch (System.Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }
            return StatusCode(200, gResponse);
        }

        [HttpGet]
        public async Task<IActionResult> Obtener(int idConstructora)
        {
            var gResponse = new GenericResponse<ConstructoraVM>();
            try
            {
                var constructora = await _constructoraServices.ConstructoraPorSecuencial(idConstructora);
                if (constructora != null)
                {
                    gResponse.Estado = true;
                    gResponse.Objeto = _mapper.Map<ConstructoraVM>(constructora);
                }
                else
                {
                    gResponse.Estado = false;
                    gResponse.Mensajes = "No se encontró la constructora";
                }
            }
            catch (System.Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }
            return StatusCode(200, gResponse);
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> ListaClientesActivos()
        {
            try
            {
                var lista = await _clienteService.Listar(); // Asumo que Listar() ya filtra por activos
                var clientesActivos = lista.Select(c => new { value = c.SecCliente, text = c.SecConstructoraNavigation.Nombre }).ToList(); // Asumo que Cliente tiene una propiedad Nombre
                return StatusCode(StatusCodes.Status200OK, new { data = clientesActivos });
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = ex.Message });
            }
        }
    }
}
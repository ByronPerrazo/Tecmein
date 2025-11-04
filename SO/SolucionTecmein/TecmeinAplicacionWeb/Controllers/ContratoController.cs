using Microsoft.AspNetCore.Mvc;
using BLL.Interfaces;
using Entity;
using System.Threading.Tasks;
using TecmeinAplicacionWeb.Models.ViewModels;
using AutoMapper;
using System.Collections.Generic;
using System.Security.Claims;
using Newtonsoft.Json;
using TecmeinWebApp.Utilidades.Response;
using TecmeinWebApp.Utilidades.ViewComponents;
using System.Globalization;
using BLL.DTOs;
using Microsoft.EntityFrameworkCore;

namespace TecmeinAplicacionWeb.Controllers
{
    public class ContratoController : Controller
    {
        private readonly IContratoService _contratoService;
        private readonly IMapper _mapper;
        private readonly IPreContratoServices _preContratoServices;
        private readonly IConstructoraServices _constructoraServices;
        private readonly IPlanDePagoService _planDePagoService; // New field declaration

        public ContratoController(IContratoService contratoService, IMapper mapper, IPreContratoServices preContratoServices, IConstructoraServices constructoraServices, IPlanDePagoService planDePagoService)
        {
            _contratoService = contratoService;
            _mapper = mapper;
            _preContratoServices = preContratoServices;
            _constructoraServices = constructoraServices;
            _planDePagoService = planDePagoService; // New field
        }

        [ValidatePermission("VER_MENU")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> Listar()
        {
            var lista = await _contratoService.Listar();
            List<ContratoVM> vmLista = new List<ContratoVM>();

            foreach (var contrato in lista)
            {
                ContratoVM vm = _mapper.Map<ContratoVM>(contrato);

                // Determinar si proviene de un pre-contrato
                if (contrato.IdCotizacion != 0)
                {
                    var preContrato = await _preContratoServices.ObtenerUltimaVersion(contrato.IdCotizacion);
                    vm.ProvieneDePreContrato = preContrato != null;
                }

                // Obtener el nombre del cliente
                if (contrato.SecCliente != 0)
                {
                    var constructora = await _constructoraServices.ConstructoraPorSecuencial(contrato.SecCliente);
                    vm.NombreCliente = constructora?.Nombre;
                }
                vmLista.Add(vm);
            }
            return StatusCode(StatusCodes.Status200OK, new { data = vmLista });
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> ListarPreContratosParaContrato()
        {
            var gResponse = new GenericResponse<List<PreContratoVM>>();
            try
            {
                var listaPreContratos = await _contratoService.ListarPreContratosParaContrato();
                var vmLista = _mapper.Map<List<PreContratoVM>>(listaPreContratos);

                gResponse.Estado = true;
                gResponse.Objeto = vmLista;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> ObtenerDetalles(int id)
        {
            var gResponse = new GenericResponse<ContratoVM>();
            try
            {
                Contrato contrato = await _contratoService.ObtenerParaEdicion(id);
                if (contrato == null)
                {
                    gResponse.Estado = false;
                    gResponse.Mensajes = "Contrato no encontrado";
                    return StatusCode(StatusCodes.Status404NotFound, gResponse);
                }

                ContratoVM vm = _mapper.Map<ContratoVM>(contrato);

                // Mapeo manual de campos que no están en el mapeo automático
                vm.SecCliente = contrato.SecCliente;
                vm.ProvieneDePreContrato = false;
                // Determinar si proviene de un pre-contrato aprobado
                if (contrato.IdCotizacion != 0 )
                {
                    var preContrato = await _preContratoServices.ObtenerUltimaVersion(contrato.IdCotizacion);
                    vm.ProvieneDePreContrato =  preContrato != null ;
                }

                // Obtener el nombre del cliente
                if (contrato.SecCliente != 0)
                {
                    var constructora = await _constructoraServices.ConstructoraPorSecuencial(contrato.SecCliente);
                    vm.NombreCliente = constructora?.Nombre;
                }
               
                gResponse.Estado = true;
                gResponse.Objeto = vm;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, gResponse);
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }


        [HttpPost]
        [ValidatePermission("CREAR")]
        public async Task<IActionResult> Crear([FromForm] string modelo, [FromForm] IFormFile archivo)
        {
            var gResponse = new GenericResponse<ContratoVM>();
            try
            {
                var vmContrato = JsonConvert.DeserializeObject<ContratoVM>(modelo);
                var gCurrentUser = HttpContext.User;
                string idUsuario = gCurrentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (archivo == null)
                {
                    throw new Exception("El archivo del contrato es obligatorio.");
                }

                var creacionDto = new BLL.DTOs.ContratoCreacionDTO
                {
                    IdCotizacion = vmContrato.IdCotizacion > 0 ? vmContrato.IdCotizacion : null,
                    SecCliente = vmContrato.SecCliente > 0 ? vmContrato.SecCliente : null,
                    NombreProyecto = vmContrato.NombreProyecto, // Añadido para pasar el nombre del proyecto
                    FechaFirma = DateTime.ParseExact(vmContrato.FechaFirma, "dd/MM/yyyy", new CultureInfo("es-ES")),
                    IdUsuarioCarga = int.Parse(idUsuario),
                    ArchivoStream = archivo.OpenReadStream(),
                    NombreArchivo = archivo.FileName
                };

                Contrato contratoCreado = await _contratoService.Crear(creacionDto);

                gResponse.Estado = true;
                gResponse.Objeto = _mapper.Map<ContratoVM>(contratoCreado);

                return StatusCode(StatusCodes.Status200OK, gResponse);
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, gResponse);
            }
        }


        [HttpPut]
        [ValidatePermission("ACTUALIZAR")]
        public async Task<IActionResult> Editar([FromForm] string modelo, [FromForm] IFormFile? archivo)
        {
            var gResponse = new GenericResponse<ContratoVM>();
            try
            {
                var vmContrato = JsonConvert.DeserializeObject<ContratoVM>(modelo);
                Contrato contrato = _mapper.Map<Contrato>(vmContrato);

                Stream? streamArchivo = archivo?.OpenReadStream();
                string nombreArchivo = archivo?.FileName ?? string.Empty;

                Contrato contratoEditado = await _contratoService.Editar(contrato, vmContrato.NombreProyecto, streamArchivo, nombreArchivo);

                gResponse.Estado = true;
                gResponse.Objeto = _mapper.Map<ContratoVM>(contratoEditado);

                return StatusCode(StatusCodes.Status200OK, gResponse);
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, gResponse);
            }
        }

        [HttpDelete]
        [ValidatePermission("ELIMINAR")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var gResponse = new GenericResponse<string>();
            try
            {
                gResponse.Estado = await _contratoService.Eliminar(id);
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerCompromisosDePagoPreContrato(int idCotizacion)
        {
            var gResponse = new GenericResponse<List<PreContratoCompromisoPagoVM>>();
            try
            {
                var preContrato = await _preContratoServices.ObtenerUltimaVersion(idCotizacion);

                if (preContrato == null)
                {
                    gResponse.Estado = false;
                    gResponse.Mensajes = "No se encontró un pre-contrato asociado a la cotización.";
                    return StatusCode(StatusCodes.Status404NotFound, gResponse);
                }

                gResponse.Estado = true;
                gResponse.Objeto = _mapper.Map<List<PreContratoCompromisoPagoVM>>(preContrato.PreContratoCompromisoPagos);
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, gResponse);
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerPlanDePagoPorContrato(int idContrato)
        {
            var gResponse = new GenericResponse<PlanDePagoVM>();
            try
            {
                var planDePagoDTO = await _planDePagoService.ObtenerPorContratoId(idContrato);

                if (planDePagoDTO == null)
                {
                    gResponse.Estado = false;
                    gResponse.Mensajes = "No se encontró un plan de pago para el contrato especificado.";
                    return StatusCode(StatusCodes.Status404NotFound, gResponse);
                }

                gResponse.Estado = true;
                gResponse.Objeto = _mapper.Map<PlanDePagoVM>(planDePagoDTO);
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, gResponse);
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarPlanDePago([FromBody] PlanDePagoVM modelo)
        {
            var gResponse = new GenericResponse<PlanDePagoVM>();
            try
            {
                var planDePagoDto = _mapper.Map<PlanDePagoDTO>(modelo);
                var planGuardado = await _planDePagoService.Guardar(planDePagoDto);
                gResponse.Estado = true;
                gResponse.Objeto = _mapper.Map<PlanDePagoVM>(planGuardado);
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
    }
}

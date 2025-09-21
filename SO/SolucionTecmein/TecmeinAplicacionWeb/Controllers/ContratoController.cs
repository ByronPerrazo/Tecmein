using Microsoft.AspNetCore.Mvc;
using BLL.Interfaces;
using Entity;
using System.Threading.Tasks;
using TecmeinAplicacionWeb.Models.ViewModels;
using AutoMapper;
using System.Collections.Generic;
using System.Security.Claims;
using Newtonsoft.Json;
using System.IO;

namespace TecmeinAplicacionWeb.Controllers
{
    public class ContratoController : Controller
    {
        private readonly IContratoService _contratoService;
        private readonly IMapper _mapper;

        public ContratoController(IContratoService contratoService, IMapper mapper)
        {
            _contratoService = contratoService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var lista = await _contratoService.Listar();
            List<VMContrato> vmLista = _mapper.Map<List<VMContrato>>(lista);
            return StatusCode(StatusCodes.Status200OK, new { data = vmLista });
        }

        [HttpGet]
        public async Task<IActionResult> ListarPreContratos()
        {
            var lista = await _contratoService.ListarPreContratosParaContrato();
            // Aquí se podría mapear a un ViewModel (VM) si fuese necesario para la vista.
            return StatusCode(StatusCodes.Status200OK, new { data = lista });
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] string modelo, [FromForm] IFormFile archivo)
        {
            try
            {
                var gCurrentUser = HttpContext.User;
                string idUsuario = gCurrentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                VMContrato vmContrato = JsonConvert.DeserializeObject<VMContrato>(modelo);
                vmContrato.IdUsuarioCarga = int.Parse(idUsuario);

                Contrato contrato = _mapper.Map<Contrato>(vmContrato);
                contrato.FechaCreacion = DateTime.Now; // Asignar la fecha de creación

                Stream streamArchivo = null;
                string nombreArchivo = "";

                if (archivo != null)
                {
                    streamArchivo = archivo.OpenReadStream();
                    nombreArchivo = archivo.FileName;
                }

                Contrato contrato_creado = await _contratoService.Crear(contrato, streamArchivo, nombreArchivo);

                vmContrato = _mapper.Map<VMContrato>(contrato_creado);

                return StatusCode(StatusCodes.Status200OK, new { success = true, data = vmContrato });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { success = false, message = ex.Message });
            }
        }


        [HttpPut]
        public async Task<IActionResult> Editar([FromForm] string modelo, [FromForm] IFormFile? archivo)
        {
            // Lógica para editar, verificando permisos de administrador
            return StatusCode(StatusCodes.Status200OK, new { success = true, message = "Contrato editado" });
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int id)
        {
            var resultado = await _contratoService.Eliminar(id);
            return StatusCode(StatusCodes.Status200OK, new { success = resultado });
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using BLL.Interfaces;
using Entity;
using System.Threading.Tasks;
using System.Collections.Generic;
using TecmeinAplicacionWeb.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using AutoMapper;
using System;

namespace TecmeinAplicacionWeb.Controllers
{
    public class PlantillaPreContratoController : Controller
    {
        private readonly IPlantillaPreContratoServices _plantillaPreContratoServices;
        private readonly IPlantillaPreContratoParrafoServices _plantillaPreContratoParrafoServices;
        private readonly ITipoDocumentoServices _tipoDocumentoServices;
        private readonly IPreContratoServices _preContratoServices; // Injected
        private readonly IMapper _mapper;
        private readonly IGeneradorDocumentoService _generadorDocumentoService;

        public PlantillaPreContratoController(
            IPlantillaPreContratoServices plantillaPreContratoServices,
            IPlantillaPreContratoParrafoServices plantillaPreContratoParrafoServices,
            ITipoDocumentoServices tipoDocumentoServices,
            IPreContratoServices preContratoServices, // Injected
            IMapper mapper,
            IGeneradorDocumentoService generadorDocumentoService)
        {
            _plantillaPreContratoServices = plantillaPreContratoServices;
            _plantillaPreContratoParrafoServices = plantillaPreContratoParrafoServices;
            _tipoDocumentoServices = tipoDocumentoServices;
            _preContratoServices = preContratoServices; // Injected
            _mapper = mapper;
            _generadorDocumentoService = generadorDocumentoService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> Lista()
        {
            List<PlantillaPreContrato> listaEntidades = await _plantillaPreContratoServices.Lista();
            List<PlantillaPreContratoVM> listaVM = _mapper.Map<List<PlantillaPreContratoVM>>(listaEntidades);
            return Json(new { data = listaVM });
        }

        [HttpGet]
        public async Task<JsonResult> ListaTiposDocumento()
        {
            List<TipoDocumento> listaTipos = await _tipoDocumentoServices.Lista();
            List<SelectListItem> selectList = listaTipos.Select(td => new SelectListItem()
            {
                Text = td.Descripcion,
                Value = td.SecTipoDocumento.ToString()
            }).ToList();
            return Json(new { data = selectList });
        }

        [HttpPost]
        public async Task<JsonResult> Crear([FromBody] PlantillaPreContrato entidad)
        {
            bool resultado = true;
            try
            {
                PlantillaPreContrato plantilla_creada = await _plantillaPreContratoServices.Crear(entidad);
                if (plantilla_creada.SecPlantillaPreContrato == 0)
                {
                    resultado = false;
                }
            }
            catch
            {
                resultado = false;
            }
            return Json(new { resultado = resultado });
        }

        [HttpPut]
        public async Task<JsonResult> Editar([FromBody] PlantillaPreContrato entidad)
        {
            bool resultado = true;
            try
            {
                PlantillaPreContrato plantilla_editada = await _plantillaPreContratoServices.Editar(entidad);
                if (plantilla_editada.SecPlantillaPreContrato == 0)
                {
                    resultado = false;
                }
            }
            catch
            {
                resultado = false;
            }
            return Json(new { resultado = resultado });
        }

        [HttpDelete]
        public async Task<JsonResult> Eliminar(int SecPlantillaPreContrato)
        {
            bool resultado = true;
            try
            {
                resultado = await _plantillaPreContratoServices.Eliminar(SecPlantillaPreContrato);
            }
            catch
            {
                resultado = false;
            }
            return Json(new { resultado = resultado });
        }

        public IActionResult Parrafos(int id)
        {
            ViewBag.IdPlantilla = id;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GenerarDocumento(int secPreContrato)
        {
            try
            {
                PreContrato preContrato = await _preContratoServices.Obtener(secPreContrato);
                if (preContrato == null)
                {
                    return NotFound($"PreContrato con ID {secPreContrato} no encontrado.");
                }

                var datos = new
                {
                    SecPreContrato = preContrato.SecPreContrato,
                    FechaRegistro = preContrato.FechaRegistro.ToShortDateString(),
                    NombreObra = preContrato.SecCotizacionNavigation?.SecVisitaNavigation?.Nombre,
                    NombreCliente = preContrato.SecCotizacionNavigation?.SecVisitaNavigation?.SecEmpresaNavigation?.Nombre,
                    NombreUsuario = preContrato.SecUsuarioCreaNavigation?.Nombre
                };

                string codigoTipoDocumento = preContrato.SecPlantillaPreContratoNavigation.SecTipoDocumentoNavigation.Codigo;
                byte[] documentBytes = await _generadorDocumentoService.GenerarDocumento(codigoTipoDocumento, preContrato.SecPlantillaPreContrato, datos);

                return File(documentBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"precontrato_{secPreContrato}.docx");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al generar documento: {ex.Message}");
                return StatusCode(500, $"Error al generar el documento: {ex.Message}");
            }
        }
    }
}
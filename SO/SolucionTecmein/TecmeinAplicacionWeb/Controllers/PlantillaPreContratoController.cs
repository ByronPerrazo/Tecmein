using Microsoft.AspNetCore.Mvc;
using BLL.Interfaces;
using Entity;
using System.Threading.Tasks;
using System.Collections.Generic;
using TecmeinWebApp.Models.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using AutoMapper; // Added

namespace TecmeinAplicacionWeb.Controllers
{
    public class PlantillaPreContratoController : Controller
    {
        private readonly IPlantillaPreContratoServices _plantillaPreContratoServices;
        private readonly IPlantillaPreContratoParrafoServices _plantillaPreContratoParrafoServices;
        private readonly ITipoDocumentoServices _tipoDocumentoServices;
        private readonly IMapper _mapper; // Added

        public PlantillaPreContratoController(
            IPlantillaPreContratoServices plantillaPreContratoServices,
            IPlantillaPreContratoParrafoServices plantillaPreContratoParrafoServices,
            ITipoDocumentoServices tipoDocumentoServices,
            IMapper mapper) // Modified constructor
        {
            _plantillaPreContratoServices = plantillaPreContratoServices;
            _plantillaPreContratoParrafoServices = plantillaPreContratoParrafoServices;
            _tipoDocumentoServices = tipoDocumentoServices;
            _mapper = mapper; // Assigned
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> Lista()
        {
            List<PlantillaPreContrato> listaEntidades = await _plantillaPreContratoServices.Lista(); // Changed to entity list
            List<PlantillaPreContratoVM> listaVM = _mapper.Map<List<PlantillaPreContratoVM>>(listaEntidades); // Mapped to VM
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
    }
}
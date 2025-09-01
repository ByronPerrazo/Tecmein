using Microsoft.AspNetCore.Mvc;
using BLL.Interfaces;
using Entity;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TecmeinAplicacionWeb.Controllers
{
    public class PlantillaPreContratoController : Controller
    {
        private readonly IPlantillaPreContratoServices _plantillaPreContratoServices;
        private readonly IPlantillaPreContratoParrafoServices _plantillaPreContratoParrafoServices;

        public PlantillaPreContratoController(
            IPlantillaPreContratoServices plantillaPreContratoServices,
            IPlantillaPreContratoParrafoServices plantillaPreContratoParrafoServices)
        {
            _plantillaPreContratoServices = plantillaPreContratoServices;
            _plantillaPreContratoParrafoServices = plantillaPreContratoParrafoServices;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> Lista()
        {
            List<PlantillaPreContrato> lista = await _plantillaPreContratoServices.Lista();
            return Json(new { data = lista });
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
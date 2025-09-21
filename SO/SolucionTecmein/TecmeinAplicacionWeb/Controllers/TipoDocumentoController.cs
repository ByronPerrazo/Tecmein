using Microsoft.AspNetCore.Mvc;
using BLL.Interfaces;
using Entity;
using System.Threading.Tasks;
using System.Collections.Generic;
using TecmeinAplicacionWeb.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using AutoMapper;

namespace TecmeinAplicacionWeb.Controllers
{
    public class TipoDocumentoController : Controller
    {
        private readonly ITipoDocumentoServices _tipoDocumentoServices;
        private readonly IMapper _mapper;

        public TipoDocumentoController(ITipoDocumentoServices tipoDocumentoServices, IMapper mapper)
        {
            _tipoDocumentoServices = tipoDocumentoServices;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> Lista()
        {
            List<TipoDocumento> listaEntidades = await _tipoDocumentoServices.Lista();
            List<TipoDocumentoVM> listaVM = _mapper.Map<List<TipoDocumentoVM>>(listaEntidades);
            return Json(new { data = listaVM });
        }

        [HttpPost]
        public async Task<JsonResult> Crear([FromBody] TipoDocumentoVM model)
        {
            bool resultado = true;
            try
            {
                TipoDocumento entidad = _mapper.Map<TipoDocumento>(model);
                TipoDocumento tipoDocumento_creado = await _tipoDocumentoServices.Crear(entidad);
                if (tipoDocumento_creado.SecTipoDocumento == 0)
                {
                    resultado = false;
                }
            }
            catch (Exception ex)
            {
                return Json(new { resultado = false, mensaje = ex.Message });
            }
            return Json(new { resultado = resultado });
        }

        [HttpPut]
        public async Task<JsonResult> Editar([FromBody] TipoDocumentoVM model)
        {
            bool resultado = true;
            try
            {
                TipoDocumento entidad = _mapper.Map<TipoDocumento>(model);
                TipoDocumento tipoDocumento_editado = await _tipoDocumentoServices.Editar(entidad);
                if (tipoDocumento_editado.SecTipoDocumento == 0)
                {
                    resultado = false;
                }
            }
            catch (Exception ex)
            {
                return Json(new { resultado = false, mensaje = ex.Message });
            }
            return Json(new { resultado = resultado });
        }

        [HttpDelete]
        public async Task<JsonResult> Eliminar(int SecTipoDocumento)
        {
            bool resultado = true;
            try
            {
                resultado = await _tipoDocumentoServices.Eliminar(SecTipoDocumento);
            }
            catch
            {
                resultado = false;
            }
            return Json(new { resultado = resultado });
        }
    }
}
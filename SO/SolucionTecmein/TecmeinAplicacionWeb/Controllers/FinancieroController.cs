using AutoMapper;
using BLL.DTOs;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using TecmeinWebApp.Utilidades.Response;
using TecmeinWebApp.Utilidades.ViewComponents;
using TecmeinAplicacionWeb.Models.ViewModels; // Add this using for VM

namespace TecmeinAplicacionWeb.Controllers
{
    // Asegurarse de que el usuario tenga permiso para ver este menú
    public class FinancieroController : Controller
    {
        private readonly IPlanDePagoService _planDePagoService;
        private readonly IPagoService _pagoService; // Add dependency
        private readonly IMapper _mapper;

        public FinancieroController(
            IPlanDePagoService planDePagoService, 
            IPagoService pagoService, // Add dependency
            IMapper mapper)
        {
            _planDePagoService = planDePagoService;
            _pagoService = pagoService; // Add dependency
            _mapper = mapper;
        }

        [ValidatePermission("VER_MENU")]
        public IActionResult Index()
        {
            ViewBag.CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            return View();
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> ListaPlanesPago()
        {
            // This method in the service now returns a DTO. Map it to a VM
            var gResponse = new GenericResponse<List<PlanPagoDashboardVM>>(); 
            try
            {
                var planesDto = await _pagoService.ObtenerPlanesDePagoParaDashboard();
                gResponse.Objeto = _mapper.Map<List<PlanPagoDashboardVM>>(planesDto);
                gResponse.Estado = true;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }
            return StatusCode(200, gResponse);
        }

        [HttpGet]
        [ValidatePermission("LEER")]
        public async Task<IActionResult> ObtenerDetallePlanDePago(int idPlanDePago)
        {
            var gResponse = new GenericResponse<DetallePlanPagoVM>(); // Changed to VM
            try
            {
                var planDto = await _pagoService.ObtenerDetallePlanDePago(idPlanDePago);
                gResponse.Objeto = _mapper.Map<DetallePlanPagoVM>(planDto);
                gResponse.Estado = true;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
            }
            return StatusCode(200, gResponse);
        }
    }
}
using AutoMapper;
using BLL.DTOs;
using BLL.Interfaces;
using DAL.Interfaces; // For IStorageServices
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using TecmeinWebApp.Utilidades.Response;
using TecmeinWebApp.Utilidades.ViewComponents;
using TecmeinAplicacionWeb.Models.ViewModels;
using Entity;
using DAL.DBContext; // Added for TecmeindbContext
using Microsoft.EntityFrameworkCore; // Added for Include extension method

namespace TecmeinAplicacionWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagoController : ControllerBase
    {
        private readonly IPagoService _pagoService;
        private readonly IMapper _mapper;
        private readonly IStorageServices _storageService;
        private readonly TecmeindbContext _dbContext; // Changed from IGenericRepository<PlanDePago>

        public PagoController(
            IPagoService pagoService, 
            IMapper mapper, 
            IStorageServices storageService,
            TecmeindbContext dbContext) // Changed constructor parameter
        {
            _pagoService = pagoService;
            _mapper = mapper;
            _storageService = storageService;
            _dbContext = dbContext; // Assign dbContext
        }

        [HttpPost("Registrar")]
        public async Task<IActionResult> Registrar([FromForm] RegistrarPagoVM modelo)
        {
            var gResponse = new GenericResponse<PagoDTO>();
            try
            {
                var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
                var idUsuarioClaim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                if (idUsuarioClaim == null)
                {
                    gResponse.Estado = false;
                    gResponse.Mensajes = "No se pudo obtener el usuario de la sesión.";
                    return StatusCode(401, gResponse);
                }

                var pagoDto = _mapper.Map<PagoDTO>(modelo);
                pagoDto.RegistradoPorUsuarioId = int.Parse(idUsuarioClaim.Value);

                if (modelo.ComprobanteFile != null)
                {
                    // Fetch PlanDePago with includes for client number
                    var planDePago = await _dbContext.PlanesDePago
                                                .Include(p => p.IdContratoNavigation)
                                                    .ThenInclude(c => c.SecClienteNavigation)
                                                .FirstOrDefaultAsync(p => p.IdPlanDePago == modelo.IdPlanDePago);

                    if (planDePago == null) throw new TaskCanceledException("Plan de pago no encontrado para la ruta del archivo.");

                    string numeroCliente = planDePago.IdContratoNavigation?.SecClienteNavigation?.NumeroCliente ?? "Desconocido";
                    string numeroContrato = planDePago.IdContratoNavigation?.IdContrato.ToString() ?? "Desconocido";
                    string carpetaDestino = $"ComprobantesPago/{numeroCliente}/{numeroContrato}"; // Reconstructed path
                    string nombreArchivo = $"{Guid.NewGuid()}_{modelo.ComprobanteFile.FileName}";
                
                    using (var stream = modelo.ComprobanteFile.OpenReadStream())
                    {
                        string urlArchivo = await _storageService.SubirStorage(stream, carpetaDestino, nombreArchivo);
                        pagoDto.ComprobanteUrl = urlArchivo;
                        pagoDto.ComprobanteNombre = nombreArchivo;
                    }
                }

                PagoDTO resultado = await _pagoService.RegistrarPago(pagoDto);

                gResponse.Estado = true;
                gResponse.Objeto = resultado;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
                return StatusCode(500, gResponse);
            }
            return Ok(gResponse);
        }

        [HttpGet("ListarPorPlan/{idPlanDePago}")]
        public async Task<IActionResult> ListarPorPlanDePago(int idPlanDePago)
        {
            var gResponse = new GenericResponse<IEnumerable<PagoDTO>>();
            try
            {
                gResponse.Objeto = await _pagoService.ListarPorPlanDePago(idPlanDePago);
                gResponse.Estado = true;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensajes = ex.Message;
                return StatusCode(500, gResponse);
            }
            return Ok(gResponse);
        }
    }
}

using BLL.ContractEngine;
using BLL.Interfaces;
using DAL.DBContext;
using DocumentFormat.OpenXml.Wordprocessing;

namespace BLL.Implementacion.ContractEngine
{
    public class GeneralPlaceholderProvider : IPlaceholderProvider
    {
        private readonly TecmeindbContext _context;

        public GeneralPlaceholderProvider(TecmeindbContext context)
        {
            _context = context;
        }

        public Task ResolveAsync(Dictionary<string, string> textPlaceholders, Dictionary<string, Table> tablePlaceholders, ContractEngineContext context)
        {
            var cotizacion = context.Cotizacion;
            var data = context.Data;

            var visita = cotizacion.SecVisitaNavigation;
            var empresa = visita?.SecEmpresaNavigation;
            var contacto = visita?.Contactovisita?.FirstOrDefault()?.SecContactoNavigation;
            var constructora = contacto?.SecConstructoraNavigation;

            // Empresa
            textPlaceholders["{{empresanombre}}"] = empresa?.Nombre ?? "";
            textPlaceholders["{{empresaid}}"] = empresa?.Identificacion ?? "";
            textPlaceholders["{{empresa_direccion}}"] = empresa?.Direccion ?? "";
            textPlaceholders["{{empresa_telefono}}"] = empresa?.Telefono ?? "";

            // Proyecto / Visita
            textPlaceholders["{{nombreproyecto}}"] = visita?.Nombre ?? "";
            textPlaceholders["{{nombreproyectoenmayusculas}}"] = visita?.Nombre?.ToUpper() ?? "";
            textPlaceholders["{{nombreprovincia}}"] = visita?.SecProvinciaNavigation?.Nombre ?? "";
            textPlaceholders["{{nombrecanton}}"] = visita?.SecCantonNavigation?.Nombre ?? "";
            textPlaceholders["{{nombreparroquia}}"] = visita?.SecParroquiaNavigation?.Nombre ?? "";
            textPlaceholders["{{direccionproyecto}}"] = visita?.Direccion ?? "";

            // Cliente
            textPlaceholders["{{nombrecompletocliente}}"] = constructora?.Nombre ?? "";
            textPlaceholders["{{identificacioncliente}}"] = constructora?.Ruc ?? constructora?.Cliente?.NumeroCliente ?? "";
            textPlaceholders["{{clientedireccion}}"] = constructora?.Direccion ?? "";
            textPlaceholders["{{clientetelefono}}"] = constructora?.Telefono ?? "";
            textPlaceholders["{{clientecorreo}}"] = constructora?.Correo ?? "";
            textPlaceholders["{{clienterepresentantelegal}}"] = constructora?.Administrador ?? "";

            // Contacto Específico (Template)
            string nombreContacto = ((contacto?.Nombres ?? "") + " " + (contacto?.Apellidos ?? "")).Trim();
            textPlaceholders["{{NombreContacto}}"] = !string.IsNullOrEmpty(nombreContacto) ? nombreContacto : (constructora?.Administrador ?? "");
            textPlaceholders["{{EmailContacto}}"] = contacto?.Correo ?? constructora?.Correo ?? "";
            textPlaceholders["{{IdentificacionContacto}}"] = constructora?.Ruc ?? "";
            textPlaceholders["{{NumeroHojasDocumentoGenerado}}"] = "2"; // Valor estándar o placeholder de relleno

            // Empresa (Alias adicionales)
            textPlaceholders["{{EmailEmpresa}}"] = empresa?.Correo ?? "";
            textPlaceholders["{{IdentificacionEmpresa}}"] = empresa?.Identificacion ?? "";

            // Datos de la Negociación (Desde el DTO)
            textPlaceholders["{{diasdeentrega}}"] = $"{data.Dias?.ToString() ?? "0"} {data.TipoDias ?? ""}".Trim();
            textPlaceholders["{{aniosgarantia}}"] = data.AniosGarantia?.ToString() ?? "0";
            textPlaceholders["{{añosgarantia}}"] = data.AniosGarantia?.ToString() ?? "0";
            textPlaceholders["{{mesesgarantia}}"] = data.MesesGarantia?.ToString() ?? "0";
            textPlaceholders["{{periodomantenimiento}}"] = data.PeriodoMantenimiento ?? "";
            textPlaceholders["{{polizagarantia}}"] = data.PolizaGarantia ?? "";

            // Fechas
            textPlaceholders["{{fecha_actual}}"] = DateTime.Now.ToString("dd/MM/yyyy");
            textPlaceholders["{{fecha_actual_larga}}"] = DateTime.Now.ToString("dd 'de' MMMM 'de' yyyy", new System.Globalization.CultureInfo("es-ES"));
            textPlaceholders["{{fechafirmacontratoenletras}}"] = DateTime.Now.ToString("dd 'de' MMMM 'de' yyyy", new System.Globalization.CultureInfo("es-ES"));
            textPlaceholders["{{fechafirmacontrato}}"] = textPlaceholders["{{fechafirmacontratoenletras}}"];
            textPlaceholders["{{fecha_firma}}"] = textPlaceholders["{{fechafirmacontratoenletras}}"];

            return Task.CompletedTask;
        }
    }
}

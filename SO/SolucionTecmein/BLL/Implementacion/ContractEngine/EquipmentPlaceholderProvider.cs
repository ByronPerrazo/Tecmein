using BLL.ContractEngine;
using BLL.DTOs;
using BLL.Interfaces;
using DAL.DBContext;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BLL.Implementacion.ContractEngine
{
    public class EquipmentPlaceholderProvider : IPlaceholderProvider
    {
        private readonly TecmeindbContext _context;

        public EquipmentPlaceholderProvider(TecmeindbContext context)
        {
            _context = context;
        }

        public async Task ResolveAsync(Dictionary<string, string> textPlaceholders, Dictionary<string, Table> tablePlaceholders, ContractEngineContext context)
        {
            var cotizacion = context.Cotizacion;
            var data = context.Data;

            if (cotizacion == null || !cotizacion.Cotizaciondetalles.Any()) return;

            // Para pre-contratos, usualmente nos basamos en el primer equipo o el principal
            var detalle = cotizacion.Cotizaciondetalles.FirstOrDefault();
            if (detalle == null) return;

            // Intentar buscar el equipo técnico si existe la referencia
            Entity.Equiposvisita? equipoTecnico = null;
            if (detalle.SecEquipoVisita.HasValue)
            {
                equipoTecnico = await _context.Equiposvisita
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Secuencial == detalle.SecEquipoVisita.Value);
            }

            // Tags de Texto Técnicos (con fallback al string de detalle si el objeto es nulo)
            string detalleStr = detalle.DetalleEquipo ?? "";

            textPlaceholders["{{MarcaEquipo}}"] = equipoTecnico?.Marca ?? GetValueFromDetail(detalleStr, "Marca", "S/N");
            textPlaceholders["{{NumeroDeParadas}}"] = equipoTecnico?.NumeroParadas?.ToString() ?? GetValueFromDetail(detalleStr, "Num. Paradas", "0");
            textPlaceholders["{{Cantidad}}"] = detalle.Cantidad.ToString();
            textPlaceholders["{{TipoEquipo}}"] = equipoTecnico?.TipoEquipo ?? GetValueFromDetail(detalleStr, "Tipo Eq", "Ascensor");
            textPlaceholders["{{Capacidad}}"] = equipoTecnico?.Capacidad.ToString() ?? GetValueFromDetail(detalleStr, "Capacidad", "0");
            textPlaceholders["{{Velocidad}}"] = equipoTecnico?.Velocidad?.ToString("N2") ?? GetValueFromDetail(detalleStr, "Velocidad", "0.00");

            // Detalle dinámico de instalación y ducto
            var infoInstalacion = new List<string>();
            if (equipoTecnico != null)
            {
                if (!string.IsNullOrEmpty(equipoTecnico.TipoDucto)) infoInstalacion.Add($"Ducto: {equipoTecnico.TipoDucto}");
                if (!string.IsNullOrEmpty(equipoTecnico.MedidasAfducto)) infoInstalacion.Add($"Medidas: {equipoTecnico.MedidasAfducto}");
                if (!string.IsNullOrEmpty(equipoTecnico.Energia)) infoInstalacion.Add($"Energía: {equipoTecnico.Energia}");
                if (equipoTecnico.Recorrido > 0) infoInstalacion.Add($"Recorrido: {equipoTecnico.Recorrido} mm");
                if (equipoTecnico.Foso > 0) infoInstalacion.Add($"Foso: {equipoTecnico.Foso} mm");
                if (!string.IsNullOrEmpty(equipoTecnico.MaterialPuertas)) infoInstalacion.Add($"Puertas: {equipoTecnico.MaterialPuertas}");
            }
            else if (!string.IsNullOrEmpty(detalleStr))
            {
                // Extraer del string si el objeto no existe
                string ducto = GetValueFromDetail(detalleStr, "Ducto", "");
                string medidas = GetValueFromDetail(detalleStr, "MedidasAF", "");
                string energia = GetValueFromDetail(detalleStr, "Energia", "");
                string foso = GetValueFromDetail(detalleStr, "Foso", "");
                string recorrido = GetValueFromDetail(detalleStr, "Recorrido", "");

                if (!string.IsNullOrEmpty(ducto)) infoInstalacion.Add($"Ducto: {ducto}");
                if (!string.IsNullOrEmpty(medidas)) infoInstalacion.Add($"Medidas: {medidas}");
                if (!string.IsNullOrEmpty(energia)) infoInstalacion.Add($"Energía: {energia}");
                if (!string.IsNullOrEmpty(foso) && foso != "0") infoInstalacion.Add($"Foso: {foso} mm");
                if (!string.IsNullOrEmpty(recorrido) && recorrido != "0") infoInstalacion.Add($"Recorrido: {recorrido} mm");
            }

            textPlaceholders["{{detalleinstalacionducto}}"] = infoInstalacion.Any() 
                ? string.Join(", ", infoInstalacion) 
                : "Se instalará en ducto existente según especificaciones técnicas de fábrica y planos adjuntos.";
            
            // Alias para compatibilidad total (con y sin 'y')
            textPlaceholders["{{detalleinstalacionyducto}}"] = textPlaceholders["{{detalleinstalacionducto}}"];

            // Tabla de Especificaciones Técnicas
            textPlaceholders["{{tablaespecificacionesequipo}}"] = "";
            tablePlaceholders["{{tablaespecificacionesequipo}}"] = GenerarTablaTecnica(equipoTecnico, detalle);
        }

        private string GetValueFromDetail(string detail, string key, string defaultValue)
        {
            if (string.IsNullOrEmpty(detail)) return defaultValue;
            // Busca "Key:Valor" o "Key : Valor" seguido de " -" o el final del string
            var match = Regex.Match(detail, $@"{Regex.Escape(key)}\s*:\s*([^-\n|]+)");
            return match.Success ? match.Groups[1].Value.Trim() : defaultValue;
        }

        private Table GenerarTablaTecnica(Entity.Equiposvisita? equipo, Entity.Cotizaciondetalle detalle)
        {
            string d = detalle.DetalleEquipo ?? "";
            var table = CreateBaseTable();
            AddHeaderRow(table, "Característica", "Especificación");

            AddRow(table, "Equipo", equipo?.TipoEquipo ?? GetValueFromDetail(d, "Tipo Eq", "Ascensor"));
            AddRow(table, "Marca", equipo?.Marca ?? GetValueFromDetail(d, "Marca", "S/N"));
            
            string capacidadText = equipo != null 
                ? $"{equipo.Capacidad} Kg / {equipo.NumeroPersonas ?? 0} Personas"
                : $"{GetValueFromDetail(d, "Capacidad", "0")} Kg / {GetValueFromDetail(d, "Num Personas", "0")} Personas";
            
            AddRow(table, "Capacidad", capacidadText);
            
            string velocidadVal = equipo?.Velocidad?.ToString("N2") ?? GetValueFromDetail(d, "Velocidad", "0.00");
            AddRow(table, "Velocidad", $"{velocidadVal} m/s");

            AddRow(table, "Paradas", equipo?.NumeroParadas?.ToString() ?? GetValueFromDetail(d, "Num. Paradas", "0"));
            AddRow(table, "Sistema", equipo?.Sistema ?? GetValueFromDetail(d, "Sistema", "S/N"));
            AddRow(table, "Sala de Máquinas", equipo?.SalaMaquinas ?? GetValueFromDetail(d, "Sala Maq", "No"));

            return table;
        }

        // Helpers de OpenXML
        private Table CreateBaseTable()
        {
            var table = new Table();
            var tblPr = new TableProperties(
                new TableStyle { Val = "TableGrid" },
                new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct },
                new TableBorders(
                    new TopBorder { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder { Val = BorderValues.Single, Size = 4 },
                    new RightBorder { Val = BorderValues.Single, Size = 4 },
                    new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
                    new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 }
                )
            );
            table.AppendChild(tblPr);
            return table;
        }

        private void AddHeaderRow(Table table, params string[] headers)
        {
            var tr = new TableRow();
            foreach (var header in headers)
            {
                var tc = new TableCell(new Paragraph(new Run(new Text(header) { Space = SpaceProcessingModeValues.Preserve })));
                tc.Append(new TableCellProperties(new Shading { Val = ShadingPatternValues.Clear, Fill = "F2F2F2" }));
                tc.GetFirstChild<Paragraph>().GetFirstChild<Run>().RunProperties = new RunProperties(new Bold());
                tr.Append(tc);
            }
            table.Append(tr);
        }

        private void AddRow(Table table, params string[] values)
        {
            var tr = new TableRow();
            foreach (var val in values)
            {
                tr.Append(new TableCell(new Paragraph(new Run(new Text(val ?? "") { Space = SpaceProcessingModeValues.Preserve }))));
            }
            table.Append(tr);
        }
    }
}

using BLL.ContractEngine;
using BLL.DTOs;
using BLL.Interfaces;
using DAL.DBContext;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using Humanizer;

namespace BLL.Implementacion.ContractEngine
{
    public class FinancialPlaceholderProvider : IPlaceholderProvider
    {
        private readonly TecmeindbContext _context;

        public FinancialPlaceholderProvider(TecmeindbContext context)
        {
            _context = context;
        }

        public Task ResolveAsync(Dictionary<string, string> textPlaceholders, Dictionary<string, Table> tablePlaceholders, ContractEngineContext context)
        {
            var cotizacion = context.Cotizacion;
            var data = context.Data;

            if (cotizacion == null) return Task.CompletedTask;

            // Tags de Texto Financieros
            textPlaceholders["{{totalcontrato}}"] = cotizacion.TotalConImpuestos.ToString("N2");

            var totalEnteros = (int)Math.Truncate(cotizacion.TotalConImpuestos);
            textPlaceholders["{{totalcontratoenterosenletras}}"] = totalEnteros.ToWords(new System.Globalization.CultureInfo("es")).ToUpper();

            var centavos = (int)Math.Round((cotizacion.TotalConImpuestos - Math.Truncate(cotizacion.TotalConImpuestos)) * 100);
            textPlaceholders["{{centavoscontrato}}"] = centavos.ToString("00");

            // Tabla de Pagos
            textPlaceholders["{{tabladepagosconfechasytotales}}"] = ""; // Placeholder para la tabla
            tablePlaceholders["{{tabladepagosconfechasytotales}}"] = GenerarTablaPagos(data);

            return Task.CompletedTask;
        }

        private Table GenerarTablaPagos(PreContratoGeneratorDTO data)
        {
            var table = CreateBaseTable();
            AddHeaderRow(table, "Detalle", "Monto", "Fecha Vencimiento");

            if (data is BLL.DTOs.PreContratoConPagosDTO dataConPagos && dataConPagos.CompromisosDePago != null && dataConPagos.CompromisosDePago.Any())
            {
                foreach (var pago in dataConPagos.CompromisosDePago)
                {
                    AddRow(table, pago.Tipo, pago.Monto.ToString("N2"), pago.FechaVencimiento?.ToString("dd 'de' MMMM 'de' yyyy", new System.Globalization.CultureInfo("es-ES")) ?? "---");
                }
                decimal total = dataConPagos.CompromisosDePago.Sum(p => p.Monto);
                AddRowTotal(table, "TOTAL:", total.ToString("N2"), "");
            }
            else
            {
                AddRow(table, "Anticipo", "---", "---");
                AddRow(table, "Saldo contra entrega", "---", "---");
            }

            return table;
        }

        // Helpers de OpenXML (Podrían moverse a una clase base o utility)
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
                tc.Append(new TableCellProperties(new Shading { Val = ShadingPatternValues.Clear, Fill = "E0E0E0" }));
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

        private void AddRowTotal(Table table, string label, string amount, string date)
        {
            var tr = new TableRow();
            var tcLabel = new TableCell(new Paragraph(new Run(new Text(label) { Space = SpaceProcessingModeValues.Preserve })));
            tcLabel.GetFirstChild<Paragraph>().GetFirstChild<Run>().RunProperties = new RunProperties(new Bold());
            tr.Append(tcLabel);

            var tcAmount = new TableCell(new Paragraph(new Run(new Text(amount) { Space = SpaceProcessingModeValues.Preserve })));
            tcAmount.GetFirstChild<Paragraph>().GetFirstChild<Run>().RunProperties = new RunProperties(new Bold());
            tr.Append(tcAmount);

            tr.Append(new TableCell(new Paragraph(new Run(new Text(date) { Space = SpaceProcessingModeValues.Preserve }))));
            table.Append(tr);
        }
    }
}

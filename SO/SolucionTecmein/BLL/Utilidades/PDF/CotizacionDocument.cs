
using Entity;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Linq;

namespace BLL.Utilidades.PDF
{
    public class CotizacionDocument : IDocument
    {
        private readonly Cotizacion _cotizacion;

        public CotizacionDocument(Cotizacion cotizacion)
        {
            _cotizacion = cotizacion;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Margin(50);

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().Element(ComposeFooter);
                });
        }

        void ComposeHeader(IContainer container)
        {
            var titleStyle = TextStyle.Default.FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text($"Cotización #{_cotizacion.Secuencial}", titleStyle);

                    column.Item().Text(text =>
                    {
                        text.Span("Fecha de Emisión: ").SemiBold();
                        text.Span($"{_cotizacion.FechaRegistro:yyyy-MM-dd}");
                    });
                });

                // Aquí se podría agregar un logo si estuviera disponible
                // row.ConstantItem(100).Height(50).Placeholder();
            });
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(40).Column(column =>
            {
                column.Spacing(20);

                // Datos del Cliente
                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("Cliente:").SemiBold();
                        // Asumiendo que la visita tiene la información de la empresa/constructora
                        col.Item().Text(_cotizacion.SecVisitaNavigation?.SecEmpresaNavigation?.Nombre ?? "N/A");
                        col.Item().Text(_cotizacion.SecVisitaNavigation?.Direccion ?? "");
                    });
                });

                // Tabla de Detalles
                column.Item().Element(ComposeTable);

                // Totales
                column.Item().AlignRight().Column(col => {
                    col.Item().Text(text =>
                    {
                        text.Span("Subtotal: ").SemiBold();
                        text.Span($"{_cotizacion.Subtotal:C}");
                    });
                    col.Item().Text(text =>
                    {
                        text.Span("Impuestos: ").SemiBold();
                        text.Span($"{_cotizacion.ValorImpuestos:C}");
                    });
                    col.Item().Text(text =>
                    {
                        text.Span("Total: ").SemiBold().FontSize(14);
                        text.Span($"{_cotizacion.TotalConImpuestos:C}").FontSize(14);
                    });
                });
            });
        }

        void ComposeTable(IContainer container)
        {
            container.Table(table =>
            {
                // Definición de columnas
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(4); // Descripción (ajustado para ocupar más espacio)
                    columns.RelativeColumn();    // Cantidad
                    columns.RelativeColumn();    // Total
                });

                // Encabezado de la tabla
                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Descripción");
                    header.Cell().Element(CellStyle).AlignCenter().Text("Cantidad");
                    header.Cell().Element(CellStyle).AlignRight().Text("Total");

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                    }
                });

                // Filas de la tabla
                foreach (var item in _cotizacion.Cotizaciondetalles)
                {
                    table.Cell().Element(CellStyle).Text(item.DetalleEquipo);
                    table.Cell().Element(CellStyle).AlignCenter().Text(item.Cantidad.ToString());
                    table.Cell().Element(CellStyle).AlignRight().Text(item.Total.ToString("C"));

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                    }
                }
            });
        }

        void ComposeFooter(IContainer container)
        {
            container.AlignCenter().Text(text =>
            {
                text.Span("Página ");
                text.CurrentPageNumber();
                text.Span(" de ");
                text.TotalPages();
            });
        }
    }
}

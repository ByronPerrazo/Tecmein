
using Entity;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Linq;

namespace BLL.Utilidades.PDF
{
    public class SolicitudEquiposDocument : IDocument
    {
        private readonly Cotizacion _cotizacion;

        public SolicitudEquiposDocument(Cotizacion cotizacion)
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
                    column.Item().Text("Solicitud de Equipos", titleStyle);
                    column.Item().Text($"Referencia Cotización #{_cotizacion.Secuencial}", TextStyle.Default.SemiBold());
                    column.Item().Text(text =>
                    {
                        text.Span("Fecha de Solicitud: ").SemiBold();
                        text.Span($"{DateTime.Now:yyyy-MM-dd}");
                    });
                     column.Item().Text(text =>
                    {
                        text.Span("Proyecto: ").SemiBold();
                        text.Span(_cotizacion.SecVisitaNavigation?.Nombre ?? "N/A");
                    });
                });
            });
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(40).Column(column =>
            {
                column.Spacing(20);
                column.Item().Element(ComposeTable);
            });
        }

        void ComposeTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(4); // Descripción
                    columns.RelativeColumn();    // Cantidad
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Descripción del Equipo");
                    header.Cell().Element(CellStyle).AlignCenter().Text("Cantidad");

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                    }
                });

                foreach (var item in _cotizacion.Cotizaciondetalles)
                {
                    table.Cell().Element(CellStyle).Text(item.DetalleEquipo);
                    table.Cell().Element(CellStyle).AlignCenter().Text(item.Cantidad.ToString());

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
            });
        }
    }
}

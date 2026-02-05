using BelaEstilo.Application.Services.Interfaces;
using BelaEstilo.Infraestructure.Repository.Interfaces;
using BelaEstilo.Application.DTOs;
using System;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.IO;

namespace BelaEstilo.Application.Services.Implementations
{
    public class ServiceFactura : IServiceFactura
    {
        private readonly IServiceEmail _serviceEmail;
        private readonly IServicePedido _servicePedido;

        public ServiceFactura(IServiceEmail serviceEmail, IServicePedido servicePedido)
        {
            _serviceEmail = serviceEmail;
            _servicePedido = servicePedido;
        }

        public byte[] GenerarEjemploPdf()
        {
            using (var ms = new MemoryStream())
            {
                var document = new PdfDocument();
                var page = document.AddPage();
                var gfx = XGraphics.FromPdfPage(page);
                var font = new XFont("Verdana", 14, XFontStyle.Bold);

                // Título centrado
                gfx.DrawString("PDF de prueba", font, XBrushes.DarkBlue,
                    new XRect(0, 100, page.Width, 30),
                    XStringFormats.TopCenter);

                // Texto normal
                font = new XFont("Verdana", 12, XFontStyle.Regular);
                gfx.DrawString("Este es un archivo PDF generado con PdfSharpCore.",
                    font, XBrushes.Black, 50, 150);

                document.Save(ms);
                return ms.ToArray();
            }
        }

        public async Task EnviarFacturaPorCorreoAsync(string correoDestino, int idPedido)
        {
            var pedido = await _servicePedido.FindByIdAsync(idPedido);
            if (pedido == null || string.IsNullOrWhiteSpace(correoDestino))
                return;

            var pdfBytes = await GenerarFacturaPdfAsync(idPedido);

            var asunto = $"Factura Bella Estilo - Pedido #{pedido.IdPedido}";
            var cuerpo = $@"
            <p>Hola {pedido.IdUsuarioNavigation?.Nombre},</p>
            <p>Gracias por tu compra en <strong>Bella Estilo</strong>.</p>
            <p>Adjuntamos la factura de tu pedido <strong>#{pedido.IdPedido}</strong>.</p>
            <p>Saludos,<br/>Equipo Bella Estilo</p>
        ";

            await _serviceEmail.EnviarFacturaPorCorreoAsync(correoDestino, asunto, cuerpo, pdfBytes, $"Factura_{pedido.IdPedido}.pdf");
        }

        public async Task<byte[]> GenerarFacturaPdfAsync(int idPedido)
        {
            PedidoDTO pedidoDto = await _servicePedido.FindByIdAsync(idPedido);
            if (pedidoDto == null) return null;

            using (MemoryStream ms = new MemoryStream())
            {
                var document = new PdfDocument();
                var page = document.AddPage();
                var gfx = XGraphics.FromPdfPage(page);
                var fontTitle = new XFont("Verdana", 20, XFontStyle.Bold);
                var fontHeader = new XFont("Verdana", 12, XFontStyle.Bold);
                var font = new XFont("Verdana", 11);
                var fontItalic = new XFont("Verdana", 10, XFontStyle.Italic);

                double y = 40;

                // Título centrado
                gfx.DrawString("Factura Comercial", fontTitle, XBrushes.DarkRed, new XRect(0, y, page.Width, 40), XStringFormats.TopCenter);
                y += 50;

                // Encabezado de pedido
                gfx.DrawString($"Pedido #: {pedidoDto.IdPedido}", font, XBrushes.Black, 40, y); y += 20;
                gfx.DrawString($"Fecha: {pedidoDto.FechaPedido:dd/MM/yyyy}", font, XBrushes.Black, 40, y); y += 20;
                gfx.DrawString($"Estado: {pedidoDto.Estado}", font, XBrushes.Black, 40, y); y += 20;
                gfx.DrawString($"Método de pago: {pedidoDto.MetodoPago}", font, XBrushes.Black, 40, y); y += 20;
                gfx.DrawString($"Dirección de envío: {pedidoDto.DireccionEnvio}", font, XBrushes.Black, 40, y); y += 30;

                // Línea separadora
                gfx.DrawLine(XPens.Gray, 40, y, page.Width - 40, y); y += 15;

                // Productos normales
                if (pedidoDto.PedidoProducto != null && pedidoDto.PedidoProducto.Any())
                {
                    gfx.DrawString("Productos normales:", fontHeader, XBrushes.Black, 40, y); y += 22;

                    foreach (var prod in pedidoDto.PedidoProducto)
                    {
                        string nombre = prod.IdProductoNavigation?.Nombre ?? "Producto";
                        gfx.DrawString($"- {nombre} x{prod.Cantidad} - ₡{prod.PrecioUnitario:N2}", font, XBrushes.Black, 50, y);
                        y += 18;
                    }

                    y += 10;
                }

                // Productos personalizados
                if (pedidoDto.PedidoPersonalizado != null && pedidoDto.PedidoPersonalizado.Any())
                {
                    gfx.DrawString("Productos personalizados:", fontHeader, XBrushes.Black, 40, y); y += 22;

                    foreach (var pers in pedidoDto.PedidoPersonalizado)
                    {
                        gfx.DrawString($"- {pers.NombreProductoPersonalizado}", font, XBrushes.Black, 50, y); y += 18;
                        gfx.DrawString($"  Base: ₡{pers.CostoBase:N2}", fontItalic, XBrushes.Black, 60, y); y += 16;

                        if (pers.Criterios != null && pers.Criterios.Any())
                        {
                            gfx.DrawString("  Extras:", fontItalic, XBrushes.Black, 60, y); y += 16;
                            foreach (var c in pers.Criterios)
                            {
                                gfx.DrawString($"    • {c.NombreCriterio}: ₡{c.CostoExtra:N2}", font, XBrushes.Black, 70, y); y += 16;
                            }
                        }

                        gfx.DrawString($"  Total personalizado: ₡{pers.TotalProductoPersonalizado:N2}", font, XBrushes.Black, 60, y); y += 25;
                    }
                }

                // Línea separadora
                gfx.DrawLine(XPens.Gray, 40, y, page.Width - 40, y); y += 15;

                // Totales
                gfx.DrawString($"Subtotal: ₡{pedidoDto.Subtotal:N2}", fontHeader, XBrushes.Black, 40, y); y += 20;
                gfx.DrawString($"IVA (13%): ₡{pedidoDto.IVA:N2}", fontHeader, XBrushes.Black, 40, y); y += 20;
                gfx.DrawString($"Total final: ₡{pedidoDto.Total:N2}", fontHeader, XBrushes.Black, 40, y); y += 20;

                // Guardar PDF
                document.Save(ms, false);
                return ms.ToArray();
            }
        }

    }
}

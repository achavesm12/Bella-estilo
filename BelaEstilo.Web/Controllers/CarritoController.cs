using BelaEstilo.Application.DTOs;
using BelaEstilo.Web.Utils;
using Microsoft.AspNetCore.Mvc;

namespace BelaEstilo.Web.Controllers
{
    public class CarritoController : Controller
    {
        private const decimal _IVA = 0.13m;

        public IActionResult Index()
        {

            var carrito = HttpContext.Session.GetObject<PedidoSessionDTO>("Pedido") ?? new PedidoSessionDTO();
            return View(carrito);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCantidadPersonalizado(Guid lineId, int cantidad)
        {
            if (cantidad < 1) cantidad = 1;

            var carrito = HttpContext.Session.GetObject<PedidoSessionDTO>("Pedido") ?? new PedidoSessionDTO();
            var item = carrito.ItemsPersonalizados.FirstOrDefault(i => i.LineId == lineId);
            if (item == null)
            {
                TempData["Error"] = "No se encontró el ítem en el carrito.";
                return RedirectToAction(nameof(Index));
            }

            item.Cantidad = cantidad;

            // Recalcular total de la línea: (base + extras) * cantidad + IVA
            var extras = item.Criterios.Sum(c => c.CostoExtra);
            var subtotalUnidad = item.CostoBase + extras;
            var subtotal = subtotalUnidad * item.Cantidad;
            var iva = Math.Round(subtotal * _IVA, 2);
            item.TotalLinea = subtotal + iva;

            RecalcularTotalesCarrito(carrito);
            HttpContext.Session.SetObject("Pedido", carrito);

            TempData["Success"] = "Cantidad actualizada.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCantidad(int idProducto, int cantidad)
        {
            var carrito = HttpContext.Session.GetObject<PedidoSessionDTO>("Pedido");

            if (carrito == null || !carrito.Items.Any())
            {
                TempData["Error"] = "No se pudo actualizar el carrito.";
                return RedirectToAction("Index");
            }

            var item = carrito.Items.FirstOrDefault(p => p.IdProducto == idProducto);
            if (item != null)
            {
                item.Cantidad = cantidad;
                item.TotalLinea = item.Cantidad * item.PrecioUnitario;
            }

            RecalcularTotalesCarrito(carrito);
            HttpContext.Session.SetObject("Pedido", carrito);

            TempData["Success"] = "Cantidad actualizada.";
            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemovePersonalizado(Guid lineId)
        {
            var carrito = HttpContext.Session.GetObject<PedidoSessionDTO>("Pedido") ?? new PedidoSessionDTO();
            var countBefore = carrito.ItemsPersonalizados.Count;

            carrito.ItemsPersonalizados = carrito.ItemsPersonalizados
                .Where(i => i.LineId != lineId)
                .ToList();

            if (carrito.ItemsPersonalizados.Count == countBefore)
            {
                TempData["Error"] = "No se pudo eliminar el ítem (no existe).";
                return RedirectToAction(nameof(Index));
            }

            RecalcularTotalesCarrito(carrito);
            HttpContext.Session.SetObject("Pedido", carrito);

            TempData["Success"] = "Producto personalizado eliminado.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int idProducto)
        {
            var carrito = HttpContext.Session.GetObject<PedidoSessionDTO>("Pedido");

            if (carrito == null || !carrito.Items.Any())
            {
                TempData["Error"] = "No se pudo eliminar el producto.";
                return RedirectToAction("Index");
            }

            var item = carrito.Items.FirstOrDefault(p => p.IdProducto == idProducto);
            if (item != null)
            {
                carrito.Items.Remove(item);
            }

            RecalcularTotalesCarrito(carrito);
            HttpContext.Session.SetObject("Pedido", carrito);

            TempData["Success"] = "Producto eliminado del carrito.";
            return RedirectToAction("Index");
        }

        private void RecalcularTotalesCarrito(PedidoSessionDTO carrito)
        {
            // Subtotal de productos normales
            var subtotalNormales = carrito.Items.Sum(p => p.PrecioUnitario * p.Cantidad);

            // Subtotal de productos personalizados
            var subtotalPersonalizados = carrito.ItemsPersonalizados.Sum(it =>
            {
                var extras = it.Criterios.Sum(c => c.CostoExtra);
                return (it.CostoBase + extras) * it.Cantidad;
            });

            carrito.Subtotal = subtotalNormales + subtotalPersonalizados;
            carrito.IVA = Math.Round(carrito.Subtotal * _IVA, 2);
            carrito.Total = carrito.Subtotal + carrito.IVA;
        }


        //private void RecalcularTotalesCarrito(PedidoSessionDTO carrito)
        //{
        //    var subtotalSinIva = carrito.ItemsPersonalizados.Sum(it =>
        //    {
        //        var extras = it.Criterios.Sum(c => c.CostoExtra);
        //        return (it.CostoBase + extras) * it.Cantidad;
        //    });

        //    carrito.Subtotal = subtotalSinIva;
        //    carrito.IVA = Math.Round(carrito.Subtotal * _IVA, 2);
        //    carrito.Total = carrito.Subtotal + carrito.IVA;
        //}
    }
}

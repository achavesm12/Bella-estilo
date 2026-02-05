using BelaEstilo.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BelaEstilo.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IServicePedido _servicePedido;

        public DashboardController(IServicePedido servicePedido)
        {
            _servicePedido = servicePedido;
        }

        public async Task<IActionResult> VentasPorDia()
        {
            DateTime desde = DateTime.Today.AddDays(-6); // últimos 7 días
            DateTime hasta = DateTime.Today;

            var datos = await _servicePedido.ObtenerVentasPorDiaAsync(desde, hasta);
            return View(datos);
        }
    }

}

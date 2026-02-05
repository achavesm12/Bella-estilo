using BelaEstilo.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace BelaEstilo.Web.Controllers
{
    public class PedidoController : Controller
    {
        private readonly IServicePedido _servicePedido;
        //private readonly IServiceProducto _service;
        //private readonly ILogger<ServiceBarcoHabitaciones> _logger;

        public PedidoController(IServicePedido servicePedido)
        {
            _servicePedido = servicePedido;
        }

        // GET: Pedido Controller
        public async Task<IActionResult> Index()
        {
            try
            {
                var collection = await _servicePedido.ListAsync();
                return View(collection);
            }
            catch (Exception ex)
            {
                return Content("Error de conexión o datos: " + ex.Message);
            }
        }

        public async Task<ActionResult> IndexAdmin(int? page)
        {
            try
            {
                var collection = await _servicePedido.ListAsync();
                return View(collection.ToPagedList(page ?? 1, 5));
            }
            catch (Exception ex)
            {
                return Content("Error de conexión o datos: " + ex.Message);
            }
        }

        public async Task<ActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("IndexAdmin");
                }

                var pedido = await _servicePedido.FindByIdAsync(id.Value);

                if (pedido == null)
                {
                    throw new Exception("Pedido no existente");
                }

                return View(pedido);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


    }
}

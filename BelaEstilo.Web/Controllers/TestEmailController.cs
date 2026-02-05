using BelaEstilo.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BelaEstilo.Web.Controllers
{
    public class TestEmailController : Controller
    {

        private readonly IServiceFactura _serviceFactura;
        private readonly IServiceUsuario _serviceUsuario;

        public TestEmailController(IServiceFactura serviceFactura, IServiceUsuario serviceUsuario)
        {
            _serviceFactura = serviceFactura;
            _serviceUsuario = serviceUsuario;
        }

        [HttpGet]
        public async Task<IActionResult> EnviarCorreoPrueba()
        {
            // Cambiá este ID por el ID de un pedido que vos hayas creado
            int idPedido = 1;

            // Buscar el correo del usuario que hizo el pedido
            var pedido = await _serviceFactura.GenerarFacturaPdfAsync(idPedido);
            var usuario = await _serviceUsuario.FindByIdAsync(1); // Cambiar si es necesario

            if (usuario == null)
            {
                return Content("Usuario no encontrado.");
            }

            await _serviceFactura.EnviarFacturaPorCorreoAsync(usuario.Correo, idPedido);

            return Content($"Correo enviado a {usuario.Correo}");
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}

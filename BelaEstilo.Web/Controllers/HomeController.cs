using BelaEstilo.Application.Services.Interfaces;
using BelaEstilo.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BelaEstilo.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IServiceCategoria _serviceCategoria;

        public HomeController(ILogger<HomeController> logger, IServiceCategoria serviceCategoria)
        {
            _logger = logger;
            _serviceCategoria = serviceCategoria;
        }

        public async Task<IActionResult> Index()
        {
            var categorias = await _serviceCategoria.ListAsync();
            return View(categorias);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult TestSession()
        {
            HttpContext.Session.SetString("MiClave", "Hola desde sesión");
            var valor = HttpContext.Session.GetString("MiClave");

            return Content($"Valor almacenado: {valor}");
        }

    }
}

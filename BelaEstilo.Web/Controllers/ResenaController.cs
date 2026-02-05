using BelaEstilo.Application.DTOs;
using BelaEstilo.Application.Services.Implementations;
using BelaEstilo.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using X.PagedList.Extensions;

namespace BelaEstilo.Web.Controllers
{
    [Authorize(Roles = "Cliente")]
    public class ResenaController : Controller
    {
        private readonly IServiceResena _serviceResena;
        //private readonly IServiceProducto _service;
        //private readonly ILogger<ServiceBarcoHabitaciones> _logger;

        public ResenaController(IServiceResena serviceResena)
        {
            _serviceResena = serviceResena;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var collection = await _serviceResena.ListAsync();
                return View(collection);
            }
            catch (Exception ex)
            {
                return Content("Error de conexión o datos: " + ex.Message);
            }
        }

        public async Task<ActionResult> IndexAdmin(int? page)
        {
            var collection = await _serviceResena.ListAsync();
            return View(collection.ToPagedList(page ?? 1, 5));
        }

        //GET: Resena Controller/Details/
        public async Task<ActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("IndexAdmin");
                }

                var resena = await _serviceResena.FindByIdAsync(id.Value);

                if (resena == null)
                {
                    return NotFound("La reseña no existe");
                }

                return View(resena);

            }
            catch (Exception ex)
            {
                return Content("Error: " + ex.Message);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Cliente")]
        public IActionResult Create(int idProducto)
        {
            var dto = new ResenaRegistroDTO { IdProducto = idProducto };
            return View(dto);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Create(ResenaRegistroDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized();
                }

                int idUsuario = int.Parse(userIdClaim.Value);

                await _serviceResena.AddAsync(dto, idUsuario);

                TempData["MensajeExito"] = "Reseña agregada correctamente.";

                return RedirectToAction("Details", "Producto", new { id = dto.IdProducto });
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Error al registrar la reseña: {ex.Message}";
                return View(dto);
            }
        }
    }
}

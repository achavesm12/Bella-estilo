using BelaEstilo.Application.DTOs;
using BelaEstilo.Application.Services.Interfaces;
using BelaEstilo.Infraestructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList.Extensions;

namespace BelaEstilo.Web.Controllers
{
    public class PromocionController : Controller
    {
        private readonly IServicePromocion _servicePromocion;
        private readonly IServiceProducto _serviceProducto;
        private readonly IServiceCategoria _serviceCategoria;


        public PromocionController(IServicePromocion servicePromocion, IServiceProducto serviceProducto, IServiceCategoria serviceCategoria)
        {
            _servicePromocion = servicePromocion;
            _serviceProducto = serviceProducto;
            _serviceCategoria = serviceCategoria;
        }

        // GET: Promocion Controller
        public async Task<IActionResult> Index()
        {
            try
            {
                var promociones = await _servicePromocion.ListAsync();
                return View(promociones);
            }
            catch (Exception ex)
            {
                return Content("Error al cargar las promociones: " + ex.Message);
            }
        }

        public async Task<IActionResult> IndexAdmin(int? page)
        {
            try
            {
                var promociones = await _servicePromocion.ListAsync();
                return View(promociones.ToPagedList(page ?? 1, 5));
            }
            catch (Exception ex)
            {
                return Content("Error al cargar las promociones: " + ex.Message);
            }
        }

        // GET: Promocion/Details/
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("IndexAdmin");
                }

                var promocion = await _servicePromocion.FindByIdAsync(id.Value);

                if (promocion == null)
                {
                    return NotFound("La promoción no existe");
                }

                return View(promocion);
            }
            catch (Exception ex)
            {
                return Content("Error de conexión o datos: " + ex.Message);
            }
        }

        public async Task<IActionResult> ProductosConPromocion()
        {
            var productos = await _serviceProducto.ListAsync();
            var promociones = await _servicePromocion.ListEntityAsync();

            var fechaActual = DateOnly.FromDateTime(DateTime.Now);
            var promocionesAplicadas = new Dictionary<int, decimal>();

            foreach (var producto in productos)
            {
                var promosDirectas = producto.IdPromocion != null
                    ? producto.IdPromocion
                        .Where(p => p.FechaInicio <= fechaActual && p.FechaFin >= fechaActual)
                        .ToList()
                    : new List<Promocion>();

                var promosPorCategoria = promociones
                    .Where(p => p.FechaInicio <= fechaActual && p.FechaFin >= fechaActual)
                    .Where(p => p.IdCategoria != null && p.IdCategoria.Any(c => c.IdCategoria == producto.IdCategoria))
                    .ToList();

                var todasLasPromos = promosDirectas.Concat(promosPorCategoria).ToList();

                var promoAplicable = todasLasPromos.FirstOrDefault(p => p.Descuento.HasValue);

                if (promoAplicable != null && producto.Precio.HasValue)
                {
                    var precioOriginal = producto.Precio.Value;
                    var descuento = promoAplicable.Descuento.Value;
                    var precioConDescuento = precioOriginal - (precioOriginal * descuento);

                    promocionesAplicadas[producto.IdProducto] = precioConDescuento;
                }
            }

            ViewBag.Promociones = promocionesAplicadas;
            ViewBag.TotalPromocionesAplicadas = promocionesAplicadas.Count;

            return View("ProductosConPromocion", productos);
        }

        // GET: Promocion/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var dto = new PromocionRegistroDTO();
            await CargarListasParaVista(dto);
            return View(dto);
        }


        // POST: Promocion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PromocionRegistroDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await CargarListasParaVista(dto);
                return View(dto);
            }

            try
            {
                await _servicePromocion.AddAsync(dto);
                TempData["MensajeExito"] = "Promoción creada correctamente.";
                return RedirectToAction("Index", "Promocion");
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"Error al crear la promoción: {ex.Message}";
                await CargarListasParaVista(dto);
                return View(dto);
            }
        }

        private static List<SelectListItem> MarcarSeleccionados(List<SelectListItem> lista, List<int>? seleccionados)
        {
            if (seleccionados == null) return lista;

            foreach (var item in lista)
            {
                if (seleccionados.Contains(int.Parse(item.Value)))
                {
                    item.Selected = true;
                }
            }

            return lista;
        }

        // Método auxiliar para cargar listas
        private async Task CargarListasParaVista(PromocionRegistroDTO dto)
        {
            // Tipos de promoción
            ViewBag.TiposPromocion = new List<SelectListItem>
            {
            new SelectListItem { Value = "", Text = "-- Seleccione --" },
            new SelectListItem { Value = "Categoria", Text = "Categoría", Selected = dto.TipoPromocion == "Categoria" },
            new SelectListItem { Value = "Producto", Text = "Producto", Selected = dto.TipoPromocion == "Producto" }
            };

            // Categorías
            var categorias = await _serviceCategoria.ListAsync();
            ViewBag.Categorias = categorias.Select(c => new SelectListItem
            {
                Value = c.IdCategoria.ToString(),
                Text = c.Nombre,
                Selected = dto.IdCategoria != null && dto.IdCategoria.Contains(c.IdCategoria)
            }).ToList();

            // Productos
            var productos = await _serviceProducto.ListAsync();
            ViewBag.Productos = productos.Select(p => new SelectListItem
            {
                Value = p.IdProducto.ToString(),
                Text = p.Nombre,
                Selected = dto.IdProducto != null && dto.IdProducto.Contains(p.IdProducto)
            }).ToList();
        }

        // GET: Promocion/Edit/5
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var promocion = await _servicePromocion.FindByIdAsync(id);
            if (promocion == null)
            {
                TempData["MensajeError"] = "Promoción no encontrada.";
                return RedirectToAction("Index");
            }

            // Validar si la promoción ya comenzó (fecha inicio anterior o igual a hoy)
            var hoy = DateOnly.FromDateTime(DateTime.Now);
            if (promocion.FechaInicio.HasValue && promocion.FechaInicio.Value <= hoy)
            {
                TempData["MensajeError"] = "No se pueden modificar promociones que ya caducaron.";
                return RedirectToAction("Index");
            }

            var dto = new PromocionRegistroDTO
            {
                IdPromocion = promocion.IdPromocion,
                Nombre = promocion.Nombre,
                TipoPromocion = (promocion.IdCategoria.Any() ? "Categoria" : "Producto"),
                IdCategoria = promocion.IdCategoria.Select(c => c.IdCategoria).ToList(),
                IdProducto = promocion.IdProducto.Select(p => p.IdProducto).ToList(),
                Descuento = (promocion.Descuento ?? 0m) * 100,
                FechaInicio = promocion.FechaInicio.HasValue ? promocion.FechaInicio.Value : DateOnly.FromDateTime(DateTime.Now),
                FechaFin = promocion.FechaFin.HasValue ? promocion.FechaFin.Value : DateOnly.FromDateTime(DateTime.Now)

            };

            await CargarListasParaVista(dto);
            return View(dto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PromocionRegistroDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await CargarListasParaVista(dto);
                return View(dto);
            }

            // Validaciones extra para fechas
            var hoy = DateOnly.FromDateTime(DateTime.Now);

            if (dto.FechaInicio < hoy)
            {
                ModelState.AddModelError(nameof(dto.FechaInicio), "La fecha de inicio no puede ser anterior a la fecha actual.");
                await CargarListasParaVista(dto);
                return View(dto);
            }
            if (dto.FechaFin < dto.FechaInicio)
            {
                ModelState.AddModelError(nameof(dto.FechaFin), "La fecha de fin no puede ser anterior a la fecha de inicio.");
                await CargarListasParaVista(dto);
                return View(dto);
            }

            try
            {
                dto.Descuento = dto.Descuento / 100;

                var promocionExistente = await _servicePromocion.FindByIdAsync(dto.IdPromocion);

                if (promocionExistente == null)
                {
                    TempData["MensajeError"] = "Promoción no encontrada.";
                    return RedirectToAction("Index");
                }

                // No permitir modificar promociones vigentes o pasadas
                if (promocionExistente.FechaInicio.HasValue && promocionExistente.FechaInicio.Value <= hoy)
                {
                    TempData["MensajeError"] = "No se pueden modificar promociones que ya están vigentes o pasadas.";
                    return RedirectToAction("Index");
                }

                await _servicePromocion.UpdateAsync(dto);

                TempData["MensajeExito"] = "Promoción actualizada correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al actualizar la promoción: {ex.Message}");
                await CargarListasParaVista(dto);
                return View(dto);
            }
        }


    }
}

using BelaEstilo.Application.DTOs;
using BelaEstilo.Application.Services.Implementations;
using BelaEstilo.Application.Services.Interfaces;
using BelaEstilo.Web.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList.Extensions;

namespace BelaEstilo.Web.Controllers
{
    public class ProductoController : Controller
    {
        private readonly IServiceProducto _serviceProducto;
        private readonly IServiceCategoria _serviceCategoria;
        private readonly IServiceEtiqueta _serviceEtiqueta;
        //private readonly ILogger<ServiceBarcoHabitaciones> _logger;

        public ProductoController(IServiceProducto serviceProducto, IServiceCategoria serviceCategoria,
            IServiceEtiqueta serviceEtiqueta)
        {
            _serviceProducto = serviceProducto;
            _serviceCategoria = serviceCategoria;
            _serviceEtiqueta = serviceEtiqueta;
        }

        //GET: Producto Controller
        public async Task<IActionResult> Index()
        {
            try
            {
                var collection = await _serviceProducto.ListAsync();
                return View(collection);
            }
            catch (Exception ex)
            {
                return Content("Error de conexión o datos: " + ex.Message);
            }
        }

        public async Task<ActionResult> IndexAdmin(int? page)
        {
            var collection = await _serviceProducto.ListAsync();
            return View(collection.ToPagedList(page ?? 1, 5));
        }

        //GET: Producto Controller/Details/
        public async Task<ActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("IndexAdmin");
                }

                var @object = await _serviceProducto.FindByIdAsync(id.Value);

                if (@object == null)
                {
                    return NotFound("El producto no existe");
                }

                return View(@object);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // GET: Producto/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var categorias = await _serviceCategoria.ListAsync();
            var etiquetas = await _serviceEtiqueta.ListAsync();

            ViewBag.Categorias = new SelectList(categorias, "IdCategoria", "Nombre");
            ViewBag.Etiquetas = new MultiSelectList(etiquetas, "IdEtiqueta", "Nombre");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(ProductoRegistroDTO dto)
        {
            // Cargar listas para la vista (categorías y etiquetas)
            var categorias = await _serviceCategoria.ListAsync();
            var etiquetas = await _serviceEtiqueta.ListAsync(); // Asegúrate de tener este servicio

            ViewBag.Categorias = new SelectList(categorias, "IdCategoria", "Nombre");
            ViewBag.Etiquetas = new MultiSelectList(etiquetas, "IdEtiqueta", "Nombre");

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                await _serviceProducto.AddAsync(dto);
                TempData["MensajeExito"] = "Producto creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al crear producto: {ex.Message}");
                return View(dto);
            }
        }


        // GET: Producto/Edit
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var productoDTO = await _serviceProducto.FindByIdAsync(id);
            if (productoDTO == null)
                return NotFound();

            var productoRegistro = new ProductoRegistroDTO
            {
                IdProducto = productoDTO.IdProducto,
                Nombre = productoDTO.Nombre,
                Descripcion = productoDTO.Descripcion,
                Precio = productoDTO.Precio,
                Stock = productoDTO.Stock,
                TipoTela = productoDTO.TipoTela,
                TallaDisponible = productoDTO.TallaDisponible,
                EstaActivo = productoDTO.EstaActivo ?? true,
                IdCategoria = productoDTO.IdCategoria,
                IdsEtiquetas = productoDTO.IdEtiqueta.Select(e => e.IdEtiqueta).ToList()
            };

            // Cargar imágenes actuales del producto
            productoRegistro.ImagenesActuales = await _serviceProducto.GetImagenesProductoAsync(id);


            var categorias = await _serviceCategoria.ListAsync();
            var etiquetas = await _serviceEtiqueta.ListAsync();

            ViewBag.Categorias = new SelectList(categorias, "IdCategoria", "Nombre", productoRegistro.IdCategoria);
            ViewBag.Etiquetas = new MultiSelectList(etiquetas, "IdEtiqueta", "Nombre", productoRegistro.IdsEtiquetas);

            return View(productoRegistro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(ProductoRegistroDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await CargarListasEnViewBag(dto);
                dto.ImagenesActuales = await _serviceProducto.GetImagenesProductoAsync(dto.IdProducto ?? 0);
                return View(dto);
            }

            if (dto.IdProducto == null || dto.IdProducto == 0)
            {
                return NotFound();
            }

            try
            {
                await _serviceProducto.UpdateAsync(dto);
                TempData["MensajeExito"] = "Producto actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al actualizar el producto: {ex.Message}");
                await CargarListasEnViewBag(dto);
                dto.ImagenesActuales = await _serviceProducto.GetImagenesProductoAsync(dto.IdProducto ?? 0);
                return View(dto);
            }
        }
        private async Task CargarListasEnViewBag(ProductoRegistroDTO dto)
        {
            var categorias = await _serviceCategoria.ListAsync();
            var etiquetas = await _serviceEtiqueta.ListAsync();

            ViewBag.Categorias = new SelectList(categorias, "IdCategoria", "Nombre", dto.IdCategoria);
            ViewBag.Etiquetas = new MultiSelectList(etiquetas, "IdEtiqueta", "Nombre", dto.IdsEtiquetas);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarProducto(int idProducto, int cantidad = 1)
        {
            try
            {
                var producto = await _serviceProducto.FindByIdAsync(idProducto);

                if (producto == null)
                {
                    TempData["Error"] = "Producto no encontrado.";
                    return RedirectToAction("Index");
                }

                var carrito = HttpContext.Session.GetObject<PedidoSessionDTO>("Pedido") ?? new PedidoSessionDTO();

                var existente = carrito.Items.FirstOrDefault(x => x.IdProducto == idProducto);

                if (existente != null)
                {
                    existente.Cantidad += cantidad;
                    existente.TotalLinea = existente.Cantidad * existente.PrecioUnitario;
                }
                else
                {
                    carrito.Items.Add(new PedidoSessionDTO.CarritoItemDTO
                    {
                        IdProducto = producto.IdProducto,
                        NombreProducto = producto.Nombre,
                        PrecioUnitario = producto.Precio ?? 0,
                        Cantidad = cantidad,
                        TotalLinea = cantidad * (producto.Precio ?? 0)
                    });
                }

                RecalcularTotalesCarrito(carrito);
                HttpContext.Session.SetObject("Pedido", carrito);

                TempData["Success"] = "Producto agregado al carrito.";
                return RedirectToAction("Index", "Carrito");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al agregar producto: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        private void RecalcularTotalesCarrito(PedidoSessionDTO carrito)
        {
            var subtotal = carrito.Items.Sum(x => x.TotalLinea)
                        + carrito.ItemsPersonalizados.Sum(x => x.TotalLinea);

            var iva = Math.Round(subtotal * 0.13m, 2);
            var total = subtotal + iva;

            carrito.Subtotal = subtotal;
            carrito.IVA = iva;
            carrito.Total = total;
        }


    }
}

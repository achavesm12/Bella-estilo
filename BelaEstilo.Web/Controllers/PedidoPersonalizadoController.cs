using BelaEstilo.Application.DTOs;
using BelaEstilo.Application.Services.Interfaces;
using BelaEstilo.Web.Utils;
using Microsoft.AspNetCore.Mvc;
using static BelaEstilo.Application.DTOs.PedidoSessionDTO;

namespace BelaEstilo.Web.Controllers
{
    public class PedidoPersonalizadoController : Controller
    {
        private const decimal _IVA = 0.13m;
        private readonly IServicePedidoPersonalizado _servicePersonalizado;
        private readonly ILogger<PedidoPersonalizadoController> _logger;

        public PedidoPersonalizadoController(IServicePedidoPersonalizado servicePersonalizado,
            ILogger<PedidoPersonalizadoController> logger)
        {
            _servicePersonalizado = servicePersonalizado;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET: /ProductoPersonalizado/Crear/5
        [HttpGet]
        public async Task<IActionResult> Create(int idProducto)
        {
            try
            {
                if (idProducto <= 0)
                {
                    TempData["Error"] = "Producto inválido.";
                    return RedirectToAction("Index", "Producto");
                }

                var dto = await _servicePersonalizado.GetPedidoBaseAsync(idProducto);
                dto.IdProducto = idProducto; /**/
                return View(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar personalización para producto {IdProducto}", idProducto);
                TempData["Error"] = "No se pudo cargar la personalización del producto.";
                return RedirectToAction("Index", "Producto");
            }
        }


        //// POST: /ProductoPersonalizado/AgregarAlCarrito
        //// Guarda SOLO en sesión (no persiste en BD)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AgregarAlCarrito(PedidoPersonalizadoDTO personalizado)
        {
            try
            {
                _logger.LogInformation("Se recibieron {count} criterios en el modelo", personalizado.Criterios?.Count ?? 0);

                _logger.LogInformation("Criterios totales en el formulario: {Count}", personalizado?.Criterios?.Count ?? 0);
                _logger.LogInformation("IDs seleccionados: {Ids}",
                    (personalizado?.CriteriosSeleccionadosIds != null && personalizado.CriteriosSeleccionadosIds.Any())
                        ? string.Join(", ", personalizado.CriteriosSeleccionadosIds)
                        : "(ninguno)");

                if (personalizado == null || personalizado.IdProducto <= 0)
                {
                    TempData["Error"] = "Datos inválidos para personalización.";
                    return RedirectToAction("Index", "Producto");
                }

                //FILTRAR solo los criterios seleccionados por el usuario
                var seleccionados = (personalizado.Criterios ?? new List<PedidoPersonalizadoCriterioDTO>())
                    .Where(c => (personalizado.CriteriosSeleccionadosIds ?? new List<int>()).Contains(c.Id))
                    .ToList();

                //Log para confirmar lo que se seleccionó
                _logger.LogInformation("🎯 Criterios SELECCIONADOS:");
                foreach (var c in seleccionados)
                {
                    _logger.LogInformation("- {Nombre} (+₡{Costo})", c.NombreCriterio, c.CostoExtra);
                }

                //Calcular totales
                var extras = seleccionados.Sum(x => x.CostoExtra);
                var subtotal = personalizado.CostoBase + extras;
                var iva = Math.Round(subtotal * _IVA, 2);
                var total = subtotal + iva;

                //Crear línea personalizada
                var linea = new CarritoItemPersonalizadoDTO
                {
                    LineId = Guid.NewGuid(),
                    IdProducto = personalizado.IdProducto,
                    NombreProducto = personalizado.NombreProductoPersonalizado,
                    CostoBase = personalizado.CostoBase,
                    Cantidad = 1,
                    Criterios = seleccionados.Select(s => new SeleccionCriterioDTO
                    {
                        NombreCriterio = s.NombreCriterio,
                        OpcionSeleccionada = string.IsNullOrWhiteSpace(s.OpcionSeleccionada) ? "Sí" : s.OpcionSeleccionada,
                        CostoExtra = s.CostoExtra
                    }).ToList(),
                    TotalLinea = total
                };

                //Obtener o crear carrito en sesión
                var carrito = HttpContext.Session.GetObject<PedidoSessionDTO>("Pedido") ?? new PedidoSessionDTO();

                //Agregar línea personalizada
                carrito.ItemsPersonalizados.Add(linea);

                //Recalcular totales del carrito
                RecalcularTotalesCarrito(carrito);

                //Guardar en sesión
                HttpContext.Session.SetObject("Pedido", carrito);

                TempData["Success"] = "Producto personalizado agregado al carrito.";
                return RedirectToAction("Index", "Carrito");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al agregar producto personalizado al carrito: {IdProducto}", personalizado?.IdProducto);
                TempData["Error"] = "Ocurrió un error al agregar el producto personalizado.";
                return RedirectToAction("Index", "Producto");
            }
        }

        // (Opcional) Endpoint para cálculo rápido por AJAX (preview sin persistir)
        [HttpPost]
        public IActionResult CalcularTotal([FromBody] PedidoPersonalizadoDTO personalizado)
        {
            try
            {
                if (personalizado == null) return BadRequest("Datos inválidos.");

                var seleccionados = (personalizado.Criterios ?? new System.Collections.Generic.List<PedidoPersonalizadoCriterioDTO>())
                                    .Where(c => c.Seleccionado)
                                    .ToList();

                var extras = seleccionados.Sum(c => c.CostoExtra);
                var subtotal = personalizado.CostoBase + extras;
                var iva = Math.Round(subtotal * _IVA, 2);
                var total = subtotal + iva;

                return Ok(new { subtotal, iva, total });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al calcular total de producto personalizado.");
                return StatusCode(500, "No se pudo calcular el total.");
            }
        }

        // ---------- Helpers ----------
        private void RecalcularTotalesCarrito(PedidoSessionDTO carrito)
        {
            var subtotalSinIva = carrito.ItemsPersonalizados.Sum(it =>
            {
                var extras = it.Criterios.Sum(c => c.CostoExtra);
                return (it.CostoBase + extras) * it.Cantidad;
            });

            carrito.Subtotal = subtotalSinIva;
            carrito.IVA = Math.Round(carrito.Subtotal * _IVA, 2);
            carrito.Total = carrito.Subtotal + carrito.IVA;
        }

        [HttpPost]
        public IActionResult TestCheckboxBinding(List<PedidoPersonalizadoCriterioDTO> criterios)
        {
            foreach (var crit in criterios)
            {
                Console.WriteLine($"Criterio: {crit.NombreCriterio}, Seleccionado: {crit.Seleccionado}");
            }

            TempData["Success"] = "Revisá la consola para ver si los checkbox marcaron correctamente.";
            return RedirectToAction("Index", "Home"); // O redirigí a donde querás
        }

        [HttpGet]
        public IActionResult Test()
        {
            var criterios = new List<PedidoPersonalizadoCriterioDTO>
            {
                new() { NombreCriterio = "Estampado floral", Seleccionado = false },
                new() { NombreCriterio = "Mangas largas", Seleccionado = true },
                new() { NombreCriterio = "Bolsillo extra", Seleccionado = false }
            };

            return View(criterios);
        }


        //// POST: /ProductoPersonalizado/Crear
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(PedidoDTO pedido, PedidoPersonalizadoDTO personalizado)
        //{
        //    try
        //    {
        //        // Validaciones mínimas
        //        if (personalizado == null || personalizado.IdProducto <= 0)
        //        {
        //            ModelState.AddModelError(string.Empty, "El producto base es requerido.");
        //        }

        //        if (!ModelState.IsValid)
        //        {
        //            // Si falla, volvemos a mostrar la misma vista con el modelo que el usuario envió
        //            return View(personalizado);
        //        }

        //        // Completar algunos campos del pedido si no vienen del formulario
        //        if (string.IsNullOrWhiteSpace(pedido.Estado))
        //            pedido.Estado = "Pendiente";

        //        // Si manejas autenticación, aquí puedes asignar el IdUsuario del usuario logueado
        //        // pedido.IdUsuario = GetUserId();

        //        var idPedido = await _servicePersonalizado.AddPersonalizadoAsync(pedido, personalizado);

        //        TempData["Success"] = "Producto personalizado agregado al pedido correctamente.";
        //        return RedirectToAction("Confirmacion", "Pedido", new { id = idPedido });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error al crear producto personalizado para producto {IdProducto}", personalizado?.IdProducto);
        //        TempData["Error"] = "Ocurrió un error al crear el producto personalizado.";
        //        return View(personalizado);
        //    }
        //}

        //// POST: /ProductoPersonalizado/AgregarAlCarrito
        //// Guarda SOLO en sesión (no persiste en BD)
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult AgregarAlCarrito(PedidoPersonalizadoDTO personalizado)
        //{
        //    try
        //    {
        //        if (personalizado == null || personalizado.IdProducto <= 0)
        //        {
        //            TempData["Error"] = "Datos inválidos para personalización.";
        //            return RedirectToAction("Index", "Producto");
        //        }

        //        // 1) Filtrar criterios seleccionados (si usas checkbox con 'Seleccionado')
        //        var seleccionados = (personalizado.Criterios ?? new System.Collections.Generic.List<PedidoPersonalizadoCriterioDTO>())
        //                            .Where(c => c.Seleccionado)
        //                            .ToList();

        //        // Si NO tienes 'Seleccionado' en el DTO, descomenta esta línea:
        //        // seleccionados = personalizado.Criterios?.ToList() ?? new();

        //        // 2) Calcular totales de la línea (base + extras + IVA)
        //        var extras = seleccionados.Sum(x => x.CostoExtra);
        //        var subtotal = personalizado.CostoBase + extras;
        //        var iva = Math.Round(subtotal * _IVA, 2);
        //        var total = subtotal + iva;

        //        // 3) Construir la línea para el carrito
        //        var linea = new CarritoItemPersonalizadoDTO
        //        {
        //            IdProducto = personalizado.IdProducto,
        //            NombreProducto = personalizado.NombreProductoPersonalizado,
        //            CostoBase = personalizado.CostoBase,
        //            Cantidad = 1,
        //            Criterios = seleccionados.Select(s => new SeleccionCriterioDTO
        //            {
        //                NombreCriterio = s.NombreCriterio,
        //                OpcionSeleccionada = string.IsNullOrWhiteSpace(s.OpcionSeleccionada) ? "Sí" : s.OpcionSeleccionada,
        //                CostoExtra = s.CostoExtra
        //            }).ToList(),
        //            TotalLinea = total
        //        };

        //        // 4) Leer carrito de sesión
        //        var carrito = HttpContext.Session.GetObject<PedidoSessionDTO>("Pedido") ?? new PedidoSessionDTO();

        //        // 5) Agregar línea personalizada
        //        carrito.ItemsPersonalizados.Add(linea);

        //        // 6) Recalcular totales del carrito
        //        RecalcularTotalesCarrito(carrito);

        //        // 7) Guardar carrito a sesión
        //        HttpContext.Session.SetObject("Pedido", carrito);

        //        TempData["Success"] = "Producto personalizado agregado al carrito.";
        //        return RedirectToAction("Index", "Carrito");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error al agregar al carrito la personalización del producto {IdProducto}", personalizado?.IdProducto);
        //        TempData["Error"] = "Ocurrió un error al agregar el producto personalizado.";
        //        return RedirectToAction("Index", "Producto");
        //    }
        //}
    }
}

using BelaEstilo.Application.DTOs;
using BelaEstilo.Application.Services.Interfaces;
using BelaEstilo.Web.Utils;
using Microsoft.AspNetCore.Mvc;
using static BelaEstilo.Application.DTOs.PedidoSessionDTO;

namespace BelaEstilo.Web.Controllers
{
    public class CheckoutController : Controller
    {
        private const decimal _TASA = 0.13m;

        private readonly IServiceProducto _serviceProducto;
        private readonly IServiceUsuario _serviceUsuario;
        private readonly IServicePedido _servicePedido;
        private readonly IServicePedidoPersonalizado _servicePersonalizado;
        private readonly IServicePedidoProducto _servicePedidoProducto;
        private readonly IServiceFactura _serviceFactura;
        private readonly IServiceEmail _serviceEmail;

        public CheckoutController(IServiceProducto serviceProducto, IServiceUsuario serviceUsuario,
            IServicePedido servicePedido, IServicePedidoPersonalizado servicePersonalizado,
            IServicePedidoProducto servicePedidoProducto, IServiceFactura serviceFactura, IServiceEmail serviceEmail)
        {
            _serviceProducto = serviceProducto;
            _serviceUsuario = serviceUsuario;
            _servicePedido = servicePedido;
            _servicePersonalizado = servicePersonalizado;
            _servicePedidoProducto = servicePedidoProducto;
            _serviceFactura = serviceFactura;
            _serviceEmail = serviceEmail;
        }

        // GET: /Checkout
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var carrito = HttpContext.Session.GetObject<PedidoSessionDTO>("Pedido") ?? new PedidoSessionDTO();

            if ((carrito.Items == null || !carrito.Items.Any()) &&
                (carrito.ItemsPersonalizados == null || !carrito.ItemsPersonalizados.Any()))
            {
                TempData["Error"] = "Tu carrito está vacío.";
                return RedirectToAction("Index", "Carrito");
            }

            var vm = new CheckoutDTO
            {
                Fecha = DateTime.Now,
                Estado = "Pendiente",
                IdUsuario = 0,
                UsuarioResumen = "Seleccioná usuario…",
                DireccionEnvio = ""
            };

            // Mapear el carrito en sesión a las líneas del checkout
            foreach (var it in carrito.ItemsPersonalizados)
            {
                vm.Lineas.Add(new CheckoutLineaVM
                {
                    LineId = it.LineId,
                    IdProducto = it.IdProducto,
                    EsPersonalizado = true,
                    Nombre = it.NombreProducto,
                    // Precio unitario (sin IVA): costo base + extras seleccionados
                    PrecioUnitario = it.CostoBase + (it.Criterios?.Sum(c => c.CostoExtra) ?? 0m),
                    Cantidad = it.Cantidad,
                    Criterios = it.Criterios ?? new List<SeleccionCriterioDTO>()
                });
            }

            foreach (var item in carrito.Items)
            {
                vm.Lineas.Add(new CheckoutLineaVM
                {
                    LineId = item.LineId,
                    IdProducto = item.IdProducto,
                    EsPersonalizado = false,
                    Nombre = item.NombreProducto,
                    PrecioUnitario = item.PrecioUnitario,
                    Cantidad = item.Cantidad,
                    Criterios = new List<SeleccionCriterioDTO>()
                });
            }
            RecalcularTotales(vm);
            var usuarios = await _serviceUsuario.ListAsync();
            ViewBag.Usuarios = usuarios;
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> UsuarioDetalle(int idUsuario)
        {
            try
            {
                var usuario = await _serviceUsuario.FindByIdAsync(idUsuario);
                if (usuario == null) return Json(new { ok = false, msg = "Usuario no encontrado" });

                return Json(new
                {
                    ok = true,
                    nombre = usuario.Nombre,
                    email = usuario.Correo
                });
            }
            catch
            {
                return Json(new { ok = false, msg = "Error al cargar el usuario" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCantidad(Guid lineId, int cantidad, bool esPersonalizado)
        {
            Console.WriteLine($"lineId: {lineId}, cantidad: {cantidad}, esPersonalizado: {esPersonalizado}");

            var carrito = HttpContext.Session.GetObject<PedidoSessionDTO>("Pedido") ?? new();

            if (esPersonalizado)
            {
                var item = carrito.ItemsPersonalizados.FirstOrDefault(x => x.LineId == lineId);
                if (item == null)
                    return BadRequest("Producto personalizado no encontrado.");

                item.Cantidad = cantidad;

                var extras = (item.Criterios?.Sum(c => c.CostoExtra) ?? 0m);
                var unit = item.CostoBase + extras;
                var sub = unit * cantidad;
                var iva = Math.Round(sub * _TASA, 2);
                item.TotalLinea = sub + iva;
            }
            else
            {
                var item = carrito.Items.FirstOrDefault(x => x.LineId == lineId);
                if (item == null)
                    return BadRequest("Producto normal no encontrado.");

                item.Cantidad = cantidad;
                var sub = item.PrecioUnitario * cantidad;
                var iva = Math.Round(sub * _TASA, 2);
                item.TotalLinea = sub + iva;
            }

            // Recalcular totales del carrito
            carrito.Subtotal =
                (carrito.Items?.Sum(it => it.PrecioUnitario * it.Cantidad) ?? 0m)
              + (carrito.ItemsPersonalizados?.Sum(it =>
                    (it.CostoBase + (it.Criterios?.Sum(c => c.CostoExtra) ?? 0m)) * it.Cantidad
                ) ?? 0m);

            carrito.IVA = Math.Round(carrito.Subtotal * _TASA, 2);
            carrito.Total = carrito.Subtotal + carrito.IVA;

            HttpContext.Session.SetObject("Pedido", carrito);

            return Ok();
        }


        //// POST: /Checkout/UpdateCantidad  (editar cantidad o eliminar con cantidad=0)
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult UpdateCantidad(Guid lineId, int cantidad)
        //{
        //    var carrito = HttpContext.Session.GetObject<PedidoSessionDTO>("Pedido") ?? new PedidoSessionDTO();
        //    var item = carrito.ItemsPersonalizados.FirstOrDefault(x => x.LineId == lineId);
        //    if (item == null)
        //    {
        //        TempData["Error"] = "Línea no encontrada.";
        //        return RedirectToAction(nameof(Index));
        //    }

        //    if (cantidad <= 0)
        //    {
        //        // Eliminar línea
        //        carrito.ItemsPersonalizados = carrito.ItemsPersonalizados.Where(x => x.LineId != lineId).ToList();
        //    }
        //    else
        //    {
        //        // Actualizar cantidad y total línea
        //        item.Cantidad = cantidad;
        //        var extras = item.Criterios.Sum(c => c.CostoExtra);
        //        var unit = item.CostoBase + extras;
        //        var sub = unit * item.Cantidad;
        //        var iva = Math.Round(sub * _TASA, 2);
        //        item.TotalLinea = sub + iva;
        //    }

        //    // Recalcular totales del carrito en sesión
        //    carrito.Subtotal = carrito.ItemsPersonalizados.Sum(it => (it.CostoBase + it.Criterios.Sum(c => c.CostoExtra)) * it.Cantidad);
        //    carrito.IVA = Math.Round(carrito.Subtotal * _TASA, 2);
        //    carrito.Total = carrito.Subtotal + carrito.IVA;

        //    HttpContext.Session.SetObject("Pedido", carrito);
        //    return RedirectToAction(nameof(Index));
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirmar(CheckoutDTO dto)
        {
            var carrito = HttpContext.Session.GetObject<PedidoSessionDTO>("Pedido") ?? new PedidoSessionDTO();
            dto.Lineas = new List<CheckoutLineaVM>();

            foreach (var it in carrito.ItemsPersonalizados)
            {
                dto.Lineas.Add(new CheckoutLineaVM
                {
                    LineId = it.LineId,
                    IdProducto = it.IdProducto,
                    EsPersonalizado = true,
                    Nombre = it.NombreProducto,
                    PrecioUnitario = it.CostoBase + (it.Criterios?.Sum(c => c.CostoExtra) ?? 0m),
                    Cantidad = it.Cantidad,
                    Criterios = it.Criterios ?? new List<SeleccionCriterioDTO>()
                });
            }

            foreach (var item in carrito.Items)
            {
                dto.Lineas.Add(new CheckoutLineaVM
                {
                    LineId = Guid.NewGuid(),
                    IdProducto = item.IdProducto,
                    EsPersonalizado = false,
                    Nombre = item.NombreProducto,
                    PrecioUnitario = item.PrecioUnitario,
                    Cantidad = item.Cantidad,
                    Criterios = new List<SeleccionCriterioDTO>()
                });
            }

            // Validaciones personalizadas
            if (!dto.Lineas.Any())
                ModelState.AddModelError("", "Debes tener al menos una línea en el pedido.");

            if (dto.Lineas.Any(l => l.Cantidad <= 0))
                ModelState.AddModelError("", "Las cantidades deben ser mayores a cero.");

            foreach (var l in dto.Lineas)
            {
                var prod = await _serviceProducto.FindByIdAsync(l.IdProducto);
                if (prod == null)
                    ModelState.AddModelError("", $"El producto {l.IdProducto} no existe.");
                else if (prod.Stock < l.Cantidad)
                    ModelState.AddModelError("", $"Stock insuficiente para '{prod.Nombre}'. Disponible: {prod.Stock}");
            }

            // Recalcular totales (para que estén disponibles antes de validación de monto)
            RecalcularTotales(dto);

            if (string.Equals(dto.MetodoPago, "Efectivo", StringComparison.OrdinalIgnoreCase))
            {
                if (dto.MontoEfectivo.HasValue && dto.MontoEfectivo.Value < dto.Total)
                    ModelState.AddModelError("", "El monto en efectivo debe ser mayor o igual al total del pedido.");
            }
            else
            {
                if (!EsTarjetaValida(dto.NumeroTarjeta ?? ""))
                    ModelState.AddModelError("", "Número de tarjeta inválido (Luhn/longitud).");

                if (!EsExpValida(dto.ExpiracionMMYY ?? ""))
                    ModelState.AddModelError("", "Fecha de expiración inválida. Use MM/AA y que no esté vencida.");

                if (!EsCVVValido(dto.CVV ?? ""))
                    ModelState.AddModelError("", "CVV inválido (3 o 4 dígitos).");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Usuarios = await _serviceUsuario.ListAsync();
                return View("Index", dto);
            }

            // Guardar pedido
            var pedido = new PedidoDTO
            {
                IdUsuario = dto.IdUsuario,
                FechaPedido = DateTime.Now,
                Estado = "Pagado",
                DireccionEnvio = dto.DireccionEnvio,
                MetodoPago = dto.MetodoPago
            };

            var idPedido = await _servicePedido.AddAsync(pedido);

            foreach (var it in carrito.ItemsPersonalizados)
            {
                var personalizado = new PedidoPersonalizadoDTO
                {
                    IdPedido = idPedido,
                    IdProducto = it.IdProducto,
                    NombreProductoPersonalizado = it.NombreProducto,
                    CostoBase = it.CostoBase,
                    Criterios = (it.Criterios ?? new List<SeleccionCriterioDTO>())
                        .Select(c => new PedidoPersonalizadoCriterioDTO
                        {
                            NombreCriterio = c.NombreCriterio,
                            OpcionSeleccionada = c.OpcionSeleccionada,
                            CostoExtra = c.CostoExtra,
                            Seleccionado = true
                        }).ToList()
                };

                await _servicePersonalizado.AddPersonalizadoEnPedidoExistenteAsync(idPedido, personalizado);
            }

            foreach (var item in carrito.Items)
            {
                await _servicePedidoProducto.AddAsync(new PedidoProductoDTO
                {
                    IdPedido = idPedido,
                    IdProducto = item.IdProducto,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.PrecioUnitario
                });
            }

            // Enviar factura
            string? correoDestino = null;
            var usuarioDestino = await _serviceUsuario.FindByIdAsync(dto.IdUsuario);
            if (usuarioDestino != null && !string.IsNullOrWhiteSpace(usuarioDestino.Correo))
            {
                correoDestino = usuarioDestino.Correo;
                await _serviceFactura.EnviarFacturaPorCorreoAsync(correoDestino, idPedido);
            }

            HttpContext.Session.Remove("Pedido");
            TempData["Success"] = "¡Pedido registrado y pago procesado correctamente!";

            return RedirectToAction(nameof(Gracias), new { id = idPedido, correoDestino });
        }



        //[HttpGet]
        //public IActionResult Gracias(int id)
        //{
        //    ViewBag.IdPedido = id;
        //    return View();
        //}
        [HttpGet]
        public async Task<IActionResult> Gracias(int id)
        {
            //var pedido = await _servicePedido.FindByIdAsync(id);

            ViewBag.IdPedido = id;

            // Asegurate de que venga la propiedad Correo
            //ViewBag.CorreoDestino = pedido?.IdUsuarioNavigation?.Correo;

            return View();
        }


        // ----------------- Helpers -----------------
        private void RecalcularTotales(CheckoutDTO dto)
        {
            var sub = (dto.Lineas ?? new List<CheckoutLineaVM>())
                        .Sum(l => l.PrecioUnitario * l.Cantidad);
            dto.Subtotal = sub;
            dto.IVA = Math.Round(sub * _TASA, 2);
            dto.Total = dto.Subtotal + dto.IVA;
        }

        private bool EsTarjetaValida(string numRaw)
        {
            if (string.IsNullOrWhiteSpace(numRaw)) return false;
            var digits = new string(numRaw.Where(char.IsDigit).ToArray());
            if (digits.Length < 13 || digits.Length > 19) return false;

            // Luhn
            int sum = 0; bool alt = false;
            for (int i = digits.Length - 1; i >= 0; i--)
            {
                int n = digits[i] - '0';
                if (alt) { n *= 2; if (n > 9) n -= 9; }
                sum += n;
                alt = !alt;
            }
            return sum % 10 == 0;
        }

        private bool EsExpValida(string mmYY)
        {
            // formato MM/AA y fecha >= hoy
            if (string.IsNullOrWhiteSpace(mmYY) || mmYY.Length != 5 || mmYY[2] != '/') return false;
            if (!int.TryParse(mmYY[..2], out var mm)) return false;
            if (!int.TryParse(mmYY.Substring(3, 2), out var yy)) return false;
            if (mm < 1 || mm > 12) return false;

            var year = 2000 + yy;
            var lastDay = new DateTime(year, mm, DateTime.DaysInMonth(year, mm));
            return lastDay >= DateTime.Today;
        }

        private bool EsCVVValido(string cvv)
            => !string.IsNullOrWhiteSpace(cvv) && cvv.All(char.IsDigit) && (cvv.Length == 3 || cvv.Length == 4);


        //PDF
        public async Task<IActionResult> DescargarFactura(int idPedido)
        {
            var pdfBytes = await _serviceFactura.GenerarFacturaPdfAsync(idPedido);
            if (pdfBytes == null)
            {
                TempData["Error"] = "No se pudo generar la factura.";
                return RedirectToAction("Gracias", new { idPedido });
            }

            return File(pdfBytes, "application/pdf", $"Factura_{idPedido}.pdf");
        }

        public IActionResult ProbarPdf()
        {
            try
            {
                var pdfBytes = _serviceFactura.GenerarEjemploPdf();
                return File(pdfBytes, "application/pdf", "ejemplo_factura.pdf");
            }
            catch (Exception ex)
            {
                return Content($"Ocurrió un error al generar el PDF:\n\n{ex.Message}");
            }
        }

        //Correo con factura
        [HttpPost]
        public async Task<IActionResult> EnviarFacturaPorCorreo(int id)
        {
            try
            {
                var pedido = await _servicePedido.FindByIdAsync(id);
                if (pedido == null)
                    return NotFound("Pedido no encontrado.");

                // Generar el PDF
                var pdfBytes = await _serviceFactura.GenerarFacturaPdfAsync(id);

                // Buscar correo del usuario
                var usuario = await _serviceUsuario.FindByIdAsync(pedido.IdUsuario);
                if (usuario == null || string.IsNullOrEmpty(usuario.Correo))
                    return BadRequest("El correo del usuario no está disponible.");

                // Enviar correo
                string asunto = "Factura de tu compra en Bella Estilo";
                string cuerpo = $"<p>¡Hola {usuario.Nombre}!</p>" +
                                $"<p>Gracias por tu compra. Adjuntamos la factura de tu pedido <strong>#{pedido.IdPedido}</strong>.</p>" +
                                $"<p>Saludos,<br/>Equipo Bella Estilo</p>";

                //await _serviceFactura.EnviarFacturaPorCorreoAsync(usuario.Correo, asunto, cuerpo, pdfBytes, $"Factura_{pedido.IdPedido}.pdf");
                await _serviceFactura.EnviarFacturaPorCorreoAsync(pedido.IdUsuarioNavigation.Correo, pedido.IdPedido);


                TempData["MensajeCorreo"] = "Factura enviada al correo exitosamente.";
                return RedirectToAction("Gracias", new { id = pedido.IdPedido });
            }
            catch (Exception ex)
            {
                TempData["MensajeCorreo"] = "Ocurrió un error al enviar el correo: " + ex.Message;
                return RedirectToAction("Gracias", new { id = id });
            }
        }



    }
}


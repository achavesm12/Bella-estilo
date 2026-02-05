using BelaEstilo.Application.DTOs;
using BelaEstilo.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace BelaEstilo.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly IServiceUsuario _serviceUsuario;

        public LoginController(IServiceUsuario serviceUsuario)
        {
            _serviceUsuario = serviceUsuario;
        }

        public IActionResult AccesoDenegado()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuario = await _serviceUsuario.AutenticarUsuarioAsync(model.Correo, model.Contrasenna);
            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "Credenciales inválidas.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home"); // Redirige después del login exitoso
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index");
        }
    }
}

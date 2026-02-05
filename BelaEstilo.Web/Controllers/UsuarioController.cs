using BelaEstilo.Application.DTOs;
using BelaEstilo.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BelaEstilo.Web.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IServiceUsuario _service;

        public UsuarioController(IServiceUsuario service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = await _service.ListAsync();
            return View(usuarios);
        }

        // GET: Usuario/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioRegistroDTO dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _service.AddAsync(dto);
                    return RedirectToAction(nameof(Index)); 
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return View(dto);
        }

        // GET: Usuario/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var usuario = await _service.FindByIdAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UsuarioDTO dto)
        {
            if (id != dto.IdUsuario)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                await _service.UpdateAsync(id, dto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al actualizar: {ex.Message}");
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

    }
}

using BelaEstilo.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BelaEstilo.Web.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly IServiceCategoria _serviceCategoria;

        public CategoriaController(IServiceCategoria serviceCategoria)
        {
            _serviceCategoria = serviceCategoria;
        }

        public async Task<IActionResult> Index()
        {
            var categoriasDTO = await _serviceCategoria.ListAsync();

            var categoriasConImagen = categoriasDTO.Select(c => new CategoriaConImagen
            {
                IdCategoria = c.IdCategoria,
                Nombre = c.Nombre,
                ImagenUrl = ObtenerImagenParaCategoria(c.Nombre)
            }).ToList();

            return View(categoriasConImagen); // Vista: Views/Categoria/Index.cshtml
        }

        private string ObtenerImagenParaCategoria(string nombre)
        {
            return nombre.ToLower() switch
            {
                "vestidos" => "/images/categorias/vestidos.jpg",
                "blusas" => "/images/categorias/blusas.jpg",
                "pantalones" => "/images/categorias/pantalones.jpg",
                "accesorios" => "/images/categorias/accesorios.jpg",
                "deportivas" => "/images/categorias/deportivas.jpg",
                "faldas" => "/images/categorias/faldas.jpg",

                _ => "/images/categorias/default.jpg"
            };
        }
    }

    public class CategoriaConImagen
    {
        public int IdCategoria { get; set; }
        public string Nombre { get; set; }
        public string ImagenUrl { get; set; } = string.Empty;
    }
}


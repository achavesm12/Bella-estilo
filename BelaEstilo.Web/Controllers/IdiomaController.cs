using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace BelaEstilo.Web.Controllers
{
    public class IdiomaController : Controller
    {
        
        [HttpPost]
        public IActionResult Seleccionar(string culture, string returnUrl)
        {
            //Guarda la cultura seleccionada en una coockie
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(
                    new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }
    }
}

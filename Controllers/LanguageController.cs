using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace AutoSalon.Controllers
{
    public class LanguageController : Controller
    {
        /// <summary>
        /// Переключает язык сайта и перенаправляет обратно на страницу откуда пришёл запрос.
        /// Вызывается по маршруту: GET /language/set?culture=ru  (или kk, en)
        /// </summary>
        [HttpGet]
        public IActionResult Set(string culture, string returnUrl = "/")
        {
            // Допустимые культуры
            var allowed = new[] { "ru", "kk", "en" };
            if (!allowed.Contains(culture))
                culture = "ru";

            // Сохраняем выбор в cookie — ASP.NET Core читает его автоматически
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    IsEssential = true,
                    HttpOnly = false,
                    SameSite = SameSiteMode.Lax
                }
            );

            // Защита от open redirect — только локальные URL
            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = "/";

            return LocalRedirect(returnUrl);
        }
    }
}

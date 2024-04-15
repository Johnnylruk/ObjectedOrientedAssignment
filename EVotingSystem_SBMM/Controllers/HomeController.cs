using EVotingSystem_SBMM.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Globalization;
using EVotingSystem_SBMM.Data;
using EVotingSystem_SBMM.Filters;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;

namespace EVotingSystem_SBMM.Controllers
{
    [UserLoggedPage]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ChangeLanguage(string culture)
        {
            // Set the culture cookie
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });
            // Redirect back to the previous page
            return Redirect(Request.Headers["Referer"].ToString());
        }
        
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View(new ErrorViewModel { RequestId = requestId });
        }
    }
}
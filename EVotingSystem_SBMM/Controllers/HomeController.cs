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
        private readonly ILogger<HomeController> _logger;
        private readonly IStringLocalizer<HomeController> _stringLocalizer;
        private readonly EVotingSystemDB _evotingSystemDB;
        
        public HomeController(ILogger<HomeController> logger, EVotingSystemDB eVotingSystemDB)
        {
            _logger = logger;
            _evotingSystemDB = eVotingSystemDB;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ChangeLanguage(string culture)
        {
            if (!string.IsNullOrEmpty(culture))
            {
                // Perform your verification logic here
                // For example:
                // Check if the user is authenticated
                if (User.Identity.IsAuthenticated)
                {
                    // Your verification logic
                }
            }

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
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public ActionResult GetData()
        {
            var votes = _evotingSystemDB.Votes.ToList();
            return Json(new { data = votes });
        }


    }
}
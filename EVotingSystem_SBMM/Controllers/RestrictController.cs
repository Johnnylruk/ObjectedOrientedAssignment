using EVotingSystem_SBMM.Filters;
using Microsoft.AspNetCore.Mvc;

namespace EVotingSystem_SBMM.Controllers;

[UserLoggedPage]
public class RestrictController : Controller
{
    
    public IActionResult Index()
    {
        return View();
    }
}
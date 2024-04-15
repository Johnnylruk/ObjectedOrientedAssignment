using EVotingSystem_SBMM.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace EVotingSystem_SBMM.ViewComponents;

public class Menu : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        string userSession = HttpContext.Session.GetString("userLoggedSession");
        if (string.IsNullOrEmpty(userSession)) return null;
        UserModel user = JsonConvert.DeserializeObject<UserModel>(userSession);
        return View(user);
    }
}
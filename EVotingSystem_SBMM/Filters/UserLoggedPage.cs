using EVotingSystem_SBMM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace EVotingSystem_SBMM.Filters;

public class UserLoggedPage : ActionFilterAttribute
{
    //Verifying if the user is logged
    //if is not redirect to login.
    // if it is override.
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        string userSession = context.HttpContext.Session.GetString("userLoggedSession");
        if (string.IsNullOrEmpty(userSession))
        {
            context.Result = new RedirectToRouteResult(new RouteValueDictionary{ {"controller" , "login"} , {"action" , "Index"} });
        }
        else
        {
            UserModel userModel = JsonConvert.DeserializeObject<UserModel>(userSession);
            if (userModel == null)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary{ {"controller" , "login"} , {"action" , "Index"} });
            }
        }
        base.OnActionExecuted(context);
    }
}
using EVotingSystem_SBMM.Enums;
using EVotingSystem_SBMM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace EVotingSystem_SBMM.Filters;

public class ElectoralAdministratorRestrictPage : ActionFilterAttribute
{
    //Verifying if the user is logged
    //if is not redirect to login.
    // if it is override.
    public override void OnActionExecuting(ActionExecutingContext  context)
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
                context.Result = new RedirectToRouteResult(new RouteValueDictionary{ { "controller" , "login"} , {"action" , "Index"} });
            }
            if (context.RouteData.Values["controller"].ToString().ToLower() == "audit")
            {
                // Allow access for both Electoral Administrators and Third-Party Auditors
                if (userModel.Profile == ProfileEnum.ElectoralAdministrator || userModel.Profile == ProfileEnum.ThirdPartAuditor)
                {
                    base.OnActionExecuting(context); // Allow access
                    return;
                }
            }
            // For other controllers, restrict access to Electoral Administrators only
            if (userModel.Profile != ProfileEnum.ElectoralAdministrator)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Restrict" }, { "action", "Index" } });
                return;
            }
        }
        base.OnActionExecuting(context);
    }
}

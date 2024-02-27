using System.Net;
using EVotingSystem_SBMM.Helper;
using EVotingSystem_SBMM.Models;
using EVotingSystem_SBMM.Repository;
using Microsoft.AspNetCore.Mvc;

namespace EVotingSystem_SBMM.Controllers;

public class LoginController : Controller
{
    private readonly IUserSession _userSession;
    private readonly ILoginRepository _loginRepository;
    private readonly IHttpContextAccessor _contextAccessor;
    
    public LoginController(IUserSession userSession,
                           ILoginRepository loginRepository, IHttpContextAccessor contextAccessor
                           )
    {
        _userSession = userSession;
        _loginRepository = loginRepository;
        _contextAccessor = contextAccessor;
    }
    public IActionResult Index()
    {
        //If user is logged I am redirecting him to home.
        if (_userSession.GetUserSession() != null) return RedirectToAction("Index", "Home");
        return View();
    }

    public IActionResult LoggedOut()
    {
        _userSession.RemoveLoginSession();
        _contextAccessor.HttpContext.Response.Cookies.Delete(".AspNet.Consent");
        return RedirectToAction("Index", "Login");
    }
   
    public dynamic FindProfile(string login, string password)
    {
        var user = _loginRepository.GetUserByLogin(login);
        string hashPassword = password.GenerateHash();
        
        if (user != null && PasswordHandle.ValidatePassword(hashPassword, user.Password))
            {
                return user;
            }

        var voter = _loginRepository.GetVoterByLogin(login);
        if (voter != null && PasswordHandle.ValidatePassword(hashPassword, voter.Password))
        {
            return voter;
        }
        
        return null;
    }
    public IActionResult LogIn(LoginModel loginModel)
    {
        try
        {
            var Profile = FindProfile(loginModel.Login, loginModel.Password);
            if (Profile != null)
            {
                if (Profile is UserModel userModel)
                {
                    _userSession.CreateSession(userModel, null);
                    return RedirectToAction("Index", "Home");
                }
                else if (Profile is VoterModel voterModel)
                {
                    _userSession.CreateSession(null, voterModel); 
                    return RedirectToAction("Index", "AccessVoter");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid Password";
                
            }
            TempData["ErrorMessage"] = "Invalid User/Password";

            return View("Index");
        }
        catch (Exception error)
        {
            TempData["ErrorMessage"] = $"Ops, We could login.{error.Message}";
            return RedirectToAction("Index");
        }
    }

    
  
}

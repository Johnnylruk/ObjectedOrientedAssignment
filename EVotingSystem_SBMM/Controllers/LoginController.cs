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
    private readonly IPasswordHandle _passwordHandle;
    public LoginController(IUserSession userSession, ILoginRepository loginRepository, IPasswordHandle passwordHandle)
    {
        _userSession = userSession;
        _loginRepository = loginRepository;
        _passwordHandle = passwordHandle;
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
        return RedirectToAction("Index", "Login");
    }
   
    public dynamic FindProfile(string login, string password)
    {
        var user = _loginRepository.GetUserByLogin(login);
        var result = new LoginModel();
        string hashPassword = password.GenerateHash();

        if (user != null && _passwordHandle.ValidatePassword(hashPassword, user.Password))
        {
            return user;
        }
    
        var voter = _loginRepository.GetVoterByLogin(login);
        if (voter != null && _passwordHandle.ValidatePassword(hashPassword, voter.Password))
        {
            if (voter.IsPending)
            {
                return result.ApprovalPending;
            }else
            {
                return voter;
            }
        }
        else
        {
            result.IsInvalidCredentials = true;
        }
        return result;
    }
    
    public IActionResult LogIn(LoginModel loginModel)
    {
        try
        {
            var Profile = FindProfile(loginModel.Login, loginModel.Password);
            if (loginModel.IsInvalidCredentials == false)
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
            else if (loginModel.ApprovalPending)
            {
                TempData["ErrorMessage"] = "Approval Pending, you will receive an email once you have been approved.";
                return View("Index");
            }
            if(loginModel.IsInvalidCredentials)
            {
                TempData["ErrorMessage"] = "Invalid Password.";
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid User/Password.";
            }
            return View("Index");
        }
        catch (Exception error)
        {
            TempData["ErrorMessage"] = $"Ops, We could login.{error.Message}";
            return RedirectToAction("Index");
        }
    }
}

using EVotingSystem_SBMM.Helper;
using EVotingSystem_SBMM.Models;
using EVotingSystem_SBMM.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EVotingSystem_SBMM.Controllers;

public class PasswordController : Controller
{
    private readonly IUsersRepository _usersRepository;
    private readonly IUserSession _userSession;
    private readonly IEmail _email;
    public PasswordController(IUsersRepository usersRepository, IUserSession userSession, IEmail email)
    {
        _usersRepository = usersRepository;
        _userSession = userSession;
        _email = email;
    }
    public IActionResult ChangePassword()
    {
        return View();
    }

    public IActionResult ResetPassword()
    {
        return View();
    }
    
    [HttpPost]
    public IActionResult ChangeUserPassword(ChangePasswordModel changePasswordModel)
    {
        try
        {
           UserModel userLogged =  _userSession.GetUserSession();
           changePasswordModel.Id = userLogged.Id;

            if (ModelState.IsValid)
            {
                _usersRepository.ChangePassword(changePasswordModel);
                TempData["SuccessMessage"] = "Password has been successful updated.";
                return RedirectToAction("ChangePassword", changePasswordModel);
            }
            TempData["ErrorMessage"] = "Error trying to update your password.";
            return View("ChangePassword", changePasswordModel);
        }
        catch (Exception error)
        {
            TempData["ErrorMessage"] = $"We could not change your password, please try again. {error.Message}";
            return View("ChangePassword", changePasswordModel);
        }
    }
    [HttpPost]
    public IActionResult SendResetPasswordLink(ResetPasswordModel resetPasswordModel)
    {
        try
        {

            if (ModelState.IsValid)
            {
                UserModel userModel = _usersRepository.GetByLoginAndEmail(resetPasswordModel.Login, resetPasswordModel.Email);

                if (userModel != null)
                {

                    string newPassword = PasswordHandle.GenerateNewPassword();
                    string message = $"Your new password is: {newPassword}";
                    
                    bool sentEmail = _email.SendEmailLink(userModel.Email,"EVoting System SBMM - New Password", message);

                    if (sentEmail)
                    {
                        string hashedPassword = PasswordHandle.HashPassword(newPassword);
                        userModel.Password = hashedPassword;
                        _usersRepository.UpdateUser(userModel);
                        TempData["SuccessMessage"] = "We have sent a new password to your email.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "We could not sent an email, please try again.";
                    }
                    return RedirectToAction("Index" , "Login");
                }
            }
            return View("ChangePassword");
        }
        catch (Exception error)
        {
            TempData["ErrorMessage"] = $"Ops, We could not reset your password.{error.Message}";
            return RedirectToAction("ChangePassword");
        }
    }
}
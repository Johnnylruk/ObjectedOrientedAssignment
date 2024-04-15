using EVotingSystem_SBMM.Filters;
using EVotingSystem_SBMM.Models;
using EVotingSystem_SBMM.Repository;
using Microsoft.AspNetCore.Mvc;

namespace EVotingSystem_SBMM.Controllers;

[ElectoralAdministratorRestrictPage]

public class ElectoralAdminUserController : Controller
{
    private readonly IUsersRepository _usersRepository;

    public ElectoralAdminUserController(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }
         public IActionResult Index()
        {
            List<UserModel> users = _usersRepository.GetAll();

            return View(users);
        }
        public IActionResult CreateUser()
        {
            return View();
        }
        public IActionResult EditUser(int id)
        {
            UserModel userModel = _usersRepository.GetUserById(id);
            return View(userModel);
        }
        public IActionResult DeleteUser(int id)
        {
            UserModel userModel = _usersRepository.GetUserById(id);
            return View(userModel);
        }
        
        [HttpPost]
        public IActionResult CreateUser(UserModel user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _usersRepository.Register(user);
                    TempData["SuccessMessage"] = "User has been created.";
                    return RedirectToAction("Index");
                }
                return View(user);
            }
            catch (Exception error)
            {
                TempData["ErrorMessage"] = $"Ops, could not create a user. Please try again. Error details: {error.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult EditUser(UserWithOutPwdModel userWithOutPwdModel)
        {
            UserModel userModel = null;

            try
            {
                if (ModelState.IsValid)
                {
                    userModel = new UserModel()
                    {
                        Id = userWithOutPwdModel.Id,
                        Name = userWithOutPwdModel.Name,
                        Login = userWithOutPwdModel.Login,
                        Email = userWithOutPwdModel.Email,
                        Profile = userWithOutPwdModel.Profile
                    };
                    _usersRepository.UpdateUser(userModel);
                    TempData["SuccessMessage"] = "User has been updated.";
                    return RedirectToAction("Index");
                }
                return View(userModel);
            }
            catch (Exception error)
            {
                TempData["ErrorMessage"] = $"Ops, could not update a user. Please try again. Error details: {error.Message}";
                return RedirectToAction("Index");
            }
        }
        
        [HttpPost]
        public IActionResult DeleteUser(UserModel userModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool deleted = _usersRepository.DeleteUser(userModel.Id);
                    if (deleted)
                    {
                        TempData["SuccessMessage"] = "User has been deleted.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "User not found.";
                    }
                    return RedirectToAction("Index");    
                }
                return View(userModel);
            }
            catch (Exception error)
            {
                TempData["ErrorMessage"] = $"Ops, could not delete a user. Please try again. Error details: {error.Message}";
                return RedirectToAction("Index");
            }
        }
}
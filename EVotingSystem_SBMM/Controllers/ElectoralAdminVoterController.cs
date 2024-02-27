using EVotingSystem_SBMM.Filters;
using EVotingSystem_SBMM.Models;
using EVotingSystem_SBMM.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EVotingSystem_SBMM.Controllers
{
    [ElectoralAdministratorRestrictPage]
    public class ElectoralAdminVoterController : Controller
    {
        private readonly IVotersRepository _votersRepository;
        
        public ElectoralAdminVoterController(IVotersRepository votersRepository)
        {
            _votersRepository = votersRepository;
        }

        //Voter Administration
        public IActionResult Index()
        {
            List<VoterModel> voters =_votersRepository.GetAll();

            return View(voters);
        }

        public IActionResult Details(int id)
        {
            VoterModel voterModel = _votersRepository.Details(id);
            return View(voterModel);
        }

        public IActionResult CreateVoter()
        {
            return View();
        }
        public IActionResult EditVoter(int id)
        {
            VoterModel voterModel = _votersRepository.GetVoterbyId(id);
            return View(voterModel);
        }
        public IActionResult DeleteVoter(int id)
        {
            VoterModel voterModel = _votersRepository.GetVoterbyId(id);
            return View(voterModel);
        }

        [HttpPost]
        public IActionResult CreateVoter(VoterModel voter)
        {
            try
            {
                    _votersRepository.Register(voter);
                    TempData["SuccessMessage"] = "Voter has been created.";
                    return RedirectToAction("Index");
            }
            catch (Exception error)
            {
                TempData["ErrorMessage"] = $"Ops, could not create a voter. Please try again. Error details: {error.Message}";
                return RedirectToAction("Index");
            }
            
        }
        [HttpPost]
        public IActionResult UpdateVoter(VoterModel voter)
        {
            try
            {
                    _votersRepository.UpdateVoter(voter);
                    TempData["SuccessMessage"] = "Voter has been updated.";
                    return RedirectToAction("Index");

            }
            catch (Exception error)
            {
                TempData["ErrorMessage"] = $"Ops, could not update a voter. Please try again. Error details: {error.Message}";
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult DeleteVoter(VoterModel voter)
        {
            try
            {
                bool deleted = _votersRepository.DeleteVoter(voter.Id);
                if (deleted)
                {
                    TempData["SuccessMessage"] = "Voter has been deleted.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Voter not found.";
                }
                return RedirectToAction("Index");

            }
            catch (Exception error)
            {

                TempData["ErrorMessage"] = $"Ops, could not delete a voter. Please try again. Error details: {error.Message}";
                return RedirectToAction("Index");
            }

        }
      
        
    }
}

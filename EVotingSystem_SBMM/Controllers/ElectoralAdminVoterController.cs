using EVotingSystem_SBMM.Filters;
using EVotingSystem_SBMM.Models;
using EVotingSystem_SBMM.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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
            var displayVoters = voters.Where(v => v.IsPending == false || v.IsPending == null).ToList();
            int pendingVotersCount = voters.Count(v => v.IsPending);
         
            ViewBag.PendingVotersCount = pendingVotersCount;

            return View(displayVoters);
        }

        public IActionResult Details(int id)
        {
            VoterModel voterModel = _votersRepository.Details(id);
            return View(voterModel);
        }

        public IActionResult PendingVoters()
        {
            List<VoterModel> pendingVoters =_votersRepository.GetAll()
                .Where(v => v.IsPending || v.IsPending == null).ToList();
            return View(pendingVoters);
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
        public IActionResult ApproveVoters(VoterModel voter)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _votersRepository.Register(voter);
                    TempData["SuccessMessage"] = "Voter has been approved.";
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }
            catch (Exception error)
            {
                TempData["ErrorMessage"] =
                    $"Ops, could not create a voter. Please try again. Error details: {error.Message}";
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

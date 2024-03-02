using EVotingSystem_SBMM.Filters;
using EVotingSystem_SBMM.Helper;
using EVotingSystem_SBMM.Models;
using EVotingSystem_SBMM.Repository;
using Microsoft.AspNetCore.Mvc;

namespace EVotingSystem_SBMM.Controllers
{
    [ElectoralAdministratorRestrictPage]
    public class ElectoralAdminVoterController : Controller
    {
        private readonly IVotersRepository _votersRepository;
        private readonly IEmail _email;
        public ElectoralAdminVoterController(IVotersRepository votersRepository, IEmail email)
        {
            _votersRepository = votersRepository;
            _email = email;
        }

        //Voter Administration
        public IActionResult Index()
        {
            List<VoterModel> voters =_votersRepository.GetAll();
            var displayVoters = voters.Where(v => v.IsPending == false).ToList();
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
                .Where(v => v.IsPending).ToList();
            return View(pendingVoters);
        }
        
        public IActionResult EditVoter(int id)
        {
            VoterModel voterModel = _votersRepository.GetVoterById(id);
            return View(voterModel);
        }
        public IActionResult DeleteVoter(int id)
        {
            VoterModel voterModel = _votersRepository.GetVoterById(id);
            return View(voterModel);
        }

        [HttpPost]
        public IActionResult ApproveVoter(VoterModel voter, int voterId)
        {
            try
            {
                voter = _votersRepository.GetVoterById(voterId);
                string message = "Your voter request has been approved";
                if (voter != null)
                {
                    _votersRepository.ApproveVoterRequest(voter);
                    _email.SendEmailLink(voter.Email,"EVoting System SBMM -" ,message);
                    TempData["SuccessMessage"] = "Voter has been approved.";
                    return RedirectToAction("Index" , "ElectoralAdminVoter");
                }
                TempData["ErrorMessage"] = "Ops, could not find a voter.";
                return RedirectToAction("Index");
            }
            catch (Exception error)
            {
                TempData["ErrorMessage"] =
                    $"Ops, could not approve a voter. Please try again. Error details: {error.Message}";
                return RedirectToAction("Index");
            }
        }
        
        [HttpPost]
        public IActionResult EditVoter(VoterModel voter)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _votersRepository.UpdateVoter(voter);
                    TempData["SuccessMessage"] = "Voter has been updated.";
                    return RedirectToAction("Index");

                }
                return View(voter);
            }
            catch (Exception error)
            {
                TempData["ErrorMessage"] = $"Ops, could not update a voter. Please try again. Error details: {error.Message}";
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult RefuseVoter(VoterModel voter, int voterId)
        {
            try
            {
                voter = _votersRepository.GetVoterById(voterId);
                if (voter != null)
                {
                    _votersRepository.DenyVoterRequest(voter);
                    TempData["SuccessMessage"] = "Voter has been refused and deleted from database.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Ops, could not find a voter.";
                }
                return RedirectToAction("Index");

            }
            catch (Exception error)
            {

                TempData["ErrorMessage"] = $"Ops, could not delete a voter. Please try again. Error details: {error.Message}";
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult DeleteVoter(VoterModel voter)
        {
            try
            {
                if (ModelState.IsValid)
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

                return View(voter);

            }
            catch (Exception error)
            {

                TempData["ErrorMessage"] = $"Ops, could not delete a voter. Please try again. Error details: {error.Message}";
                return RedirectToAction("Index");
            }
        }
      
        
    }
}

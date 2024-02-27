using EVotingSystem_SBMM.Filters;
using EVotingSystem_SBMM.Models;
using EVotingSystem_SBMM.Repository;
using Microsoft.AspNetCore.Mvc;

namespace EVotingSystem_SBMM.Controllers;

[ElectoralAdministratorRestrictPage]
public class ElectoralAdminCandidateController : Controller
{
   private readonly ICandidateRepository _candidateRepository;

    public ElectoralAdminCandidateController(ICandidateRepository candidateRepository)
    {
        _candidateRepository = candidateRepository;
    }
         public IActionResult Index()
        {
            List<CandidateModel> candidates = _candidateRepository.GetAll();
            return View(candidates);
        }
        public IActionResult CreateCandidate()
        {
            return View();
        }
        public IActionResult EditCandidate(int id)
        {
            CandidateModel candidateModel = _candidateRepository.GetCandidateById(id);
            return View(candidateModel);
        }
        public IActionResult DeleteCandidate(int id)
        {
            CandidateModel candidateModel = _candidateRepository.GetCandidateById(id);
            return View(candidateModel);
        }
        
        [HttpPost]
        public IActionResult CreateCandidate(CandidateModel candidateModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _candidateRepository.Register(candidateModel);
                    TempData["SuccessMessage"] = "Candidate has been created.";
                    return RedirectToAction("Index");
                }
                return View(candidateModel);

            }
            catch (Exception error)
            {
                TempData["ErrorMessage"] = $"Ops, could not create a candidate. Please try again. Error details: {error.Message}";
                return RedirectToAction("Index");
            }

        }
        
        [HttpPost]
        public IActionResult EditCandidate(CandidateModel candidateModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _candidateRepository.UpdateCandidate(candidateModel);
                    TempData["SuccessMessage"] = "Candidate has been updated.";
                    return RedirectToAction("Index");
                }
                return View (candidateModel);

            }
            catch (Exception error)
            {
                TempData["ErrorMessage"] = $"Ops, could not update a candidate. Please try again. Error details: {error.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult DeleteCandidate(CandidateModel candidateModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool deleted = _candidateRepository.DeleteCandidate(candidateModel.Id);
                    if (deleted)
                    {
                        TempData["SuccessMessage"] = "Candidate has been deleted.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Candidate not found.";
                    }
                    return RedirectToAction("Index");    
                }

                return View(candidateModel);

            }
            catch (Exception error)
            {

                TempData["ErrorMessage"] = $"Ops, could not delete a candidate. Please try again. Error details: {error.Message}";
                return RedirectToAction("Index");
            }

        }
}
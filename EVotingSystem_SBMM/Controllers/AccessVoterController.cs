using EVotingSystem_SBMM.Enums;
using EVotingSystem_SBMM.Filters;
using EVotingSystem_SBMM.Helper;
using EVotingSystem_SBMM.Models;
using EVotingSystem_SBMM.Repository;
using Microsoft.AspNetCore.Mvc;

namespace EVotingSystem_SBMM.Controllers;


public class AccessVoterController : Controller
{
    private readonly IVotersRepository _votersRepository;
    private readonly ICandidateRepository _candidateRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IUserSession _userSession;
    private readonly IVoteRepository _voteRepository;
    
    public AccessVoterController(IVotersRepository votersRepository, ICandidateRepository candidateRepository, IEventRepository eventRepository,
                                IUserSession userSession, IVoteRepository voteRepository)
    {
        _votersRepository = votersRepository;
        _candidateRepository = candidateRepository;
        _eventRepository = eventRepository;
        _userSession = userSession;
        _voteRepository = voteRepository;
    }
         public IActionResult Index()
         {
            return View();
         }
         
         public IActionResult AccessBallot()
         {
             List<EventModel> activeEvents = _eventRepository.GetAll()
                 .Where(e => e.StartDate <= DateTime.Now && e.EndDate >= DateTime.Now)
                 .ToList();
             if (activeEvents.Any())
             {
                 ViewData["EventId"] = activeEvents.First().EventId;
             }
             return View(activeEvents);
         }
         
         public IActionResult Ballot(int eventId, int candidateId)
         {
             EventModel eventModel = _eventRepository.GetEventById(eventId);
             if (eventModel == null)
             {
                 return RedirectToAction("Index", "Home");
             }

             List<CandidateModel> candidates = _candidateRepository.GetAll();
             string voterCity = _votersRepository.GetVoterCity();

             if (candidates == null || voterCity == null)
             {
                 return View("Index", candidates);
             }

             bool isEventActive = eventModel.StartDate <= DateTime.Now && eventModel.EndDate >= DateTime.Now;

             // Checking if the voter has already voted in this event
             var voterId = _userSession.GetVoterSession().Id;
             bool hasVoted = _voteRepository.HasVoted(voterId, eventId);
            
             candidates = candidates.Where(c => c.City == eventModel.City && c.City == voterCity).ToList();

             ViewData["SelectedEventId"] = eventId; 
             ViewData["SelectedCandidateId"] = candidateId;
             ViewData["IsEventActive"] = isEventActive;
             ViewData["HasVoted"] = hasVoted;
             
             if (eventModel.EventType == EventTypeEnum.FPTP && hasVoted == false)
             {
                 return View("BallotFPTP", candidates);
             }
             if (eventModel.EventType == EventTypeEnum.STV && hasVoted == false)
             {
                 ViewData["HasVoted"] = hasVoted;
                 return View("BallotSTV", candidates);
             }
             if (eventModel.EventType == EventTypeEnum.PV && hasVoted == false)
             {             
                 ViewData["HasVoted"] = hasVoted;
                 return View("BalloPV", candidates);
             }
             
             
             TempData["ErrorMessage"] = "There is no available event for you or you have voted in this event before.";
             return View("Index", candidates);
         }

        public IActionResult Register()
        {
            return View();
        }
     
        public IActionResult UpdateVoter(int id)
        {
            var voterId = _userSession.GetVoterSession();
            VoterModel voterModel = _votersRepository.GetVoterbyId(voterId.Id);
            return View(voterModel);
        }

        public IActionResult VoteStatus()
        {
            var voterId = _userSession.GetVoterSession().Id;

            // Get all events
            List<EventModel> allEvents = _eventRepository.GetAll().ToList();

            // Create a dictionary to store whether the voter has voted for each event
            Dictionary<int, bool> hasVotedForEvent = new Dictionary<int, bool>();

            // Check if the voter has voted for each event
            foreach (var eventModel in allEvents)
            {
                bool hasVoted = _voteRepository.HasVoted(voterId, eventModel.EventId);
                hasVotedForEvent[eventModel.EventId] = hasVoted;
            }

            ViewData["HasVotedForEvent"] = hasVotedForEvent;
            return View(allEvents);
        }

        [HttpPost]
        public IActionResult Register(VoterModel voter)
        {

            VoterModel voterDB = _votersRepository.GetAll().FirstOrDefault(x =>
                x.Passport == voter.Passport ||
                x.Mobile == voter.Mobile ||
                x.Login == voter.Login ||
                x.Email == voter.Email);

            if (voter.BirthDate >= DateTime.Now.AddYears(-16))
            {
                ViewData["BirthDateError"] = "Voter is not older enough to vote.";
                return View();
            }

            if (voterDB != null)
            {
                if (voterDB.Passport == voter.Passport)
                {
                    ViewData["PassportError"] = "Passport is already registered.";
                    return View();
                }
                else if (voterDB.Mobile == voter.Mobile)
                {
                    ViewData["MobileError"] = "Mobile is already registered.";
                    return View();
                }
                else if (voterDB.Login == voter.Login)
                {
                    ViewData["LoginError"] = "Login is already registered.";
                    return View();
                }
                else if (voterDB.Email == voter.Email)
                {
                    ViewData["EmailError"] = "Email is already registered.";
                    return View();
                }
            }

            try
            {
                voter.Profile = ProfileEnum.Voter;
                _votersRepository.Register(voter);
                TempData["SuccessMessage"] = "You have successful registered.";
                return RedirectToAction("Index", "Login");
            }
            catch (Exception error)
            {
                TempData["ErrorMessage"] =
                    $"Ops, we could not register you. Please try again. Error details: {error.Message}";
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpPost]
           public IActionResult UpdateVoter(VoterModel voter)
           {
               try
               {
                   _votersRepository.GetVoterbyId(voter.Id);
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
           
}
using EVotingSystem_SBMM.Helper;
using EVotingSystem_SBMM.Models;
using EVotingSystem_SBMM.Repository;
using Microsoft.AspNetCore.Mvc;

namespace EVotingSystem_SBMM.Controllers;

public class VoteController : Controller
{
    private readonly IUserSession _userSession;
    private readonly IVoteRepository _voteRepository;
    private readonly  ILogger _logger;
    private readonly ICandidateRepository _candidateRepository;
    private readonly IEventRepository _eventRepository;

    public VoteController(IUserSession userSession, IVoteRepository voteRepository, ILogger<VoteController> logger,
                            ICandidateRepository candidateRepository, IEventRepository eventRepository)
    {
        _userSession = userSession;
        _voteRepository = voteRepository;
        _logger = logger;
        _candidateRepository = candidateRepository;
        _eventRepository = eventRepository;
    }

    #region SubmitVote
    
    [HttpPost]
    public IActionResult SubmitVoteSTV(VoteModel vote, int eventId)
    {
        try
        {
            var voterId = _userSession.GetVoterSession();
            EventModel eventModel = _eventRepository.GetEventById(eventId);

            vote.VotedAtTime = DateTime.Now;
            vote.VoterId = voterId.Id;
            vote.EventId = eventModel.EventId;
           // vote.CandidateId = candidateId.Id;
            
            // Ensure there are preferences and each preference has both CandidateId and Rank
            if (vote.Preferences != null && vote.Preferences.All(p => p.CandidateId != 0 && p.Rank != 0))
            {
                // Submit the vote to the repository
                _voteRepository.SubmitVote(vote);

                TempData["SuccessMessage"] = "Votes submitted successfully!";
                return RedirectToAction("Index", "AccessVoter");
            }
            else
            {
                TempData["ErrorMessage"] = "Please rank all candidates before submitting your vote.";
                return RedirectToAction("Error", "Home");
            }
        }
        catch (Exception error)
        {
            TempData["ErrorMessage"] = $"Failed to submit votes: {error.Message}";
            _logger.LogError($"Error occurred while saving votes: {error}");
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpPost]
    public IActionResult SubmitVoteFPTP(int candidateId)
    {
        try
        {
            var voterId = _userSession.GetVoterSession();
            EventModel eventId = _eventRepository.GetActivityEvent();
            
            var vote = new VoteModel()
            {
                VotedAtTime = DateTime.Now,
                VoterId = voterId.Id,
                CandidateId = candidateId,
                EventId = eventId.EventId
            };
        
            _voteRepository.SubmitVote(vote);
            TempData["SuccessMessage"] = "Vote submitted successfully!";
            TempData["SelectedCandidateId"] = candidateId;
            return RedirectToAction("Index", "AccessVoter");
        }
        catch (Exception error)
        {
            TempData["ErrorMessage"] = $"Failed to submit vote: {error.Message}";
            _logger.LogError($"Error occurred while saving vote: {error}");
            return RedirectToAction("Error", "Home");
        }
    }
    
    
    
    
    #endregion

    #region Vote Results

    //Results
    //Single Candidate
    public IActionResult VotesResultFPTP(int id)
    {
        EventModel eventModel = _eventRepository.GetEventById(id);
        List<CandidateModel> candidates = _candidateRepository.GetAll();
        candidates = candidates.Where(c => c.City == eventModel.City).ToList();

        
        var voteCounts = new Dictionary<int, int>();
        
        // Calculate vote count for each candidate
        foreach (var candidate in candidates)
        {
            int voteCount = _voteRepository.GetVoteCountForCandidate(candidate.Id, id);
            voteCounts[candidate.Id] = voteCount;
        }

        int totalVotes = voteCounts.Values.Sum();

        // Find the candidate with the maximum votes
        int maxVoteCount = voteCounts.Values.Max();
        int candidateWithMaxVotesId = voteCounts.FirstOrDefault(x => x.Value == maxVoteCount).Key;

        // Passing necessary data to view
        ViewBag.TotalVotes = totalVotes;
        ViewBag.MaxVoteCount = maxVoteCount;
        ViewBag.CandidateWithMaxVotesId = candidateWithMaxVotesId;
        ViewBag.VoteCounts = voteCounts;
        ViewBag.Candidates = candidates;
    
        return View("IndexResultsFPTP");
    }
    
    
    //The single transferable vote (STV), sometimes known as proportional ranked choice voting (P-RCV), is a multi-winner electoral system in which each voter casts a single vote in the form of a ranked-choice ballot. 
public IActionResult VotesResultSTV(int id)
{
    EventModel eventModel = _eventRepository.GetEventById(id);
    List<CandidateModel> candidates = _candidateRepository.GetAll()
        .Where(c => c.City == eventModel.City)
        .ToList();
    List<VotePreferenceModel> votePreferences = _voteRepository.GetAllVotesPreferential()
        .Where(c => c.EventId == eventModel.EventId).ToList();    
    
    // Initialize vote counts and elected status for each candidate
    var voteCounts = candidates.ToDictionary(candidate => candidate.Id, _ => 0);
    var electedCandidates = new HashSet<int>();
 
    Dictionary<int, int> candidateRanks = new Dictionary<int, int>();

        foreach (var votePreference in votePreferences)
        {
            voteCounts[votePreference.CandidateId] += votePreferences.Count - votePreference.Rank + 1;
        }
       
        VoterCalculationHelper.ElectCandidatesSTV(voteCounts, candidates, electedCandidates, votePreferences.Count / (candidates.Count + 1.0));
       
        int totalVotes = voteCounts.Values.Sum();

        // Find the candidate with the maximum votes
        int maxVoteCount = voteCounts.Values.Max();
        int candidateWithMaxVotesId = voteCounts.FirstOrDefault(x => x.Value == maxVoteCount).Key;

        // Passing necessary data to view
        ViewBag.TotalVotes = totalVotes;
        ViewBag.MaxVoteCount = maxVoteCount;
        ViewBag.CandidateWithMaxVotesId = candidateWithMaxVotesId;
        ViewBag.VoteCounts = voteCounts;
        ViewBag.Candidates = candidates;

        return View("IndexResultsSTV");
}

public IActionResult VotesResultPV(int id)
{
    EventModel eventModel = _eventRepository.GetEventById(id);
    List<CandidateModel> candidates = _candidateRepository.GetAll()
        .Where(c => c.City == eventModel.City)
        .ToList();

    List<VotePreferenceModel> votePreferences = _voteRepository.GetAllVotesPreferential()
        .Where(c => c.EventId == eventModel.EventId).ToList();

    if (votePreferences == null || !votePreferences.Any())
    {
        TempData["ErrorMessage"] = "There are no events to display results";
        return RedirectToAction("Index", "Event");
    }
    
// Initialize vote counts for each candidate
    var voteCounts = candidates.ToDictionary(candidate => candidate.Id, _ => 0);
    foreach (var votePreference in votePreferences)
    {
        voteCounts[votePreference.CandidateId] += VoterCalculationHelper.CalculateVotesBasedOnRank(votePreference.Rank, candidates.Count);
    }

    // Initialize elected status for each candidate
    var electedCandidates = new HashSet<int>();
 
    List<CandidateModel> candidatesCopy = new List<CandidateModel>(candidates);
    VoterCalculationHelper.ElectCandidatesPV(voteCounts, candidatesCopy, electedCandidates);
    
    int totalVotes = voteCounts.Values.Sum();

    // Find the candidate with the maximum votes
    int maxVoteCount = voteCounts.Values.Max();
    int candidateWithMaxVotesId = voteCounts.FirstOrDefault(x => x.Value == maxVoteCount).Key;

    // Passing necessary data to view
    ViewBag.TotalVotes = totalVotes;
    ViewBag.MaxVoteCount = maxVoteCount;
    ViewBag.CandidateWithMaxVotesId = candidateWithMaxVotesId;
    ViewBag.VoteCounts = voteCounts;
    ViewBag.Candidates = candidates;
    ViewBag.Rank = votePreferences.Where(c => c.Rank == voteCounts.Count);


    // Logging elected candidates
    Console.WriteLine("Elected Candidates:");
    foreach (var candidateId in electedCandidates)
    {
        var candidate = candidates.FirstOrDefault(c => c.Id == candidateId);
        if (candidate != null)
            Console.WriteLine($"{candidate.Name} (ID: {candidate.Id})");
    }

    return View("IndexResultsPV");
}


    #endregion

}
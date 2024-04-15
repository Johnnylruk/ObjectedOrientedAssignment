using EVotingSystem_SBMM.Filters;
using EVotingSystem_SBMM.Models;
using EVotingSystem_SBMM.Repository;
using Microsoft.AspNetCore.Mvc;

namespace EVotingSystem_SBMM.Controllers;

[ElectoralAdministratorRestrictPage]
public class AuditController : Controller
{
    private readonly IEventRepository _eventRepository;
    private readonly IVoteRepository _voteRepository;

    public AuditController(IEventRepository eventRepository, IVoteRepository voteRepository)
    {
        _eventRepository = eventRepository;
        _voteRepository = voteRepository;
    }
    public IActionResult Index()
    {
        List<EventModel> events = _eventRepository.GetAll();
        return View(events);
    }
    public IActionResult AuditVotesPage(int id)
    {
        ViewBag.EventId = id;
        return View();
    }

    public IActionResult AuditPreferenceVotesPage(int id)
    {
        ViewBag.EventId = id;
        return View();
    }
    
    public IActionResult GetVotesForAudit(int id)
    {
        EventModel eventModel = _eventRepository.GetEventById(id);
    
        var votes = _voteRepository.GetVotesByEventId(eventModel.EventId)
            .Where(e => e.EventId == eventModel.EventId)
            .Select(v => new 
            {
                id = v.Id,
                votedAtTime = v.VotedAtTime,
                voterId = v.VoterId,
                candidateId = v.CandidateId,
                eventId = v.EventId
            })
            .ToList();

        return Json(new { data = votes });
    }
    public IActionResult GetPreferencesVoteForAudit(int id)
    {
        EventModel eventModel = _eventRepository.GetEventById(id);

        var votes = _voteRepository.GetPreferenceVotesByEventId(eventModel.EventId)
            .Where(e => e.EventId == eventModel.EventId)
            .Select(v => new
            {
                id = v.Id,
                voteId = v.VoteId,
                candidateId = v.CandidateId,
                rank = v.Rank,
                eventId = v.EventId,
                voterId = v.VoterId
            })
            .ToList();
        
        return Json(new { data = votes });
    }
}
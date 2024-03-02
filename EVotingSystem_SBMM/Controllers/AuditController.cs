using System.Text.Json;
using System.Text.Json.Serialization;
using EVotingSystem_SBMM.Data;
using EVotingSystem_SBMM.Filters;
using EVotingSystem_SBMM.Models;
using EVotingSystem_SBMM.Repository;
using Microsoft.AspNetCore.Mvc;

namespace EVotingSystem_SBMM.Controllers;

[ElectoralAdministratorRestrictPage]
public class AuditController : Controller
{
    private readonly EVotingSystemDB _evotingSystemDB;
    private readonly IEventRepository _eventRepository;
    

    public AuditController(EVotingSystemDB eVotingSystemDb,IEventRepository eventRepository)
    {
        _evotingSystemDB = eVotingSystemDb;
        _eventRepository = eventRepository;
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
    
        var votes = _evotingSystemDB.Votes
            .Where(e => e.EventId == eventModel.EventId)
            .Select(v => new 
            {
                Id = v.Id,
                VotedAtTime = v.VotedAtTime,
                VoterId = v.VoterId,
                CandidateId = v.CandidateId,
                EventId = v.EventId
            })
            .ToList();

        return Json(new { data = votes });
    }
    
    public IActionResult GetPreferencesVoteForAudit(int id)
    {
        EventModel eventModel = _eventRepository.GetEventById( id);

        var votes = _evotingSystemDB.VotePreferences
            .Where(e => e.EventId == eventModel.EventId)
            .ToList();
            
        return Json(new { data = votes });
        
    }
 
}
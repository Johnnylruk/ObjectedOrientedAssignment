using EVotingSystem_SBMM.Filters;
using EVotingSystem_SBMM.Models;
using EVotingSystem_SBMM.Repository;
using Microsoft.AspNetCore.Mvc;

namespace EVotingSystem_SBMM.Controllers;

[ElectoralAdministratorRestrictPage]

public class EventController : Controller
{
    private readonly IEventRepository _eventRepository;

    public EventController(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }
    public IActionResult Index()
    {
        List<EventModel> events = _eventRepository.GetAll();
        return View(events);
    }

    public IActionResult CreateEvent()
    {
        return View();
    }

    public IActionResult EditEvent(int id)
    {
        EventModel eventModel = _eventRepository.GetEventById(id);
        return View(eventModel);
    }
    
    public IActionResult DeleteEvent(int id)
    {
        EventModel eventModel = _eventRepository.GetEventById(id);
        return View(eventModel);
    }
    
    [HttpPost]
    public IActionResult CreateEvent(EventModel eventModel)
    {
        try
        {
            if (ModelState.IsValid)
            {
                TempData["SuccessMessage"] = "Event has been created.";
                _eventRepository.CreateEvent(eventModel);
                return RedirectToAction("Index");
            }

            return View(eventModel);
        }
        catch (Exception error)
        {
            TempData["ErrorMessage"] = $"Ops, could not create an event. Please try again. Error details: {error.Message}";
            return RedirectToAction("Index");
        }
    }
    [HttpPost]
    public IActionResult EditEvent(EventModel eventModel)
    {
        try
        {
            //Lembrar de validar se o event ja foi votado, nao pode editar mais.
            if (ModelState.IsValid)
            {
                _eventRepository.UpdateEvent(eventModel);
                TempData["SuccessMessage"] = "Event has been updated.";
                return RedirectToAction("Index");
            }
            return View (eventModel);

        }
        catch (Exception error)
        {
            TempData["ErrorMessage"] = $"Ops, could not update a event. Please try again. Error details: {error.Message}";
            return RedirectToAction("Index");
        }
    }
    
    [HttpPost]
    public IActionResult DeleteEvent(EventModel eventModel)
    {
        try
        {
            if (ModelState.IsValid)
            {
                bool deleteEvent = _eventRepository.DeleteEvent(eventModel.EventId);
                if (deleteEvent)
                {
                    TempData["SuccessMessage"] = "Event has been deleted.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Event not found.";
                }

                return RedirectToAction("Index");    
            }

            return View(eventModel);

        }
        catch (Exception error)
        {

            TempData["ErrorMessage"] = $"Ops, could not delete an event. Please try again. Error details: {error.Message}";
            return RedirectToAction("Index");
        }
    }
}
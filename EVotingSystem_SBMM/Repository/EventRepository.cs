using EVotingSystem_SBMM.Data;
using EVotingSystem_SBMM.Models;

namespace EVotingSystem_SBMM.Repository;

public class EventRepository : IEventRepository
{
    private readonly EVotingSystemDB _evotingSystem;
    private readonly IVotersRepository _votersRepository;
    public EventRepository(EVotingSystemDB eventRepository, IVotersRepository voterRepository)
    {
        _evotingSystem = eventRepository;
        _votersRepository = voterRepository;
    }
    
    public List<EventModel> GetAll()
    {
        return _evotingSystem.Events.ToList();
    }

    public EventModel GetEventById(int id)
    {
        return _evotingSystem.Events.FirstOrDefault(e => e.EventId == id);
    }

    public EventModel CreateEvent(EventModel eventModel)
    {
        _evotingSystem.Events.Add(eventModel);
        _evotingSystem.SaveChanges();
        return eventModel;
    }

    public EventModel UpdateEvent(EventModel eventModel)
    {
        EventModel events = GetEventById(eventModel.EventId);
        events.Name = eventModel.Name;
        events.City = eventModel.City;
        events.StartDate = eventModel.StartDate;
        events.EndDate = eventModel.EndDate;
        events.EventType = eventModel.EventType;
        _evotingSystem.Events.Update(events);
        _evotingSystem.SaveChanges();
        return events;
    }

    public bool DeleteEvent(int eventModel)
    {
        EventModel events = GetEventById(eventModel);
        if (events == null) throw new Exception("Event does not exist");
        _evotingSystem.Events.Remove(events);
        _evotingSystem.SaveChanges();
        return true;
    }
    
    public EventModel GetActivityEvent()
    {
        // Retrieve the active event based on the current date
        var currentDate = DateTime.Now;
        return _evotingSystem.Events.FirstOrDefault(e => e.StartDate <= currentDate && e.EndDate >= currentDate);
    }
}
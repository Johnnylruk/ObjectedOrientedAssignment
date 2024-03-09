using EVotingSystem_SBMM.Models;

namespace EVotingSystem_SBMM.Repository;

public interface IEventRepository
{
    public List<EventModel> GetAll();
    public EventModel GetEventById(int id);
    public EventModel CreateEvent(EventModel eventModel);
    public EventModel UpdateEvent(EventModel eventModel);
    bool DeleteEvent(int eventModel);
    public EventModel  GetActivityEvent();
}
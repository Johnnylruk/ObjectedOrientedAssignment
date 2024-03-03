using System.ComponentModel.DataAnnotations;
using EVotingSystem_SBMM.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EVotingSystem_SBMM.Models;

public class EventModel
{
    public EventModel()
    {
        Votes = new List<VoteModel>();
        VotePreference = new List<VotePreferenceModel>();
    }
    public int EventId { get; set; }
    [Required(ErrorMessage = "Type event name.")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Type event city.")]
    public string City { get; set; }
    [Required(ErrorMessage = "Select event start date.")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime StartDate { get; set; }
    [Required(ErrorMessage = "Select event end date.")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime EndDate { get; set; }
    [Required(ErrorMessage = "Type a description for this event.")]
    public string Description { get; set; }
    [Required(ErrorMessage = "Select Event type")]
    [Display(Name = "Event Type")]
    public EventTypeEnum EventType { get; set; }
    public ICollection<VoteModel> Votes { get; set; }  = new List<VoteModel>();
    public ICollection<VotePreferenceModel> VotePreference { get; set; } = new List<VotePreferenceModel>();
}
namespace EVotingSystem_SBMM.Models;


public class VoteModel : BaseModel
{

    public DateTime VotedAtTime { get; set; }
    
    // Navigation properties
    public int VoterId { get; set; }
    public virtual VoterModel Voter { get; set; }
    public int CandidateId { get; set; }
    public virtual CandidateModel Candidates { get; set; }
    public int EventId { get; set; }
    public virtual EventModel Event { get; set; }
    public virtual ICollection<VotePreferenceModel> Preferences { get; set; }
}




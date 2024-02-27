namespace EVotingSystem_SBMM.Models;

public class VotePreferenceModel
{
    public int Id { get; set; }
    public int VoteId { get; set; }
    public virtual VoteModel Votes { get; set; }
    public int VoterId { get; set; }

    public virtual VoterModel Voters {get; set; }

    public int CandidateId { get; set; }
    public virtual CandidateModel Candidates { get; set; }
    
    public int Rank { get; set; }

    public int? EventId { get; set; }
    
    public virtual EventModel Event { get; set; }
    
}
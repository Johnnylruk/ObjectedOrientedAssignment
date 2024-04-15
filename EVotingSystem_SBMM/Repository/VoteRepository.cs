using EVotingSystem_SBMM.Data;
using EVotingSystem_SBMM.Enums;
using EVotingSystem_SBMM.Helper;
using EVotingSystem_SBMM.Models;
using Microsoft.EntityFrameworkCore;

namespace EVotingSystem_SBMM.Repository;

public class VoteRepository : IVoteRepository 
{
    private readonly EVotingSystemDB _evotingSystem;
    private readonly IEventRepository _eventRepository;
    
    public VoteRepository(EVotingSystemDB votingSystem, IEventRepository eventRepository)
    {
        _evotingSystem = votingSystem;
        _eventRepository = eventRepository;
    }

    public void SubmitVote(VoteModel vote)
    {
        EventModel eventModel = _eventRepository.GetEventById(vote.EventId);
        vote.VoterId = Cryptography.HashVoterId(vote.VoterId);
        if (eventModel.EventType == EventTypeEnum.FPTP)
        {
            _evotingSystem.Votes.Add(vote);
            _evotingSystem.SaveChanges();    
        }
        if (eventModel.EventType == EventTypeEnum.STV || eventModel.EventType == EventTypeEnum.PV)
        {
            var newVote = new VoteModel
            {
                VoterId = vote.VoterId,
                EventId = eventModel.EventId,
                VotedAtTime = vote.VotedAtTime,
                Preferences = new List<VotePreferenceModel>()
            };
            // Iterate over preferences and add them to the new vote
            foreach (var preference in vote.Preferences)
            {
                var newPreference = new VotePreferenceModel
                {
                    CandidateId = preference.CandidateId,
                    Rank = preference.Rank,
                    EventId = eventModel.EventId,
                    VoterId = vote.VoterId, 
                };
                newVote.Preferences.Add(newPreference);
            }
            // Add the new vote to the context and save changes
            _evotingSystem.Votes.Add(newVote);
            _evotingSystem.SaveChanges();
        }
    }
    public List<VoteModel> GetVotesByEventId(int eventId)
    {
        return _evotingSystem.Votes
            .Where(v => v.EventId == eventId)
            .ToList();
    }
    
    public List<VotePreferenceModel> GetPreferenceVotesByEventId(int? eventId)
    {
        return _evotingSystem.VotePreferences
            .Where(v => v.EventId == eventId)
            .ToList();
    }
    
    public List<VotePreferenceModel> GetSTVVotesByEventId(int voteId)
    {
        return _evotingSystem.VotePreferences
            .Where(v => v.VoteId == voteId)
            .ToList();
    }
    
    public int GetVoteCountForCandidate(int candidateId, int eventId)
    {
        var candidate = _evotingSystem.Candidates
            .Include(c => c.Votes)
            .FirstOrDefault(c => c.Id == candidateId);
        // Filter votes based on both candidateId and eventId
        int voteCount = candidate?.Votes?.Count(v => v.EventId == eventId) ?? 0;
        return voteCount;
    }

    public List<VoteModel> GetAllVotes()
    {
        return _evotingSystem.Votes.ToList();
    }

    public List<VotePreferenceModel> GetAllVotesPreferential()
    {
        return _evotingSystem.VotePreferences.ToList();
    }
    public bool HasVoted(int voterId, int eventId)
    {
        int hashedVoterId = Cryptography.HashVoterId(voterId);
        return _evotingSystem.Votes.Any(v => v.EventId == eventId && v.VoterId == hashedVoterId);
    }
}
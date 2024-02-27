using EVotingSystem_SBMM.Models;

namespace EVotingSystem_SBMM.Repository;

public interface IVoteRepository
{
   void SubmitVote(VoteModel vote);
   int GetVoteCountForCandidate(int candidateId, int eventId);
   List<VoteModel> GetAllVotes();
   bool HasVoted(int voterId, int eventId);
   List<VotePreferenceModel> GetSTVVotesByEventId(int eventId);
   List<VotePreferenceModel> GetAllVotesPreferential();

}

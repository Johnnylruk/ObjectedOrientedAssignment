using EVotingSystem_SBMM.Data;
using EVotingSystem_SBMM.Models;

namespace EVotingSystem_SBMM.Repository;

public class CandidateRepository : ICandidateRepository
{
    
    private readonly EVotingSystemDB _evotingSystem;
   
           public CandidateRepository(EVotingSystemDB evotingSystemDb)
           {
               _evotingSystem = evotingSystemDb;
           }
   
           public CandidateModel Details(int id)
           {
               return _evotingSystem.Candidates.FirstOrDefault(x => x.Id == id) ;
           }
   
           public List<CandidateModel> GetAll()
           {
               
               return _evotingSystem.Candidates.ToList();
           }
           public CandidateModel Register(CandidateModel candidate)
           {
               _evotingSystem.Candidates.Add(candidate);
               _evotingSystem.SaveChanges();
               return candidate;
           }
   
           public CandidateModel GetCandidateById(int id)
           {
               return _evotingSystem.Candidates.FirstOrDefault(x => x.Id == id );
           }
   
           public CandidateModel UpdateCandidate(CandidateModel candidate)
           {
               CandidateModel candidateDb = GetCandidateById(candidate.Id);
               if (candidateDb == null) throw new Exception("Error when trying to update Candidate");
   
               candidateDb.Name = candidate.Name;
               candidateDb.Party = candidate.Party;
               candidateDb.ProfileImage = candidate.ProfileImage;
               candidateDb.Description = candidate.Description;
        
   
               _evotingSystem.Candidates.Update(candidateDb);
               _evotingSystem.SaveChanges();
               return candidateDb;
               
           }
   
           public bool DeleteCandidate(int candidate)
           {
               CandidateModel candidateDb = GetCandidateById(candidate);
               if (candidateDb == null) throw new Exception("Error when trying to delete Candidate");
   
               _evotingSystem.Remove(candidateDb);
               _evotingSystem.SaveChanges();
               return true;
           }
}
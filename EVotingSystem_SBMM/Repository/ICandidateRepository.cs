using EVotingSystem_SBMM.Models;

namespace EVotingSystem_SBMM.Repository;

public interface ICandidateRepository
{
    List<CandidateModel> GetAll();
    CandidateModel Register(CandidateModel candidate);
    CandidateModel GetCandidateById(int id);
    CandidateModel UpdateCandidate(CandidateModel candidate);

    bool DeleteCandidate(int candidateId);
}
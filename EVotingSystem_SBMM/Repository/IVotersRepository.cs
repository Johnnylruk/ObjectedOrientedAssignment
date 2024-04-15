using EVotingSystem_SBMM.Models;

namespace EVotingSystem_SBMM.Repository
{
    public interface IVotersRepository
    {
        List<VoterModel> GetAll();
        VoterModel Details(int voterId);
        VoterModel Register(VoterModel voter);
        VoterModel SubmitRequest(VoterModel voter);
        VoterModel GetVoterById(int id);
        VoterModel UpdateVoter(VoterModel voter);
        public VoterModel ApproveVoterRequest(VoterModel voter);
        public VoterModel DenyVoterRequest(VoterModel voter);
        bool DeleteVoter(int voterId);
        string  GetVoterCity();
    }
}

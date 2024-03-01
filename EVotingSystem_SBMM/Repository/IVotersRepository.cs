using EVotingSystem_SBMM.Models;

namespace EVotingSystem_SBMM.Repository
{
    public interface IVotersRepository
    {
        List<VoterModel> GetAll();
        VoterModel Details(int voterId);
        VoterModel Register(VoterModel voter);
        VoterModel GetVoterbyId(int id);
        VoterModel UpdateVoter(VoterModel voter);
        /*VoterModel GetByLogin(string login);
        VoterModel GetByLoginAndEmail(string login, string email);*/

        bool DeleteVoter(int voterId);
        
        string  GetVoterCity();


    }
}

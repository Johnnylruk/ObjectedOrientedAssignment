using EVotingSystem_SBMM.Models;

namespace EVotingSystem_SBMM.Repository;

public interface ILoginRepository
{
    UserModel GetUserByLogin(string login);
    
    UserModel GetUserByLoginAndEmail(string login, string password);
    VoterModel GetVoterByLogin(string login);
}
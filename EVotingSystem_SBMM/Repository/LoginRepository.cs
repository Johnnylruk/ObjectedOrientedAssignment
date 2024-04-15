using EVotingSystem_SBMM.Data;
using EVotingSystem_SBMM.Models;

namespace EVotingSystem_SBMM.Repository;

public class LoginRepository : ILoginRepository
{
    private readonly EVotingSystemDB _evotingSystem;

    public LoginRepository(EVotingSystemDB evotingSystem)
    {
        _evotingSystem = evotingSystem;
    }
    
    public UserModel GetUserByLogin(string login)
    {
        return _evotingSystem.Users.FirstOrDefault(x => x.Login.ToUpper() == login.ToUpper());
    }

    public UserModel GetUserByLoginAndEmail(string login, string email)
    {
        return _evotingSystem.Users.FirstOrDefault(x => x.Email.ToUpper() == email.ToUpper() && x.Login.ToUpper() == login.ToUpper());
    }
    
    public VoterModel GetVoterByLogin(string login)
    {
        return _evotingSystem.Voters.FirstOrDefault(x => x.Login.ToUpper() == login.ToUpper());
    }

    public VoterModel GetVoterByLoginAndEmail(string login, string email)
    {
        return _evotingSystem.Voters.FirstOrDefault(x => x.Email.ToUpper() == email.ToUpper() && x.Login.ToUpper() == login.ToUpper());
    }
}
using EVotingSystem_SBMM.Models;

namespace EVotingSystem_SBMM.Helper;

public interface IUserSession
{
    void CreateSession(UserModel? userModel, VoterModel? voter);
    void RemoveLoginSession();
    UserModel GetUserSession();
    VoterModel GetVoterSession();
}
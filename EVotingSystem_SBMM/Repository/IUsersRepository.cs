using EVotingSystem_SBMM.Models;

namespace EVotingSystem_SBMM.Repository
{
    public interface IUsersRepository
    {
        List<UserModel> GetAll();
        UserModel Register(UserModel user);
        UserModel GetUserById(int id);
        UserModel UpdateUser(UserModel user);

        bool DeleteUser(int userId);
        
        UserModel GetByLoginAndEmail(string login, string password);
        UserModel ChangePassword (ChangePasswordModel changePasswordModel);
    }
}

using EVotingSystem_SBMM.Data;
using EVotingSystem_SBMM.Helper;
using EVotingSystem_SBMM.Models;

namespace EVotingSystem_SBMM.Repository
{
    public class UsersRepository : IUsersRepository
    {
        private readonly EVotingSystemDB _evotingSystem;

        public UsersRepository(EVotingSystemDB evotingSystemDb)
        {
            _evotingSystem = evotingSystemDb;
        }
        public List<UserModel> GetAll()
        {
            
            return _evotingSystem.Users.ToList();
        }
        public UserModel Register(UserModel user)
        {
            user.RegisterDate = DateTime.Now;
            user.Password = Cryptography.GenerateHash(user.Password);
            _evotingSystem.Users.Add(user);
            _evotingSystem.SaveChanges();
            return user;
        }

        public UserModel GetUserById(int id)
        {
            return _evotingSystem.Users.FirstOrDefault(x => x.Id == id );
        }
        
        public UserModel UpdateUser(UserModel user)
        {
            UserModel userDb = GetUserById(user.Id);
            if (userDb == null) throw new Exception("Error when trying to update user");
        
            userDb.Name = user.Name;
            userDb.Email = user.Email;
            userDb.Login = user.Login;
            userDb.Profile = user.Profile;
            userDb.UpdatedDate = DateTime.Now;

            _evotingSystem.Users.Update(userDb);
            _evotingSystem.SaveChanges();
            return userDb;
            
        }

        public bool DeleteUser(int user)
        {
            UserModel userDb = GetUserById(user);
            if (userDb == null) throw new Exception("Error when trying to delete user");

            _evotingSystem.Remove(userDb);
            _evotingSystem.SaveChanges();
            return true;

        }

        /*public UserModel GetByLogin(string login)
        {
            return _evotingSystem.Users.FirstOrDefault(x => x.Login.ToUpper() == login.ToUpper());
        }*/

        public UserModel GetByLoginAndEmail(string login, string email)
        {
            return _evotingSystem.Users.FirstOrDefault(x => x.Email.ToUpper() == email.ToUpper() && x.Login.ToUpper() == login.ToUpper());
        }

        public UserModel ChangePassword(ChangePasswordModel changePasswordModel)
        {
            UserModel userDB = GetUserById(changePasswordModel.Id);
            //Checking user exist
            if (userDB == null) throw new Exception("Error when trying to update password. User not found.");
            // Checking password
            if (!PasswordHandle.ValidatePassword2(changePasswordModel.OldPassword.GenerateHash(), userDB.Password)) throw new Exception("Error, you have input wrong password.");
            //Checking if new password equals with old password
            if(PasswordHandle.ValidatePassword2(changePasswordModel.NewPassword, userDB.Password)) throw new Exception("Error, new password cannot be equal to old password.");
            
            userDB.Password = PasswordHandle.HashPassword(changePasswordModel.NewPassword);
            userDB.UpdatedDate = DateTime.Now;

            _evotingSystem.Users.Update(userDB);
            _evotingSystem.SaveChanges();

            return userDB;
        }
    }
}

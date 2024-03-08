using EVotingSystem_SBMM.Models;

namespace EVotingSystem_SBMM.Helper;

public class PasswordHandle : IPasswordHandle
{
    public bool ValidatePassword(string hashedPassword, string inputPassword)
    {
        return hashedPassword == inputPassword;
    }
    public bool CheckByInput(string hashedPassword, string inputPassword)
    {
        return  inputPassword == hashedPassword;
    }
    public string GenerateNewPassword()
    {
        return Guid.NewGuid().ToString().Substring(0, 8);
    }

    public string HashPassword(string password)
    {
        return password.GenerateHash();
    }
}
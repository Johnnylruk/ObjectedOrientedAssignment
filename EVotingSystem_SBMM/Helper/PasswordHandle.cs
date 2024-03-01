using EVotingSystem_SBMM.Models;

namespace EVotingSystem_SBMM.Helper;

public class PasswordHandle
{
    public static bool ValidatePassword(string hashedPassword, string inputPassword)
    {
        return hashedPassword == inputPassword;
        
    }
    public static bool CheckByInput(string hashedPassword, string inputPassword)
    {
        return  inputPassword == hashedPassword;
        
    }
    public static string GenerateNewPassword()
    {
        
        return Guid.NewGuid().ToString().Substring(0, 8);
    }

    public static string HashPassword(string password)
    {
        return password.GenerateHash();
    }
}
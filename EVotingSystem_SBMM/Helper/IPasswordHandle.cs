namespace EVotingSystem_SBMM.Helper;

public interface IPasswordHandle
{
    bool ValidatePassword(string hashedPassword, string inputPassword);
    bool CheckByInput(string hashedPassword, string inputPassword);
    string GenerateNewPassword();
    string HashPassword(string password);

}
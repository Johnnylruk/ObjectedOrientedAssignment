namespace EVotingSystem_SBMM.Helper;

public class PassportValidation
{
    public static bool ValidatePassport(string hashedPassport, string inputPassport)
    {
        return hashedPassport == inputPassport;
    }
    public static bool CheckByInput(string hashedPassport, string inputPassport)
    {
        return  inputPassport == hashedPassport;
        
    }
}
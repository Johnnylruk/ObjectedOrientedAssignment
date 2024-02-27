namespace EVotingSystem_SBMM.Helper;

public interface IEmail
{
    bool SendEmailLink(string email, string message, string subject);
}
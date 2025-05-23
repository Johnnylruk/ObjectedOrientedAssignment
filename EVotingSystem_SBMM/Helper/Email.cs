using System.Net;
using System.Net.Mail;

namespace EVotingSystem_SBMM.Helper;

public class Email : IEmail
{
    private readonly IConfiguration _configuration;

    public Email(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public bool SendEmailLink(string email, string subject, string message)
    {
        try
        {
            string host = _configuration.GetValue<string>("SMTP:Host");
            string name = _configuration.GetValue<string>("SMTP:Name");
            string userName = _configuration.GetValue<string>("SMTP:UserName");
            string password = _configuration.GetValue<string>("SMTP:Password");
            int port = _configuration.GetValue<int>("SMTP:Port");

            MailMessage mail = new MailMessage()
            {
                From = new MailAddress(userName, name)
            };
            
            mail.To.Add(email);
            mail.Subject = subject;
            mail.Body = message;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;

            using (SmtpClient smtpClient = new SmtpClient(host, port))
            {
                smtpClient.Credentials = new NetworkCredential(userName, password);
                smtpClient.EnableSsl = true;
                smtpClient.Send(mail);
                return true;
            }
        }
        catch (Exception error)
        {
            return false;
        }
    }
}
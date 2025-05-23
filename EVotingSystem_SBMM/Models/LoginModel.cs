using System.ComponentModel.DataAnnotations;

namespace EVotingSystem_SBMM.Models;

public class LoginModel
{
    public bool IsInvalidCredentials { get; set; }
    public bool ApprovalPending { get; set; }
    [Required(ErrorMessage = "Type your login.")]
    public string Login { get; set;}
    [Required(ErrorMessage = "Type your password.")]
    public string Password { get; set; }
}
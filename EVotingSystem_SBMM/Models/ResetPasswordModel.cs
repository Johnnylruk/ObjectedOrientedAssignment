using System.ComponentModel.DataAnnotations;

namespace EVotingSystem_SBMM.Models;

public class ResetPasswordModel
{
    [Required(ErrorMessage = "Type your login.")]
    public string Login { get; set;}
    [Required(ErrorMessage = "Type your email.")]
    [EmailAddress]
    public string Email { get; set; }
}
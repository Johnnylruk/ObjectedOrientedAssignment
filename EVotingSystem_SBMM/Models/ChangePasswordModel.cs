using System.ComponentModel.DataAnnotations;

namespace EVotingSystem_SBMM.Models;

public class ChangePasswordModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Type your old password.")]
    public string OldPassword { get; set; }
    [Required(ErrorMessage = "Type your new password.")]

    public string NewPassword { get; set; }
    [Required(ErrorMessage = "Confirm your new password.")]
    [Compare("NewPassword", ErrorMessage = "New Password needs to match.")]
    public string ConfirmNewPassword { get; set; }
}
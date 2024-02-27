using EVotingSystem_SBMM.Enums;
using System.ComponentModel.DataAnnotations;
using EVotingSystem_SBMM.Helper;

namespace EVotingSystem_SBMM.Models
{
    public class UserModel
    {

        public int Id { get; set; }
        [Required(ErrorMessage = "Type user name.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Type user login.")]
        public string Login { get; set; }
        [Required(ErrorMessage = "Type your Email.")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Type user password.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Select user profile")]
        [Display(Name = "Profile")]
        public ProfileEnum? Profile { get; set; }
        
        public DateTime RegisterDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        
    }
}

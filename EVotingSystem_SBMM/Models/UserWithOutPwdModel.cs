using EVotingSystem_SBMM.Enums;
using System.ComponentModel.DataAnnotations;

namespace EVotingSystem_SBMM.Models
{
    public class UserWithOutPwdModel : BaseModel
    {

        [Required(ErrorMessage = "Type user name.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Type user login.")]
        public string Login { get; set; }
        [Required(ErrorMessage = "Type your Email.")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Select user profile")]
        [Display(Name = "Profile")]
        public ProfileEnum? Profile { get; set; }
    
    }
}

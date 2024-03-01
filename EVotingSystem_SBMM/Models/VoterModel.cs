    using System.ComponentModel.DataAnnotations;
    using EVotingSystem_SBMM.Enums;

    namespace EVotingSystem_SBMM.Models
    {
        public class VoterModel : BaseModel
        {
            [Required(ErrorMessage = "Type your full name.")]
            public string Name { get; set; }
            [Required(ErrorMessage = "Type your Email.")]
            [EmailAddress]
            public string Email { get; set; }
            [Required(ErrorMessage = "Type your contact number.")]
            [Phone(ErrorMessage = "Contact number it's not valid.")]
            public string Mobile { get; set; }
            [Required(ErrorMessage = "Type your State/County.")]
            public string State { get; set; }
            [Required(ErrorMessage = "Type your city.")]
            public string City { get; set; }
            [Required(ErrorMessage = "Select your date of birth.")]
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
            public DateTime BirthDate { get; set; }
            [Required(ErrorMessage = "Type your Address.")]
            public string Address { get; set; }
            [Required(ErrorMessage = "Type user login.")]
            public string Login { get; set; }
            
            [Required(ErrorMessage = "Type user password.")]
            public string Password { get; set; }
            
            [Required(ErrorMessage = "Type user passport.")]
            public string Passport { get; set; }
            
            public ProfileEnum? Profile { get; set; }
            public ICollection<VoteModel> Votes { get; set; }
            public ICollection<VotePreferenceModel> PreferenceVotes { get; set; }


        }
    }

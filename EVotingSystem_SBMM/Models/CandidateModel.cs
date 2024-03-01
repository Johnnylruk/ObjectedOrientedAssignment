using System.ComponentModel.DataAnnotations;
using EVotingSystem_SBMM.Enums;

namespace EVotingSystem_SBMM.Models;

public class CandidateModel : BaseModel
{
    public CandidateModel()
    {
        Votes = new List<VoteModel>();
    }
    [Required(ErrorMessage = "Type candidate name.")]
    public string Name { get; set; }
    public string? Party { get; set; }
    public string? ProfileImage { get; set; }
    [Required(ErrorMessage = "Type candidate city name.")]
    public string City { get; set; }
    [Required(ErrorMessage = "Type Description for candidate.")]
    public string Description { get; set; }
    public ProfileEnum? Profile { get; set; }
    public ICollection<VoteModel> Votes { get; set; } = new List<VoteModel>();
    public bool IsElected { get; set; }

}
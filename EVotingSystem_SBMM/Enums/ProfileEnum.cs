using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EVotingSystem_SBMM.Enums
{
    public enum ProfileEnum
    {
        [Display(Name ="Electoral Administrator")]
        ElectoralAdministrator = 1,
        Candidate = 2,
        [Display(Name ="Third-Part Auditor")]
        ThirdPartAuditor = 3,
        Voter = 4,
    }
}
     
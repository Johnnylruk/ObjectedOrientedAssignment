using System.ComponentModel.DataAnnotations;

namespace EVotingSystem_SBMM.Enums;

public enum EventTypeEnum
{
    [Display(Name ="Single Transferable Vote")]
    STV = 1,
    [Display(Name ="First-Past-The-Post")]
    FPTP = 2,
    [Display(Name ="Preferential Voting")]
    PV = 3,
}
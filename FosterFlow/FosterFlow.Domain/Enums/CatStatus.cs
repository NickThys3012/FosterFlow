namespace FosterFlow.Domain.Enums;

public enum CatStatus
{
    Available,
    Pending,
    Matched,
    InFoster,
    Returned,

    //both of these are archiving the cat
    Adopted,
    Deceased
}

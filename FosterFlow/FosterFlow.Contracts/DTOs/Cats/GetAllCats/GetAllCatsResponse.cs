using FosterFlow.Domain.Enums;
namespace FosterFlow.Contracts.DTOs.Cats.GetAllCats;

public class GetAllCatsResponse
{
    public List<GetAllCatsDto> Cats { get; set; } = [];
}

public class GetAllCatsDto
{
    public string CatName
    {
        get;
        set;
    }
    public Guid Id { get; set; }
    public int Age { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;
    public Sex Sex { get; set; }
    public CatStatus Status { get; set; }
    public int WeeksNeeded { get; set; }
    public bool IsUrgent { get; set; }
    /// <summary>
    ///     Number of matches temporary 0 functionality isnt there
    /// </summary>
    public int Matches { get; set; }
    /// <summary>
    ///     functionality isnt there
    /// </summary>
    public string FostersName { get; set; }

    /// <summary>
    ///     functionality isnt there
    /// </summary>
    public ExperienceLevel ExperienceLevel { get; set; }
    /// <summary>
    ///     functionality isnt there
    /// </summary>
    public HomeType HomeType { get; set; }
    /// <summary>
    ///     functionality isnt there
    /// </summary>
    public bool HasDogs { get; set; }
}

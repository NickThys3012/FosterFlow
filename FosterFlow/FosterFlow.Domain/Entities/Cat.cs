using FosterFlow.Domain.Enums;
namespace FosterFlow.Domain.Entities;

//TODO: this is boilerplate code for the proj setup and will be changed throughout the development
public class Cat
{
    public Cat() {}
    public Cat(string requestName, DateTime requestBirthDate)
    {
        Name = requestName;
        BirthDate = requestBirthDate;
        Status = CatStatus.Initial;
        Id = Guid.NewGuid();
    }
    public CatStatus Status { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime BirthDate { get; set; }
}

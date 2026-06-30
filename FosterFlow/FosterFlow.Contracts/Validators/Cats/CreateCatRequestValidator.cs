using FluentValidation;
using FosterFlow.Contracts.DTOs.Cats;
namespace FosterFlow.Contracts.Validators.Cats;

public class CreateCatRequestValidator : AbstractValidator<CreateCatRequest>
{
    public CreateCatRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must be 100 characters or fewer.");

        RuleFor(x => x.Age)
            .NotEmpty().WithMessage("Age is required.");
        
        RuleFor(x=>x.Sex)
            .IsInEnum().WithMessage("Sex is required.");
        
        RuleFor(x => x.FosterDuration)
            .NotEmpty().WithMessage("Foster duration is required.");
        
        RuleFor(x => x.PhotoUrl)
            .NotEmpty().WithMessage("Photo is required.");
        
        RuleFor(x => x.TemperamentTags)
            .NotEmpty().WithMessage("At least one temperament tag is required.");
        
        RuleFor(x => x.MedicalNeeds)
            .MaximumLength(500).WithMessage("Medical needs must be 500 characters or fewer.");
        
        RuleFor(x => x.IsUrgent)
            .NotNull().WithMessage("Is urgent is required.");
        
        RuleFor(x => x.DogFriendly)
            .NotNull().WithMessage("Dog friendly home is required.");
    }
}

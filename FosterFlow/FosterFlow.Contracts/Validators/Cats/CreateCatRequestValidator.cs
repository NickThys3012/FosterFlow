using FluentValidation;
using FosterFlow.Contracts.DTOs.Cats.CreateCat;
namespace FosterFlow.Contracts.Validators.Cats;

public class CreateCatRequestValidator : AbstractValidator<CreateCatRequest>
{
    public CreateCatRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must be 100 characters or fewer.");

        RuleFor(x => x.Age)
            .GreaterThanOrEqualTo(0).WithMessage("Age must be greater than or equal to 0 months.");

        RuleFor(x => x.Sex)
            .IsInEnum().WithMessage("Sex is required.");

        RuleFor(x => x.FosterDuration)
            .GreaterThanOrEqualTo(0).WithMessage("Foster duration must be greater than or equal to 0 weeks.");

        RuleFor(x => x.PhotoUrl)
            .NotEmpty().WithMessage("Photo is required.");

        RuleFor(x => x.TemperamentTags)
            .Must(tags => tags is null || tags.Count <= 6)
            .WithMessage("Temperament tags cannot exceed 6.");
        RuleFor(x => x.MedicalNeeds)
            .MaximumLength(500).WithMessage("Medical needs must be 500 characters or fewer.");
    }
}

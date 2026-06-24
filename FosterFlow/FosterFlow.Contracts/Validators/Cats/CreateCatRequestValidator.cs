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

        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage("Birth date is required.")
            .LessThanOrEqualTo(DateTime.Now).WithMessage("Birth date cannot be in the future.");
    }
}

using FluentValidation;
using FosterFlow.Contracts.DTOs.Auth;
namespace FosterFlow.Contracts.Validators.Auth;

public class RegisterFosterRequestValidator : AbstractValidator<RegisterFosterRequest>
{
    public RegisterFosterRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must be 200 characters or fewer.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email is not valid.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%.*?&])[A-Za-z\d@.$!%*?&]+$")
            .WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone is required.");

        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Address is required.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.");

        RuleFor(x => x.PostalCode)
            .NotEmpty().WithMessage("Postal code is required.");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required.");
        
        RuleFor(x => x.ExperienceLevel)
            .IsInEnum().WithMessage("Experience level is required.");
        
        RuleFor(x => x.HomeType)
            .IsInEnum().WithMessage("Home type is required.");
        
        RuleFor(x => x.MaxCats)
            .NotEmpty().WithMessage("Max cats is required.")
            .GreaterThan(0).WithMessage("Max cats must be greater than 0.");
        
        RuleFor(x => x.AvailableFrom)
            .NotEmpty().WithMessage("Available from is required.")
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Now)).WithMessage("Available from must be in the future.");
        
        RuleFor(x => x.AvailableTo)
            .NotEmpty().WithMessage("Available to is required.")
            .GreaterThan(DateOnly.FromDateTime(DateTime.Now)).WithMessage("Available to must be in the future.");
        
    }
}

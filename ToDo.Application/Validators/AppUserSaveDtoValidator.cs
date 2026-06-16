using FluentValidation;
using ToDo.Application.DTOs.User;

namespace ToDo.Application.Validators
{
    public class AppUserSaveDtoValidator : AbstractValidator<AppUserSaveDto>
    {
        public AppUserSaveDtoValidator()
        {
            RuleFor(x => x.name)
                .NotEmpty().WithMessage("User name cannot be empty")
                .MaximumLength(200).WithMessage("User name cannot exceed 200 characters.");

            RuleFor(x => x.password)
                .NotEmpty().WithMessage("Password cannot be empty")
                .MinimumLength(5).WithMessage("The password must be at least 5 characters long")
                .Matches(@"[A-Z]").WithMessage("The password must contain at least one uppercase letter")
                .Matches(@"[a-z]").WithMessage("The password must contain at least one lowercase letter")
                .Matches(@"\d").WithMessage("The password must contain at least one digit");
            
            RuleFor(x => x.email)
                .NotEmpty().WithMessage("Email cannot be empty")
                .EmailAddress().WithMessage("Invalid format for mail");
        
        }
    }
}

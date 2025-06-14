using FluentValidation;
using Shared.DTOs;

namespace UserService.Validators
{
    // FluentValidator за валидация на входните данни за създаване на потребител
    public class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
    {
        public UserCreateDtoValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters long");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters");
        }
    }
}

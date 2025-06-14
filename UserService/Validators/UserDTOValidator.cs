using FluentValidation;
using Shared.DTOs;

namespace UserService.Validators
{
    public class UserDTOValidator : AbstractValidator<UserDTO>
    {
        public UserDTOValidator()
        {
            RuleFor(x => x.UserName).NotEmpty();
            RuleFor(x => x.Password).MinimumLength(6);
        }
    }
}

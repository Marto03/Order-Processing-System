using FluentValidation;
using Shared.DTOs;

namespace OrderService.Validators
{
    // Валидираме данните, които клиентът праща
    public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
    {
        public CreateOrderDtoValidator()
        {
            RuleFor(x => x.CustomerName)
                .NotEmpty()
                .WithMessage("Customer name is required")
                .MinimumLength(3)
                .WithMessage("Customer name must be at least 3 characters");

            RuleFor(x => x.TotalAmount)
                .GreaterThan(0)
                .WithMessage("Total amount must be greater than zero");
        }
    }
}

using FluentValidation;
using InventoryService.Application.DTOs;

namespace InventoryService.Application.Validators;


public class UpdateQuantityRequestValidator : AbstractValidator<UpdateQuantityRequest>
{
    public UpdateQuantityRequestValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Количество должно быть больше нуля.")
            .LessThanOrEqualTo(100000).WithMessage("Количество слишком большое.");
    }
}
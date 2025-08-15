using FluentValidation;
using GroceryInventory.Application.DTOs;

namespace GroceryInventory.Application.Validation;

public class MoveStockRequestValidator : AbstractValidator<MoveStockRequest>
{
    public MoveStockRequestValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity)
            .NotEqual(0).WithMessage("Quantity cannot be zero");
        // For negative adjustments require reason (also enforced in service)
        When(x => x.Quantity < 0, () =>
        {
            RuleFor(x => x.Reason).NotEmpty().WithMessage("Reason is required for negative movements");
        });
    }
}
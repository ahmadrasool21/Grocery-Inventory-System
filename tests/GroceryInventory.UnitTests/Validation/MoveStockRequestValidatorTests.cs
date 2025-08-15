using FluentAssertions;
using FluentValidation.TestHelper;
using GroceryInventory.Application.DTOs;
using GroceryInventory.Application.Validation;

namespace GroceryInventory.UnitTests.Validation;

public class MoveStockRequestValidatorTests
{
    private readonly MoveStockRequestValidator _validator = new();

    [Fact]
    public void Positive_quantity_is_valid()
    {
        var req = new MoveStockRequest(Guid.NewGuid(), 5, 0.7m, null, null, null);
        var result = _validator.TestValidate(req);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Zero_quantity_fails()
    {
        var req = new MoveStockRequest(Guid.NewGuid(), 0, null, null, null, null);
        var result = _validator.TestValidate(req);
        // result.ShouldHaveAnyValidationError();
        Console.WriteLine("result" + result);
         result.ShouldHaveValidationErrorFor();
    }

    [Fact]
    public void Negative_adjustment_without_reason_fails()
    {
        var req = new MoveStockRequest(Guid.NewGuid(), -2, null, null, null, null);
        var result = _validator.TestValidate(req);
        result.ShouldHaveValidationErrorFor(x => x.Reason);
    }
}
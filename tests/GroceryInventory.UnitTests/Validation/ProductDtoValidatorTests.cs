using FluentAssertions;
using FluentValidation.TestHelper;
using GroceryInventory.Application.DTOs;
using GroceryInventory.Application.Validation;

namespace GroceryInventory.UnitTests.Validation;

public class ProductDtoValidatorTests
{
    private readonly ProductDtoValidator _validator = new();

    [Fact]
    public void Valid_product_should_pass()
    {
        var dto = new ProductDto(
            Id: Guid.Empty,
            Name: "Milk 1L",
            Sku: "SKU-MLK-001",
            CategoryId: Guid.NewGuid(),
            Unit: "pcs",
            CostPrice: 0.7m,
            SalePrice: 1.2m,
            TaxRate: 0.10m,
            ReorderLevel: 10,
            IsBatchTracked: true,
            IsActive: true,
            CreatedAt: DateTime.UtcNow
        );

        var result = _validator.TestValidate(dto);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Missing_name_and_sku_should_fail()
    {
        var dto = new ProductDto(Guid.Empty, "", "", Guid.Empty, "", -1, -1, -0.1m, -5, false, true, DateTime.UtcNow);
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.Sku);
        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
        result.ShouldHaveValidationErrorFor(x => x.Unit);
        result.ShouldHaveValidationErrorFor(x => x.CostPrice);
        result.ShouldHaveValidationErrorFor(x => x.SalePrice);
        result.ShouldHaveValidationErrorFor(x => x.TaxRate);
        result.ShouldHaveValidationErrorFor(x => x.ReorderLevel);
    }
}
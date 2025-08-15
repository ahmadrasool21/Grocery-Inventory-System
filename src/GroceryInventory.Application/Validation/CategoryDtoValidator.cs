using FluentValidation;
using GroceryInventory.Application.DTOs;

namespace GroceryInventory.Application.Validation;

public class CategoryDtoValidator : AbstractValidator<CategoryDto>
{
    public CategoryDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
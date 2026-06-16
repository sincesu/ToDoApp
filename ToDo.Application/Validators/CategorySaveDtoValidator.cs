using FluentValidation;
using ToDo.Application.DTOs.Category;

namespace ToDo.Application.Validators
{
    public class CategorySaveDtoValidator : AbstractValidator<CategorySaveDto>
    {
        public CategorySaveDtoValidator()
        {
            RuleFor(x => x.name)
                .NotEmpty().WithMessage("Category name cannot be empty")
                .NotNull().WithMessage("Category name cannot be null")
                .MaximumLength(200).WithMessage("Category name cannot exceed 200 characters.");
        }
    }
}

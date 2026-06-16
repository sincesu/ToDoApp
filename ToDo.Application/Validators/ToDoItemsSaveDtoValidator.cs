using FluentValidation;
using ToDo.Application.DTOs.ToDo;

namespace ToDo.Application.Validators
{
    public class ToDoItemsSaveDtoValidator : AbstractValidator<ToDoItemsSaveDto>
    {
        public ToDoItemsSaveDtoValidator()
        {
            RuleFor(x => x.content)
                .NotEmpty().WithMessage("The content field cannot be empty")
                .MaximumLength(200).WithMessage("The content field can be up to 200 characters long.");

            RuleFor(x => x.priority)
                .NotEmpty().WithMessage("The priority value cannot be empty.")
                .InclusiveBetween(1, 5).WithMessage("The priority value can range from 1 (lowest) to 5 (highest).");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Category ID cannot be empty.")
                .GreaterThan(0).WithMessage("Category ID must be greater than 0.");
        }
    }
}
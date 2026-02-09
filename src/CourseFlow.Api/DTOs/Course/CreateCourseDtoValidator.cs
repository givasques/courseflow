using FluentValidation;

namespace CourseFlow.Api.DTOs.Course;

public class CreateCourseDtoValidator : AbstractValidator<CreateCourseDto>
{
    public CreateCourseDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(150)
            .WithMessage("Title must have at most 150 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Description must have at most 1000 characters.");
        
        RuleFor(x => x.Category)
            .NotEmpty()
            .WithMessage("Category is required.")
            .IsInEnum()
            .WithMessage("Invalid category.");

        RuleFor(x => x.WorkloadHours)
            .NotEmpty()
            .WithMessage("Workload Hours is required.")
            .GreaterThan(0)
            .WithMessage("Workload hours must be greater than zero.")
            .LessThanOrEqualTo(500)
            .WithMessage("Workload hours must be less than or equal to 500.");
    }
}

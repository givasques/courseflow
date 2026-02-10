using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CourseFlow.Api.DTOs.Course;

public sealed class CourseQueryParamsDtoValidator : AbstractValidator<CourseQueryParamsDto>
{
    public CourseQueryParamsDtoValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("PageSize must be between 1 and 100.");

        RuleFor(x => x.Search)
            .MaximumLength(100)
            .WithMessage("Search must have at most 100 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Search));

        RuleFor(x => x.Category)
            .IsInEnum()
            .WithMessage("The Category value is invalid.");
    }
}
using FluentValidation;

namespace CourseFlow.Api;

public class LoginStudentDtoValidator : AbstractValidator<LoginStudentDto>
{
    public LoginStudentDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.")
            .MaximumLength(254);

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(6)
            .WithMessage("Password must have at least 6 characters.");
    }
}

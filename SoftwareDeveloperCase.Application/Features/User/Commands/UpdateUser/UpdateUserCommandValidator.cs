using FluentValidation;

namespace SoftwareDeveloperCase.Application.Features.User.Commands.UpdateUser;

/// <summary>
/// Validator for UpdateUserCommand to ensure business rules are satisfied
/// </summary>
public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    /// <summary>
    /// Initializes a new instance of the UpdateUserCommandValidator class
    /// </summary>
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("{PropertyName} cannot be empty")
            .NotNull().WithMessage("{PropertyName} cannot be null");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("{PropertyName} cannot be empty")
            .NotNull().WithMessage("{PropertyName} cannot be null")
            .EmailAddress().WithMessage("{PropertyName} must be a valid email address");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("{PropertyName} cannot be empty")
            .NotNull().WithMessage("{PropertyName} cannot be null");
    }
}

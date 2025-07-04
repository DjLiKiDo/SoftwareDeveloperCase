using FluentValidation;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Validators;

namespace SoftwareDeveloperCase.Application.Features.Identity.Users.Commands.InsertUser;

/// <summary>
/// Validator for InsertUserCommand to ensure business rules are satisfied
/// </summary>
public class InsertUserCommandValidator : AbstractValidator<InsertUserCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the InsertUserCommandValidator class
    /// </summary>
    /// <param name="unitOfWork">The unit of work instance for data validation</param>
    public InsertUserCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("{PropertyName} cannot be empty")
            .NotNull().WithMessage("{PropertyName} cannot be null");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("{PropertyName} cannot be empty")
            .NotNull().WithMessage("{PropertyName} cannot be null")
            .MustAsync(NotExistingEmail).WithMessage("{PropertyName} already registered."); ;

        RuleFor(x => x.Password)
            .PasswordComplexity();
    }

    private async Task<bool> NotExistingEmail(string? emailAddress, CancellationToken cancellationToken)
    {
        if (emailAddress is null)
            return false;

        var users = await _unitOfWork.UserRepository
            .GetAsync(u => u.Email!.Equals(emailAddress), cancellationToken);

        return !users.Any();
    }

    //private async Task<bool> NotExistingEmail(string emailAddress)
    //{
    //    var users = await _unitOfWork.UserRepository
    //        .GetAsync(u => u.Email!.Equals(emailAddress));

    //    return !users.Any();
    //}
}

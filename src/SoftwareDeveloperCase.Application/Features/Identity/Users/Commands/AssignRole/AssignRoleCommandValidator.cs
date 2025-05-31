using FluentValidation;
using SoftwareDeveloperCase.Application.Contracts.Persistence;

namespace SoftwareDeveloperCase.Application.Features.Identity.Users.Commands.AssignRole;

/// <summary>
/// Validator for AssignRoleCommand to ensure business rules are satisfied
/// </summary>
public class AssignRoleCommandValidator : AbstractValidator<AssignRoleCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the AssignRoleCommandValidator class
    /// </summary>
    /// <param name="unitOfWork">The unit of work instance for data validation</param>
    public AssignRoleCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("{PropertyName} cannot be empty")
            .MustAsync(UserExistsAsync).WithMessage("User with specified {PropertyName} does not exist");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("{PropertyName} cannot be empty")
            .MustAsync(RoleExistsAsync).WithMessage("Role with specified {PropertyName} does not exist");
    }

    private async Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty)
            return false;

        var users = await _unitOfWork.UserRepository
            .GetAsync(u => u.Id == userId, cancellationToken);

        return users.Any();
    }

    private async Task<bool> RoleExistsAsync(Guid roleId, CancellationToken cancellationToken)
    {
        if (roleId == Guid.Empty)
            return false;

        var roles = await _unitOfWork.RoleRepository
            .GetAsync(r => r.Id == roleId, cancellationToken);

        return roles.Any();
    }
}

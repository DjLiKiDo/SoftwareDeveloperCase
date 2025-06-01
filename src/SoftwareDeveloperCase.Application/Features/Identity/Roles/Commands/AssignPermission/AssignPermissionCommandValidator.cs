using FluentValidation;
using SoftwareDeveloperCase.Application.Contracts.Persistence;

namespace SoftwareDeveloperCase.Application.Features.Identity.Roles.Commands.AssignPermission;

/// <summary>
/// Validator for AssignPermissionCommand to ensure business rules are satisfied
/// </summary>
public class AssignPermissionCommandValidator : AbstractValidator<AssignPermissionCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the AssignPermissionCommandValidator class
    /// </summary>
    /// <param name="unitOfWork">The unit of work instance for data validation</param>
    public AssignPermissionCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("{PropertyName} cannot be empty")
            .MustAsync(RoleExistsAsync).WithMessage("Role with specified {PropertyName} does not exist");

        RuleFor(x => x.PermissionId)
            .NotEmpty().WithMessage("{PropertyName} cannot be empty")
            .MustAsync(PermissionExistsAsync).WithMessage("Permission with specified {PropertyName} does not exist");
    }

    private async Task<bool> RoleExistsAsync(Guid roleId, CancellationToken cancellationToken)
    {
        if (roleId == Guid.Empty)
            return false;

        var roles = await _unitOfWork.RoleRepository
            .GetAsync(r => r.Id == roleId, cancellationToken);

        return roles.Any();
    }

    private async Task<bool> PermissionExistsAsync(Guid permissionId, CancellationToken cancellationToken)
    {
        if (permissionId == Guid.Empty)
            return false;

        var permissions = await _unitOfWork.PermissionRepository
            .GetAsync(p => p.Id == permissionId, cancellationToken);

        return permissions.Any();
    }
}

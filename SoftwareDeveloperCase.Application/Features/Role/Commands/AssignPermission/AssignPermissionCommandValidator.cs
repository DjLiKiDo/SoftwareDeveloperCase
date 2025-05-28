using FluentValidation;
using SoftwareDeveloperCase.Application.Contracts.Persistence;

namespace SoftwareDeveloperCase.Application.Features.Role.Commands.AssignPermission
{
    public class AssignPermissionCommandValidator : AbstractValidator<AssignPermissionCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

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
                .GetAsync(r => r.Id == roleId);

            return roles.Any();
        }

        private async Task<bool> PermissionExistsAsync(Guid permissionId, CancellationToken cancellationToken)
        {
            if (permissionId == Guid.Empty)
                return false;

            var permissions = await _unitOfWork.PermissionRepository
                .GetAsync(p => p.Id == permissionId);

            return permissions.Any();
        }
    }
}

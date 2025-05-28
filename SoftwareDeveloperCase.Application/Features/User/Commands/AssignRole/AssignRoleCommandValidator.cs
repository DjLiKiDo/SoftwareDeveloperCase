using FluentValidation;
using SoftwareDeveloperCase.Application.Contracts.Persistence;

namespace SoftwareDeveloperCase.Application.Features.User.Commands.AssignRole
{
    public class AssignRoleCommandValidator : AbstractValidator<AssignRoleCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

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
                .GetAsync(u => u.Id == userId);

            return users.Any();
        }

        private async Task<bool> RoleExistsAsync(Guid roleId, CancellationToken cancellationToken)
        {
            if (roleId == Guid.Empty)
                return false;

            var roles = await _unitOfWork.RoleRepository
                .GetAsync(r => r.Id == roleId);

            return roles.Any();
        }
    }
}

using FluentValidation;
using SoftwareDeveloperCase.Application.Contracts.Persistence;

namespace SoftwareDeveloperCase.Application.Features.Role.Commands.InsertRole
{
    public class InsertRoleCommandValidator : AbstractValidator<InsertRoleCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public InsertRoleCommandValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .NotNull().WithMessage("{PropertyName} cannot be null")
                .MustAsync(NotExistingNameAsync).WithMessage("{PropertyName} already registered.");
        }

        private async Task<bool> NotExistingNameAsync(string? name, CancellationToken cancellationToken)
        {
            if (name is null)
                return false;

            var roles = await _unitOfWork.RoleRepository
                .GetAsync(r => r.Name != null && r.Name.Equals(name));

            return !roles.Any();
        }
    }
}

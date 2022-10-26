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
                .Must(NotExistingName).WithMessage("{PropertyName} already registered.");
        }

        private bool NotExistingName(string? name)
        {
            return name is null ? 
                false : 
                !_unitOfWork.RoleRepository
                .GetAsync(r => r.Name.Equals(name))
                .Result
                .Any();
        }
    }
}

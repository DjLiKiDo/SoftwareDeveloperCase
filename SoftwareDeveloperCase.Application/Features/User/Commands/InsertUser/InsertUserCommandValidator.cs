using FluentValidation;
using SoftwareDeveloperCase.Application.Contracts.Persistence;

namespace SoftwareDeveloperCase.Application.Features.User.Commands.InsertUser
{
    public class InsertUserCommandValidator : AbstractValidator<InsertUserCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public InsertUserCommandValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .NotNull().WithMessage("{PropertyName} cannot be null");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .NotNull().WithMessage("{PropertyName} cannot be null")
                .Must(NotExistingEmail).WithMessage("{PropertyName} already registered."); ;

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .NotNull().WithMessage("{PropertyName} cannot be null");
        }

        private bool NotExistingEmail(string emailAddress)
        {
            return !_unitOfWork.UserRepository
                .GetAsync(u => u.Email.Equals(emailAddress))
                .Result
                .Any();
        }
    }
}

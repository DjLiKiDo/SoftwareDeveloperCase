using FluentValidation;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Validation.Common;
using SoftwareDeveloperCase.Domain.Enums.Core;

namespace SoftwareDeveloperCase.Application.Features.Projects.Commands.CreateProject;

/// <summary>
/// Validator for CreateProjectCommand to ensure business rules are satisfied
/// </summary>
public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the CreateProjectCommandValidator class
    /// </summary>
    /// <param name="unitOfWork">The unit of work instance for data validation</param>
    public CreateProjectCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("{PropertyName} cannot be empty")
            .NotNull().WithMessage("{PropertyName} cannot be null")
            .Length(3, 100).WithMessage("{PropertyName} must be between 3 and 100 characters")
            .MustAsync(async (command, name, cancellationToken) => 
                await NotExistingProjectNameInTeam(command.TeamId, name, cancellationToken))
            .WithMessage("Project name already exists in the specified team");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.TeamId)
            .NotEmpty().WithMessage("{PropertyName} cannot be empty")
            .MustAsync(TeamExistsAsync).WithMessage("Team with specified {PropertyName} does not exist");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("{PropertyName} must be a valid priority value");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("{PropertyName} cannot be empty");

        RuleFor(x => x.EndDate)
            .Must((command, endDate) => !endDate.HasValue || endDate.Value > command.StartDate)
            .WithMessage("End date must be after start date")
            .When(x => x.EndDate.HasValue);
    }

    private async Task<bool> NotExistingProjectNameInTeam(Guid teamId, string? projectName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(projectName) || teamId == Guid.Empty)
            return false;

        return !await _unitOfWork.ProjectRepository
            .IsProjectNameExistsInTeamAsync(teamId, projectName, null, cancellationToken);
    }

    private async Task<bool> TeamExistsAsync(Guid teamId, CancellationToken cancellationToken)
    {
        if (teamId == Guid.Empty)
            return false;

        var teams = await _unitOfWork.TeamRepository
            .GetAsync(t => t.Id == teamId, cancellationToken);

        return teams.Any();
    }
}
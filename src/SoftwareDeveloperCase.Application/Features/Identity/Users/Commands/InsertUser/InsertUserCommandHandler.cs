using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Contracts.Services;
using SoftwareDeveloperCase.Application.Models;
using SoftwareDeveloperCase.Domain.Entities.Identity;
using UserEntity = SoftwareDeveloperCase.Domain.Entities.User;

namespace SoftwareDeveloperCase.Application.Features.Identity.Users.Commands.InsertUser;

/// <summary>
/// Handler for processing insert user commands
/// </summary>
public class InsertUserCommandHandler : IRequestHandler<InsertUserCommand, Result<Guid>>
{
    private readonly ILogger<InsertUserCommandHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly IPasswordService _passwordService;

    /// <summary>
    /// Initializes a new instance of the InsertUserCommandHandler class
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="unitOfWork">The unit of work instance</param>
    /// <param name="emailService">The email service for sending notifications</param>
    /// <param name="passwordService">The password service for hashing passwords</param>
    public InsertUserCommandHandler(ILogger<InsertUserCommandHandler> logger, IMapper mapper, IUnitOfWork unitOfWork, IEmailService emailService, IPasswordService passwordService)
    {
        _logger = logger;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _passwordService = passwordService;
    }

    /// <summary>
    /// Handles the insert user command
    /// </summary>
    /// <param name="request">The insert user command</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>Result containing the identifier of the created user</returns>
    public async Task<Result<Guid>> Handle(InsertUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new user with email: {Email}", request.Email);

        var user = _mapper.Map<UserEntity>(request);

        // Hash the password before saving
        if (!string.IsNullOrWhiteSpace(user.Password))
        {
            user.Password = _passwordService.HashPassword(user.Password);
        }

        _unitOfWork.UserRepository.Insert(user);

        await AssignDefaultRoleAsync(user.Id);

        var result = await _unitOfWork.SaveChanges();

        if (result <= 0)
        {
            _logger.LogError("The user was not inserted");
            return Result<Guid>.Failure("Failed to create user");
        }

        _logger.LogInformation("New user registered with ID: {UserId}", user.Id);

        return Result<Guid>.Success(user.Id);
    }

    private async Task AssignDefaultRoleAsync(Guid userId)
    {
        var defaultRole = await _unitOfWork.RoleRepository
            .GetAsync(r => r.Name != null && r.Name.Equals("Employee"), CancellationToken.None);

        var defaultRoleList = defaultRole.ToList();
        if (!defaultRoleList.Any())
            return;

        var defaultRoleId = defaultRoleList.First().Id;

        if (defaultRole is not null)
        {
            _unitOfWork.UserRoleRepository.Insert(
                new UserRole()
                {
                    RoleId = defaultRoleId,
                    UserId = userId
                });
        }
    }
}

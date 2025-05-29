using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using SoftwareDeveloperCase.Application.Contracts.Services;
using SoftwareDeveloperCase.Application.Models;
using SoftwareDeveloperCase.Domain.Entities;
using System.Linq.Expressions;

namespace SoftwareDeveloperCase.Application.Features.User.Commands.InsertUser;

/// <summary>
/// Handler for processing insert user commands
/// </summary>
public class InsertUserCommandHandler : IRequestHandler<InsertUserCommand, Guid>
{
    private readonly ILogger<InsertUserCommandHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;

    /// <summary>
    /// Initializes a new instance of the InsertUserCommandHandler class
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="unitOfWork">The unit of work instance</param>
    /// <param name="emailService">The email service for sending notifications</param>
    public InsertUserCommandHandler(ILogger<InsertUserCommandHandler> logger, IMapper mapper, IUnitOfWork unitOfWork, IEmailService emailService)
    {
        _logger = logger;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
    }

    /// <summary>
    /// Handles the insert user command
    /// </summary>
    /// <param name="request">The insert user command</param>
    /// <param name="cancellationToken">Cancellation token for the operation</param>
    /// <returns>The identifier of the created user</returns>
    public async Task<Guid> Handle(InsertUserCommand request, CancellationToken cancellationToken)
    {
        var user = _mapper.Map<Domain.Entities.User>(request);

        _unitOfWork.UserRepository.Insert(user);

        await AssignDefaultRoleAsync(user.Id);

        var result = await _unitOfWork.SaveChanges();

        if (result <= 0)
        {
            _logger.LogError("The user was not inserted");
            throw new Exception("The user was not inserted");
        }

        _logger.LogInformation($"New user registered (Id: {user.Id})");

        await SendEmail(user);

        return user.Id;
    }

    private async Task SendEmail(Domain.Entities.User user)
    {
        var departmentManagers = await _unitOfWork.DepartmentRepository.GetManagersAsync(user.DepartmentId);

        var managerEmailList = departmentManagers.Select(dm => dm.Email).ToList();

        var addresses = string.Empty;

        managerEmailList.ForEach(address => addresses += $"{address};");

        var email = new Email
        {
            To = addresses,
            Subject = "A new user has been registered in your department.",
            Body = $"Say hi! to your new colleague {user.Name}"
        };

        try
        {
            await _emailService.SendEmail(email);
        }
        catch (Exception)
        {
            _logger.LogError("Error trying to send new user registration notification.");
        }
    }

    private async Task AssignDefaultRoleAsync(Guid userId)
    {
        var defaultRole = await _unitOfWork.RoleRepository
            .GetAsync(r => r.Name != null && r.Name.Equals("Employee"));

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

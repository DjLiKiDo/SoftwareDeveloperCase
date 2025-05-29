using AutoMapper;
using SoftwareDeveloperCase.Application.Features.Role.Commands.AssignPermission;
using SoftwareDeveloperCase.Application.Features.Role.Commands.InsertRole;
using SoftwareDeveloperCase.Application.Features.User.Commands.AssignRole;
using SoftwareDeveloperCase.Application.Features.User.Commands.InsertUser;
using SoftwareDeveloperCase.Application.Features.User.Commands.UpdateUser;
using SoftwareDeveloperCase.Application.Features.User.Queries.GetUserPermissions;
using SoftwareDeveloperCase.Domain.Entities;

namespace SoftwareDeveloperCase.Application.Mappings;

/// <summary>
/// AutoMapper profile for configuring object mappings between entities and DTOs/commands
/// </summary>
public class MappingProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the MappingProfile class and configures mappings
    /// </summary>
    public MappingProfile()
    {
        CreateMap<Permission, PermissionDto>();
        CreateMap<InsertUserCommand, User>();
        CreateMap<UpdateUserCommand, User>();
        CreateMap<InsertRoleCommand, Role>();
        CreateMap<AssignRoleCommand, UserRole>();
        CreateMap<AssignPermissionCommand, RolePermission>();
    }
}

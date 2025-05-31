using AutoMapper;
using SoftwareDeveloperCase.Application.Features.Identity.Roles.Commands.AssignPermission;
using SoftwareDeveloperCase.Application.Features.Identity.Roles.Commands.InsertRole;
using SoftwareDeveloperCase.Application.Features.Identity.Users.Commands.AssignRole;
using SoftwareDeveloperCase.Application.Features.Identity.Users.Commands.InsertUser;
using SoftwareDeveloperCase.Application.Features.Identity.Users.Commands.UpdateUser;
using SoftwareDeveloperCase.Application.Features.Identity.Users.Queries.GetUserPermissions;
using SoftwareDeveloperCase.Domain.Entities.Core;
using SoftwareDeveloperCase.Domain.Entities.Identity;

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

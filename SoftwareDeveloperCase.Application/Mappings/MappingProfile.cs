using AutoMapper;
using SoftwareDeveloperCase.Application.Features.Role.Commands.AssignPermission;
using SoftwareDeveloperCase.Application.Features.Role.Commands.InsertRole;
using SoftwareDeveloperCase.Application.Features.User.Commands.AssignRole;
using SoftwareDeveloperCase.Application.Features.User.Commands.InsertUser;
using SoftwareDeveloperCase.Application.Features.User.Commands.UpdateUser;
using SoftwareDeveloperCase.Application.Features.User.Queries.GetUserPermissions;
using SoftwareDeveloperCase.Domain.Entities;

namespace SoftwareDeveloperCase.Application.Mappings
{
    public class MappingProfile : Profile
    {
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
}

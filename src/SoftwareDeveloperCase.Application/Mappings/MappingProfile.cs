using AutoMapper;
using SoftwareDeveloperCase.Application.Features.Identity.Roles.Commands.AssignPermission;
using SoftwareDeveloperCase.Application.Features.Identity.Roles.Commands.InsertRole;
using SoftwareDeveloperCase.Application.Features.Identity.Users.Commands.AssignRole;
using SoftwareDeveloperCase.Application.Features.Identity.Users.Commands.InsertUser;
using SoftwareDeveloperCase.Application.Features.Identity.Users.Commands.UpdateUser;
using SoftwareDeveloperCase.Application.Features.Identity.Users.Queries.GetUserPermissions;
using SoftwareDeveloperCase.Application.Features.Projects.DTOs;
using SoftwareDeveloperCase.Domain.Entities;
using SoftwareDeveloperCase.Domain.Entities.Identity;
using SoftwareDeveloperCase.Domain.Entities.Project;
using SoftwareDeveloperCase.Domain.Entities.Team;

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
        
        // Project mappings
        CreateMap<Project, ProjectDto>()
            .ForMember(dest => dest.TeamName, opt => opt.Ignore())
            .ForMember(dest => dest.TeamMemberCount, opt => opt.Ignore())
            .ForMember(dest => dest.TaskCount, opt => opt.Ignore())
            .ForMember(dest => dest.CompletedTaskCount, opt => opt.Ignore())
            .ForMember(dest => dest.OverdueTaskCount, opt => opt.Ignore())
            .ForMember(dest => dest.ProgressPercentage, opt => opt.Ignore())
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.CreatedOn))
            .ForMember(dest => dest.EndDate, opt => opt.Ignore())
            .ForMember(dest => dest.EstimatedCompletionDate, opt => opt.Ignore())
            .ForMember(dest => dest.ActualCompletionDate, opt => opt.Ignore());
    }
}

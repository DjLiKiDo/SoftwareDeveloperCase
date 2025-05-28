using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SoftwareDeveloperCase.Application.Contracts.Persistence;
using System.Linq;

namespace SoftwareDeveloperCase.Application.Features.User.Queries.GetUserPermissions
{
    public class GetUserPermissionsQueryHandler : IRequestHandler<GetUserPermissionsQuery, List<PermissionDto>>
    {
        private readonly ILogger<GetUserPermissionsQueryHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public GetUserPermissionsQueryHandler(ILogger<GetUserPermissionsQueryHandler> logger, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<PermissionDto>> Handle(GetUserPermissionsQuery request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository
                .GetByIdAsync(request.UserId);

            var userRoles = await _unitOfWork.UserRoleRepository
                .GetAsync(ur => ur.UserId.Equals(request.UserId));

            var roleIds = userRoles
                .Select(ur => ur.RoleId);

            var rolePermissions = await _unitOfWork.RolePermissionRepository
                .GetAsync(rp => roleIds.Contains(rp.RoleId));

            var permissionIds = rolePermissions
                .GroupBy(rp => rp.PermissionId)
                .Select(rpg => rpg.Key);

            var userPermissions = await _unitOfWork.PermissionRepository
                .GetAsync(p => permissionIds.Contains(p.Id));

            return _mapper.Map<List<PermissionDto>>(userPermissions);
        }
    }
}

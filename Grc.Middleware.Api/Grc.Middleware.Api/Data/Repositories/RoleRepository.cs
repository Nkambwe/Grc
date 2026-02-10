using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Http.Responses;
using Grc.Middleware.Api.Utils;
using Microsoft.EntityFrameworkCore;

namespace Grc.Middleware.Api.Data.Repositories {
    public class RoleRepository : Repository<SystemRole>, IRoleRepository {
        public RoleRepository(IServiceLoggerFactory loggerFactory,
            GrcContext _context) : base(loggerFactory, _context) {
        }

        public async Task<RoleMinResponse> GetRoleWithPermissionsAsync(IdRequest request) {
            var lookup = await GetLookupAsync(
                    r => r.Id == request.RecordId,
                    r => new {
                        Role = r,
                        RolePermissionSetIds = r.PermissionSets.Select(x => x.PermissionSetId),
                        GroupPermissionSetIds = r.Group.PermissionSets.Select(x => x.PermissionSetId)
                    },
                    request.MarkAsDeleted
                );

            if (lookup == null)
                return null;

            var permissionSetIds = lookup.RolePermissionSetIds.Concat(lookup.GroupPermissionSetIds).Distinct().ToList();
            if (!permissionSetIds.Any())
                return MapRoleOnly(lookup.Role);

            //..get permission sets
            var permissionSets = await context.SystemPermissionSets.Where(ps => permissionSetIds.Contains(ps.Id))
                            .Select(ps => new PermissionSetMinResponse {
                                Id = ps.Id,
                                SetName = ps.SetName,
                                SetDescription = ps.Description
                            }).ToListAsync();

            //..get permissions
            var permissions = await context.SystemPermissionPermissionSets
                 .Where(x => permissionSetIds.Contains(x.PermissionSetId))
                 .Select(x => new PermissionResponse {
                     Id = x.Permission.Id,
                     SetId = x.PermissionSetId,  // This is the permission set ID
                     PermissionName = x.Permission.PermissionName,
                     PermissionDescription = x.Permission.Description
                 })
                 .Distinct()
                 .ToListAsync();

            return new RoleMinResponse {
                Id = lookup.Role.Id,
                RoleName = lookup.Role.RoleName,
                Description = lookup.Role.Description,
                GroupId = lookup.Role.GroupId,
                GroupName = lookup.Role.Group?.GroupName,
                GroupCategory = lookup.Role.Group?.GroupCategory,
                IsActive = !lookup.Role.IsDeleted,
                IsApproved = lookup.Role.IsApproved ?? false,
                IsVerified = lookup.Role.IsVerified ?? false,
                IsDeleted = lookup.Role.IsDeleted,
                CreatedBy = lookup.Role.CreatedBy,
                CreatedOn = lookup.Role.CreatedOn,
                ModifiedBy = lookup.Role.LastModifiedBy,
                ModifiedOn = lookup.Role.LastModifiedOn,
                PermissionSets = permissionSets,
                Permissions = permissions
            };
        }

        private static RoleMinResponse MapRoleOnly(SystemRole role)
            => new() {
                Id = role.Id,
                RoleName = role.RoleName,
                Description = role.Description,
                GroupId = role.GroupId,
                GroupName = role.Group?.GroupName,
                GroupCategory = role.Group?.GroupCategory,
                IsActive = !role.IsDeleted,
                IsApproved = role.IsApproved ?? false,
                IsVerified = role.IsVerified ?? false,
                IsDeleted = role.IsDeleted,
                CreatedBy = role.CreatedBy,
                CreatedOn = role.CreatedOn,
                ModifiedBy = role.LastModifiedBy,
                ModifiedOn = role.LastModifiedOn
            };

    }
}

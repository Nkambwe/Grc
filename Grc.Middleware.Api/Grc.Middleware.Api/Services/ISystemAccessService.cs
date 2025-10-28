using Grc.Middleware.Api.Data.Entities.Logging;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Http.Responses;
using System.Linq.Expressions;

namespace Grc.Middleware.Api.Services {

    public interface ISystemAccessService: IBaseService {

        #region System Users
        bool UserExists(Expression<Func<SystemUser, bool>> predicate, bool excludeDeleted = false);

        Task<bool> UserExistsAsync(Expression<Func<SystemUser, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default);

        Task<SystemUser> GetByIdAsync(long id);

        Task<SystemUser> GetUserByEmailAsync(string email);

        Task<SystemUser> GetUserByUsernameAsync(string username);

        Task<List<SystemUser>> GetAllUsersAsync();

        Task<bool> InsertUserAsync(SystemUser userRecord);

        bool PasswordUpdate(PasswordResetRequest request);

        bool UpdateUser(UserRecordRequest request, bool includeDeleted = false);

        Task<bool> UpdateUserAsync(UserRecordRequest request, bool includeDeleted = false);

        bool DeleteUser(IdRequest request);

        Task<bool> DeleteUserAsync(IdRequest request);

        Task<PagedResult<SystemUser>> PagedUsersAsync(int pageIndex = 1, int pageSize = 5, bool includeDeleted = false);

        Task<PagedResult<SystemUser>> PageAllUsersAsync(CancellationToken token, int pageIndex, int pageSize, Expression<Func<SystemUser, bool>> predicate = null, bool includeDeleted = false);

        #endregion

        #region Admin Dashboard

        Task<AdminCountResponse> GetAdminiDashboardStatisticsAsync();

        Task<int> GetActiveUsersCountAsync();

        Task<int> GetTotalUsersCountAsync();

        #endregion

        #region System Access
        Task<UsernameValidationResponse> ValidateUsernameAsync(string username);

        Task<AuthenticationResponse> AuthenticateUserAsync(string username);

        Task UpdateLastLoginAsync(long userId, DateTime loginTime);

        Task<bool> UpdateLoginStatusAsync(long userId, DateTime loginTime);

        Task<bool> LogFailedLoginAsync(long userId, string ipAddress);

        Task LockUserAccountAsync(long userId);

        Task<WorkspaceResponse> GetWorkspaceAsync(long userId, string ipAddress);

        #endregion

        #region System Roles

        bool RoleExists(Expression<Func<SystemRole, bool>> predicate, bool excludeDeleted = false);

        Task<bool> RoleExistsAsync(Expression<Func<SystemRole, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default);

        Task<string> GetRoleNameAsync(long userId);

        Task<SystemRole> GetRoleByIdAsync(IdRequest id);

        Task<IList<SystemRole>> GetAllRolesAsync(bool includeDeleted = false);

        Task<bool> InsertRoleAsync(RoleRequest request);

        bool UpdateRole(RoleRequest request, bool includeDeleted = false);

        Task<bool> UpdateRoleAsync(RoleRequest request, bool includeDeleted = false);

        bool DeleteRole(IdRequest request);

        Task<bool> DeleteRoleAsync(IdRequest request);

        Task<PagedResult<SystemRole>> PagedRolesAsync(int pageIndex = 1, int pageSize = 10, bool includeDeleted = false);

        Task<PagedResult<SystemRole>> PageAllRolesAsync(CancellationToken token, int page, int size, Expression<Func<SystemRole, bool>> predicate = null, bool includeDeleted = false);

        #endregion

        #region System Role Groups
        bool RoleGroupExists(Expression<Func<SystemRoleGroup, bool>> predicate, bool excludeDeleted = false);

        Task<bool> RoleGroupExistsAsync(Expression<Func<SystemRoleGroup, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default);

        Task<SystemRoleGroup> GetRoleGroupByIdAsync(IdRequest request);

        Task<IList<SystemRoleGroup>> GetAllRoleGroupsAsync(bool includeDeleted = false);

        Task<bool> InsertRoleGroupAsync(RoleGroupRequest request);

        bool UpdateRoleGroup(RoleGroupRequest request, bool includeDeleted = false);

        Task<bool> UpdateRoleGroupAsync(RoleGroupRequest request, bool includeDeleted = false);

        bool DeleteRoleGroup(IdRequest request);

        Task<bool> DeleteRoleGroupAsync(IdRequest request);

        Task<PagedResult<SystemRoleGroup>> PagedRoleGroupsAsync(int pageIndex = 1, int pageSize = 10, bool includeDeleted = false);

        Task<PagedResult<SystemRoleGroup>> PageAllRoleGroupsAsync(CancellationToken token, int page, int size, Expression<Func<SystemRoleGroup, bool>> predicate = null, bool includeDeleted = false);

        #endregion

        #region System Permissions
        Task<List<SystemPermission>> GetAllPermissionsAsync();

        Task<List<SystemPermission>> GetRolePermissionsAsync(RolePermissionRequest request);

        Task<bool> UpdateRolePermissionSetsAsync(long roleId, List<long> newPermissionSetIds);

        Task<List<SystemPermission>> GetPermissionSetPermissionsAsync(long permissionSetId, bool includeDeleted = false);

        Task<bool> UpdatePermissionSetPermissionsAsync(long permissionSetId, List<long> newPermissionIds);

        Task<PagedResult<SystemPermission>> PagedPermissionsAsync(int pageIndex = 1, int pageSize = 10, bool includeDeleted = false);

        Task<PagedResult<SystemPermission>> PageAllPermissionsAsync(CancellationToken token, int page, int size, Expression<Func<SystemPermission, bool>> predicate = null, bool includeDeleted = false);

        #endregion

        #region System Permission Sets
        Task<SystemPermissionSet> GetPermissionSetAsync(IdRequest request);

        Task<bool> PermissionSetExistsAsync(Expression<Func<SystemPermissionSet, bool>> expression);

        Task<List<SystemPermissionSet>> GetRoleGroupPermissionSetsAsync(long roleGroupId, bool includePermissions = false);
        
        Task<bool> UpdateRoleGroupPermissionSetsAsync(long roleGroupId, List<long> newPermissionSetIds);

        Task<bool> InsertPermissionSetAsync(PermissionSetRequest request);

        bool UpdatePermissionSet(PermissionSetRequest request, bool includeDeleted = false);

        Task<bool> UpdatePermissionSetAsync(PermissionSetRequest request, bool includeDeleted = false);

        bool DeletePermissionSet(IdRequest request);

        Task<bool> DeletePermissionSetAsync(IdRequest request);

        Task<List<SystemPermissionSet>> GetAllPermissionSetsAsync();

        Task<PagedResult<SystemPermissionSet>> PagedPermissionSetAsync(int pageIndex = 1, int pageSize = 10, bool includeDeleted = false);

        Task<PagedResult<SystemPermissionSet>> PageAllPermissionSetAsync(CancellationToken token, int page, int size, Expression<Func<SystemPermissionSet, bool>> predicate = null, bool includeDeleted = false);

        #endregion

        #region System Activities

        Task<ActivityLog> GetActivityLogAsync(IdRequest request);

        Task<PagedResult<ActivityLog>> GetPagedActivityLogAsync(int pageIndex = 1, int pageSize = 10, bool includeDeleted = false);

        #endregion

    }
}

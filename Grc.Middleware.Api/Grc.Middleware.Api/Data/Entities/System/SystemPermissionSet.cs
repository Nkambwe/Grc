using Grc.Middleware.Api.Data.Entities.Operations.Processes;

namespace Grc.Middleware.Api.Data.Entities.System
{
    public class SystemPermissionSet: BaseEntity {
        public string SetName { get; set; }
        public string Description { get; set; }
        public ICollection<SystemRolePermissionSet> Roles { get; set; } = new List<SystemRolePermissionSet>();
        public ICollection<SystemPermissionPermissionSet> Permissions { get; set; } = new List<SystemPermissionPermissionSet>();
        public virtual ICollection<SystemRoleGroupPermissionSet> RoleGroups { get; set; } = new List<SystemRoleGroupPermissionSet>();
    }
}

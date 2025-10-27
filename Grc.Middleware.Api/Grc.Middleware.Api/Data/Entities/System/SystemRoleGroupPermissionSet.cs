namespace Grc.Middleware.Api.Data.Entities.System
{
    public class SystemRoleGroupPermissionSet
    {
        public long RoleGroupId { get; set; }
        public long PermissionSetId { get; set; }

        public virtual SystemRoleGroup RoleGroup { get; set; }
        public virtual SystemPermissionSet PermissionSet { get; set; }
    }
}

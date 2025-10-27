namespace Grc.Middleware.Api.Data.Entities.System
{
    public class SystemRolePermissionSet
    {
        public long RoleId { get; set; }
        public long PermissionSetId { get; set; }

        public virtual SystemRole Role { get; set; }
        public virtual SystemPermissionSet PermissionSet { get; set; }
    }
}

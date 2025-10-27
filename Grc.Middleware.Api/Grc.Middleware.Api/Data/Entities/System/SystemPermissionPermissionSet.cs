using Grc.Middleware.Api.Data.Entities.System;

namespace Grc.Middleware.Api.Data.Entities.System
{
    public class SystemPermissionPermissionSet
    {
        public long PermissionId { get; set; }
        public long PermissionSetId { get; set; }

        public virtual SystemPermission Permission { get; set; }
        public virtual SystemPermissionSet PermissionSet { get; set; }
    }
}

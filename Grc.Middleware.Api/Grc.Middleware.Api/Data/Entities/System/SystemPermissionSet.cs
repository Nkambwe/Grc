using Grc.Middleware.Api.Data.Entities.Operations.Processes;

namespace Grc.Middleware.Api.Data.Entities.System
{
    public class SystemPermissionSet: BaseEntity {
        public string SetName { get; set; }
        public string Description { get; set; }

        public ICollection<SystemPermissionPermissionSet> Permissions { get; set; } = new List<SystemPermissionPermissionSet>();
    }
}

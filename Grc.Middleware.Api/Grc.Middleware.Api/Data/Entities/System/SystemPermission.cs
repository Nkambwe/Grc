
namespace Grc.Middleware.Api.Data.Entities.System
{
    public class SystemPermission: BaseEntity {
        public string PermissionName { get; set; }
        public string Description { get; set; }

        public ICollection<SystemPermissionPermissionSet> PermissionSets { get; set; } = new List<SystemPermissionPermissionSet>();
    }
}

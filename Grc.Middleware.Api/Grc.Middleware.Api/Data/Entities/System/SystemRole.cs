
namespace Grc.Middleware.Api.Data.Entities.System {

    public class SystemRole : BaseEntity {
        public long GroupId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsVerified { get; set; }
        public virtual SystemRoleGroup Group { get; set; }
        public virtual ICollection<SystemUser> Users { get; set; } = new List<SystemUser>();
        public virtual ICollection<SystemRolePermissionSet> PermissionSets { get; set; } = new List<SystemRolePermissionSet>();
        public override bool Equals(object obj) {

            if (obj is not SystemRole)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            var item = (SystemRole)obj;

            if (item.IsNew() || IsNew())
                return false;

            return item.RoleName.Equals(RoleName);
        }

        public override string ToString()
            => $"{RoleName} ({Description})";

        public override int GetHashCode() => ToString().GetHashCode() ^ 31;
    }

}

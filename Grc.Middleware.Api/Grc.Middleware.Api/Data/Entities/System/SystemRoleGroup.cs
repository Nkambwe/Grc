using Grc.Middleware.Api.Enums;

namespace Grc.Middleware.Api.Data.Entities.System {
    public class SystemRoleGroup : BaseEntity {
        public string GroupName { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// Get or Set Scope of group
        /// </summary>
        public GroupScope Scope { get; set; }
        /// <summary>
        /// Get or Set Scope of group
        /// </summary>
        public string Department { get; set; }
        /// <summary>
        /// Get or Set Group type
        /// </summary>
        public RoleGroup Type { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsVerified { get; set; }

        public virtual ICollection<SystemRole> Roles { get; set; }
        public override bool Equals(object obj) {

            if (obj is not SystemRoleGroup)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            var item = (SystemRoleGroup)obj;

            if (item.IsNew() || IsNew())
                return false;

            return item.GroupName.Equals(GroupName);
        }

        public override string ToString()
            => $"{GroupName} ({Description})";

        public override int GetHashCode() => ToString().GetHashCode() ^ 31;
    }

}

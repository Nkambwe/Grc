using Grc.Middleware.Api.Data.Entities.Compliance;
using Grc.Middleware.Api.Data.Entities.System;

namespace Grc.Middleware.Api.Data.Entities.Org {

    public class Department : BaseEntity {
        public long BranchId { get; set; }
        public string DepartmenCode { get; set; }
        public string DepartmentName { get; set; }
        public string Alias { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual ICollection<SystemUser> Users {get; set;}
        public virtual ICollection<DepartmentUnit> Units { get; set; }
        public virtual ICollection<Responsibility> Responsibilities { get; set; }
        public override bool Equals(object obj) {

            if (obj is not Department)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            var item = (Department)obj;

            if (item.IsNew() || IsNew())
                return false;

            return item.DepartmenCode.Equals(DepartmenCode) && item.DepartmentName.Equals(DepartmentName);
        }

        public override string ToString() 
            => $"{DepartmenCode}-{DepartmentName}";

        public override int GetHashCode() 
            => ToString().GetHashCode() ^ 31;
    }

}

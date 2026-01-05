using Grc.Middleware.Api.Data.Entities.Compliance.Returns;
using Grc.Middleware.Api.Data.Entities.Support;
using Grc.Middleware.Api.Data.Entities.System;

namespace Grc.Middleware.Api.Data.Entities.Org {

    public class Department : BaseEntity {
        public long BranchId { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string Alias { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual ICollection<SystemUser> Users {get; set;}
        public virtual ICollection<DepartmentUnit> Units { get; set; }
        public virtual ICollection<Responsebility> Responsibilities { get; set; }
        public virtual ICollection<ReturnReport> Returns { get; set; }
        public virtual ICollection<Circular> Circulars { get; set; }
        public override bool Equals(object obj) {

            if (obj is not Department)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            var item = (Department)obj;

            if (item.IsNew() || IsNew())
                return false;

            return item.DepartmentCode.Equals(DepartmentCode) && item.DepartmentName.Equals(DepartmentName);
        }

        public override string ToString() 
            => $"{DepartmentCode}-{DepartmentName}";

        public override int GetHashCode() 
            => ToString().GetHashCode() ^ 31;
    }

}

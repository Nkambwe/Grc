
using Grc.Middleware.Api.Data.Entities.Operations.Processes;

namespace Grc.Middleware.Api.Data.Entities.Org {

    public class DepartmentUnit : BaseEntity {
        public long DepartmentId { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public virtual Department Department { get; set; }

        public virtual ICollection<OperationProcess> Processes { get; set; }
        public override bool Equals(object obj) {

            if (obj is not DepartmentUnit)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            var item = (DepartmentUnit)obj;

            if (item.IsNew() || IsNew())
                return false;

            return item.UnitCode.Equals(UnitCode) && item.UnitName.Equals(UnitName);
        }

        public override string ToString() => $"{UnitCode}-{UnitName}";

        public override int GetHashCode() => ToString().GetHashCode() ^ 31;
    }
}

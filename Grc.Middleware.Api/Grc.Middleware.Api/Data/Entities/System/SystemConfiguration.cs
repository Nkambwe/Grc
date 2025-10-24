using Grc.Middleware.Api.Data.Entities.Org;

namespace Grc.Middleware.Api.Data.Entities.System
{
    public class SystemConfiguration: BaseEntity {
        public long CompanyId { get; set; }
        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }
        public string ParameterType { get; set; }

        public virtual Company Company { get; set; }
        public override bool Equals(object obj)
        {

            if (obj is not SystemConfiguration)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            var item = (SystemConfiguration)obj;

            if (item.IsNew() || IsNew())
                return false;

            return item.Id.Equals(Id) && item.ParameterName.Equals(ParameterName);
        }

        public override int GetHashCode() => ToString().GetHashCode() ^ 31;
    }
}

using Grc.Middleware.Api.Data.Entities.Logging;
using Grc.Middleware.Api.Data.Entities.System;

namespace Grc.Middleware.Api.Data.Entities.Org {

    public class Company : BaseEntity {
        public string CompanyName { get; set; }
        public string ShortName { get; set; }
        public string RegistrationNumber { get; set; }
        public string SystemLanguage { get; set; }
        public override string ToString() => $"{CompanyName}";

        public virtual ICollection<Branch> Branches { get; set; }
        public virtual ICollection<SystemError> SystemErrors { get; set; }
        public virtual ICollection<ActivityLogSetting> ActivitySettings { get; set; }
        public virtual ICollection<SystemConfiguration> SystemConfigurations { get; set; }
        public override bool Equals(object obj) {

            if (obj is not Company)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            var item = (Company)obj;

            if (item.IsNew() || IsNew())
                return false;

            return item.CompanyName.Equals(CompanyName) && item.Id.Equals(Id);
        }

        public override int GetHashCode() => ToString().GetHashCode() ^ 31;
    }

}

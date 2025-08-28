
using Grc.Middleware.Api.Data.Entities.System;

namespace Grc.Middleware.Api.Data.Entities.Logging {

    public class ActivityLog : BaseEntity {
        public long? EntityId { get; set; }
        public string EntityName { get; set; }
        public string IpAddress { get; set; }
        public string Comment { get; set; }
        public long UserId { get; set; }
        public virtual ActivityType ActivityType { get; set; }
        public virtual SystemUser User { get; set; }

        public override bool Equals(object obj) {

            if (obj is not ActivityLog)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            var item = (ActivityLog)obj;

            if (item.IsNew() || IsNew())
                return false;

            return item.Id.Equals(Id) && item.EntityName.Equals(EntityName);
        }
        public override string ToString() => $"{Id}-{EntityName}";
        public override int GetHashCode() => ToString().GetHashCode() ^ 31;
    }
}

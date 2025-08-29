using Grc.Middleware.Api.Data.Entities.Org;

namespace Grc.Middleware.Api.Data.Entities.Logging {

    public class ActivityLogSetting: BaseEntity {
        public long CompanyId { get; set; }
        public string ParameterName { get; set; } 
        public string ParameterValue { get; set; } 
        public string Description { get; set; } 
        public virtual Company Company { get; set; }
        public override string ToString() => $"{Id}-{Id}";
        public override int GetHashCode() => ToString().GetHashCode() ^ 31;
    }
}

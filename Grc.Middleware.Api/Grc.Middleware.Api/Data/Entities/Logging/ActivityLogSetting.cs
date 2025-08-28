using Grc.Middleware.Api.Data.Entities.Org;

namespace Grc.Middleware.Api.Data.Entities.Logging {

    public class ActivityLogSetting: BaseEntity {
        public long CompanyId { get; set; }
        public bool EnableLogging { get; set; } = true;
        public bool LogSupportActivities { get; set; } = true;
        public bool LogUserActivities { get; set; } = true;
        public bool AutoDeleteActivities { get; set; } = true;
        /// <summary>
        /// Auto-delete after 90 days
        /// </summary>
        public int AutoDeleteDays { get; set; } = 90;
        public bool LogIpAddress { get; set; } = true;
        public List<string> DisabledActivityTypes { get; set; } = new();

        public virtual Company Company { get; set; }
        public override string ToString() => $"{Id}-{Id}";
        public override int GetHashCode() => ToString().GetHashCode() ^ 31;
    }
}

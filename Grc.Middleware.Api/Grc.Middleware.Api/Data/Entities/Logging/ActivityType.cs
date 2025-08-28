
namespace Grc.Middleware.Api.Data.Entities.Logging {
    public class ActivityType: BaseEntity {
        public string Name { get; set; }
        public string SystemKeyword { get; set; }
        public string Description { get; set; }      
        public bool Enabled { get; set; } = true;
        /// <summary>
        /// User Activity, Role Activity, etc
        /// </summary>
        public string Category { get; set; }
        public bool IsSupportActivity { get; set; }
        public virtual ICollection<ActivityLog> ActivityLogs { get; set; }
        public override bool Equals(object obj) {

            if (obj is not ActivityType)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            var item = (ActivityType)obj;

            if (item.IsNew() || IsNew())
                return false;

            return item.Name.Equals(Name) && item.Category.Equals(Category);
        }
        public override string ToString() => $"{Category}-{Name}";
        public override int GetHashCode() => ToString().GetHashCode() ^ 31;
    }
}

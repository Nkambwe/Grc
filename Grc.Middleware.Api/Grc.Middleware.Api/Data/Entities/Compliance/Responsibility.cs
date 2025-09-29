using Grc.Middleware.Api.Data.Entities.Org;

namespace Grc.Middleware.Api.Data.Entities.Compliance {
    public class Responsibility : BaseEntity {
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPosition { get; set; }
        public string Description { get; set; }
        public long DepartmentId { get; set; } // IT, HR, Finance, Legal, Operations, BOD
        public virtual ICollection<RegulatoryReturn> Returns { get; set; }
        public virtual Department Department { get; set; }
    }

}

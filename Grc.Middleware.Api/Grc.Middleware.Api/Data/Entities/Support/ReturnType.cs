using Grc.Middleware.Api.Data.Entities.Compliance.Returns;

namespace Grc.Middleware.Api.Data.Entities.Support {
    public class ReturnType : BaseEntity
    {
        public string TypeName { get; set; } // Report, Declaration, Other
        public virtual ICollection<RegulatoryReturn> Returns { get; set; }
    }

}

using Grc.Middleware.Api.Data.Entities.Org;

namespace Grc.Middleware.Api.Data.Entities.System {
    public class MailSettings: BaseEntity {
        public string SystemEmail { get; set; }
        public string SystemPassword { get; set; }
        public int NetworkPort { get; set; }
        public string Sender { get; set; }
        public long CompanyId { get; set; }
        public virtual Company Company { get; set; }
        
    }
}

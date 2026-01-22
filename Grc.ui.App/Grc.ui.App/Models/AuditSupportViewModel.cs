
namespace Grc.ui.App.Models {

    public class AuditSupportViewModel {
        public List<AuthorityViewModel> Authorities { get; set; } = new();
        public List<AuditTypeMiniViewModel> Types { get; set; } = new();
        public List<AuditMiniViewModel> Audits { get; set; } = new();
        public List<ResponsibilityViewModel> Responsibilities { get; set; } = new();

    }

}

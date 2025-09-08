using Grc.ui.App.Helpers;

namespace Grc.ui.App.Models {
    public class PageComponentViewModel {
        public string ComponentId { get; set; }
        public string ComponentKey { get; set; }
        public List<ComponentAction> LeftActions { get; set; } = new();
    }
}

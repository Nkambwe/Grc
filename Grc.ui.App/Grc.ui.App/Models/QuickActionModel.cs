namespace Grc.ui.App.Models {

    public class QuickActionModel {
        public string Label { get; set; }
        public string IconClass { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public string Area { get; set; }
        public string CssClass { get; set; } = "is-primary";

        public bool IsModal { get; set; } = false;
        public string ComponentName { get; set; } 
        public string ComponentData { get; set; } 
    }

}

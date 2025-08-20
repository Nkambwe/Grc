namespace Grc.ui.App.Menus {
    public class MenuItem {
        public string Key => $"{Area}.{Controller}.{Action}";
        public string Label { get; set; }
        public string IconClass { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Area { get; set; }
        public string CssClass { get; set; } = "";

        public List<MenuItem> Children { get; set; } = new();
    }
}

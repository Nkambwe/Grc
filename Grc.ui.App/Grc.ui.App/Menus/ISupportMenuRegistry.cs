namespace Grc.ui.App.Menus {
    public interface ISupportMenuRegistry {
        IReadOnlyList<MenuItem> GetAll();
        MenuItem Find(string area, string controller, string action);
    }
}

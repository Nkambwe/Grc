namespace Grc.ui.App.Menus {
    public interface IOperationsMenuRegistry {
        IReadOnlyList<MenuItem> GetAll();
        MenuItem Find(string area, string controller, string action);
    }
}

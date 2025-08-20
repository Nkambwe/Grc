using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Menus;
using Grc.ui.App.Models;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Grc.ui.App.Filters {
    /// <summary>
    /// Filter to help add recent item to session
    /// </summary>
    public class RecentMenuItemAttribute : IActionFilter {
        private readonly SessionManager _session;
        private readonly ISupportMenuRegistry _menuRegistry;

        public RecentMenuItemAttribute(SessionManager session, ISupportMenuRegistry menuRegistry) {
            _session = session;
            _menuRegistry = menuRegistry;
        }

        public void OnActionExecuting(ActionExecutingContext context) {
            var controller = context.RouteData.Values["controller"]?.ToString();
            var action = context.RouteData.Values["action"]?.ToString();
            var area = context.RouteData.Values.ContainsKey("area")
                ? context.RouteData.Values["area"]?.ToString()
                : "";

             var registry = _menuRegistry.Find(area, controller, action);
            if (registry == null) 
                return; 

            //..create recent item
            var recent = new RecentModel {
                Label = registry.Label, 
                IconClass = registry.IconClass, 
                Controller = controller,
                Action = action,
                Area = area,
                CssClass = registry.CssClass,
            };

            //..get existing list
            var items = _session.Get<List<RecentModel>>(SessionKeys.RecentItems.GetDescription()) ?? new List<RecentModel>();

            //..prevent duplicates at the top
            items.RemoveAll(x => x.Controller == controller && x.Action == action && x.Area == area);
            items.Insert(0, recent);

            //..keep last 10
            items = items.Take(10).ToList();

            _session.Save(SessionKeys.RecentItems.GetDescription(), items);
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}

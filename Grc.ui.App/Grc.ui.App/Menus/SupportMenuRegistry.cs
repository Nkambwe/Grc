
namespace Grc.ui.App.Menus {

    public class SupportMenuRegistry : ISupportMenuRegistry {

        private readonly List<MenuItem> _root;
        public SupportMenuRegistry() {
            _root = new List<MenuItem> {
                new() {
                    Label = "App.Menu.Home",
                    IconClass = "mdi mdi-home-outline",
                    Controller = "Support",
                    Action = "Index",
                    Area = "Admin"
                },
                new() {
                    Label = "App.Menu.Departments",
                    IconClass = "mdi mdi-share-all-outline",
                    Controller = "Support",
                    Action = "Departments",
                    Area = "Admin"
                },
                new() {
                    Label = "App.Menu.Users",
                    IconClass = "mdi mdi-account-outline",
                    Controller = "Support",
                    Action = "Users",
                    Area = "Admin"
                },
                new() {
                    Label = "App.Menu.Configurations.Roles",
                    IconClass = "mdi mdi-account-box-outline",
                    Controller = "Support",
                    Action = "Roles",
                    Area = "Admin"
                },
                new(){
                    Label = "App.Menu.Permissions",
                    IconClass = "mdi mdi-server-security",
                    Children = new List<MenuItem> {
                         new() {
                            Label = "App.Menu.Permissions.Assign",
                            IconClass = "mdi mdi-shield-check-outline",
                            Controller = "Support",
                            Action = "AssignPermissions",
                            Area = "Admin"
                        },
                         new() {
                            Label = "App.Menu.Permissions.Sets",
                            IconClass = "mdi mdi-shield-account-outline",
                            Controller = "Support",
                            Action = "PermissionSets",
                            Area = "Admin"
                        },
                         new() {
                            Label = "App.Menu.Permissions.Delegation",
                            IconClass = "mdi mdi-account-convert-outline",
                            Controller = "Support",
                            Action = "PermissionDelegation",
                            Area = "Admin"
                        }
                    }
                },
                new() {
                    Label = "App.Menu.Configurations.Settings",
                    IconClass = "mdi mdi-account-cog-outline",
                    Children = new List<MenuItem> {
                         new() {
                            Label = "App.Menu.Configurations.IP",
                            IconClass = "mdi mdi-ip-network-outline",
                            Controller = "Configuration",
                            Action = "IPManagement",
                            Area = "Admin"
                         },
                         new() {
                            Label = "App.Menu.Configurations.Data",
                            IconClass = "mdi mdi-account-details-outline",
                            Controller = "Configuration",
                            Action = "UserData",
                            Area = "Admin"
                         },
                         new() {
                            Label = "App.Menu.Configurations.Groups",
                            IconClass = "mdi mdi-account-group-outline",
                            Controller = "Configuration",
                            Action = "UserGroups",
                            Area = "Admin"
                         },
                         new() {
                            Label = "App.Menu.Configurations.Authentication",
                            IconClass = "mdi mdi-account-question-outline",
                            Controller = "Configuration",
                            Action = "UserAuthentication",
                            Area = "Admin"
                         }
                    }
                },
                new() {
                    Label = "App.Menu.Configurations.Encryption",
                    IconClass = "mdi mdi-account-box-outline",
                    Controller = "Configuration",
                    Action = "DataEncryptions",
                    Area = "Admin"
                },
                new() {
                    Label = "App.Menu.Configurations.Errors",
                    IconClass = "mdi mdi-bug-outline",
                    Controller = "Configuration",
                    Action = "BugReporter",
                    Area = "Admin"
                },
                new() {
                    Label = "App.Menu.Configurations.Activities",
                    IconClass = "mdi mdi-bell-ring-outline",
                    Controller = "Configuration",
                    Action = "SystemActivity",
                    Area = "Admin"
                },
            };
        }

        /// <summary>
        /// Get all support menu items
        /// </summary>
        /// <returns>List of all menu items for support</returns>
        public IReadOnlyList<MenuItem> GetAll() => _root;

        /// <summary>
        /// Find a menu item beloging to an area, controller and action
        /// </summary>
        /// <param name="area"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public MenuItem Find(string area, string controller, string action) {
            return Flatten(_root).FirstOrDefault(m =>
                string.Equals(m.Area, area, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(m.Controller, controller, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(m.Action, action, StringComparison.OrdinalIgnoreCase));
        }

        private static IEnumerable<MenuItem> Flatten(IEnumerable<MenuItem> items) {
            foreach (var item in items) {
                yield return item;
                foreach (var child in Flatten(item.Children))
                    yield return child;
            }
        }
    }
}

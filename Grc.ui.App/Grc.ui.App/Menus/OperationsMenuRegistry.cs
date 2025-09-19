namespace Grc.ui.App.Menus {
    public class OperationsMenuRegistry : IOperationsMenuRegistry {

        private readonly List<MenuItem> _root;
        public OperationsMenuRegistry() {
            _root = new List<MenuItem> {
                new() {
                    Label = "Operations.Menu.Home",
                    IconClass = "mdi mdi-home-outline",
                    Controller = "OperationDashboard",
                    Action = "Index",
                    Area = "Operations"
                },
                new(){
                    Label = "Operations.Menu.Processes",
                    IconClass = "mdi mdi-server-security",
                    Children = new List<MenuItem> {
                         new() {
                            Label = "Operations.Menu.Processes.Register",
                            IconClass = "mdi mdi-shield-check-outline",
                            Controller = "OperationWorkflow",
                            Action = "RegisterProcess",
                            Area = "Operations"
                         },
                         new() {
                            Label = "Operations.Menu.Processes.Groups",
                            IconClass = "mdi mdi-shield-account-outline",
                            Controller = "OperationWorkflow",
                            Action = "GroupProcesses",
                            Area = "Operations"
                        },
                         new() {
                            Label = "Operations.Menu.Processes.Tags",
                            IconClass = "mdi mdi-account-convert-outline",
                            Controller = "OperationWorkflow",
                            Action = "TagProcesses",
                            Area = "Operations"
                        }
                    }
                },
                new(){
                    Label = "Operations.Menu.Tasks",
                    IconClass = "mdi mdi-server-security",
                    Children = new List<MenuItem> {
                         new() {
                            Label = "Operations.Menu.Tasks.Approvals",
                            IconClass = "mdi mdi-shield-check-outline",
                            Controller = "OperationWorkflow",
                            Action = "Approvals",
                            Area = "Operations"
                        },
                         new() {
                            Label = "Operations.Menu.Tasks.Pending",
                            IconClass = "mdi mdi-shield-account-outline",
                            Controller = "OperationWorkflow",
                            Action = "Pending",
                            Area = "Operations"
                        },
                         new() {
                            Label = "Operations.Menu.Tasks.Revisions",
                            IconClass = "mdi mdi-account-convert-outline",
                            Controller = "OperationWorkflow",
                            Action = "Revisions",
                            Area = "Operations"
                        }
                    }
                },
                new() {
                    Label = "Operations.Menu.Configurations.Access",
                    IconClass = "mdi mdi-account-cog-outline",
                    Children = new List<MenuItem> {
                         new() {
                            Label = "Operations.Menu.Configurations.Teams",
                            IconClass = "mdi mdi-account-details-outline",
                            Controller = "OperationsConfiguration",
                            Action = "Teams",
                            Area = "Operations"
                         },
                         new() {
                            Label = "Operations.Menu.Configurations.Guests",
                            IconClass = "mdi mdi-account-details-outline",
                            Controller = "OperationsConfiguration",
                            Action = "Guests",
                            Area = "Operations"
                         },
                         new() {
                            Label = "Operations.Menu.Configurations.Delegation",
                            IconClass = "mdi mdi-account-group-outline",
                            Controller = "OperationsConfiguration",
                            Action = "Delegation",
                            Area = "Operations"
                         }
                    }
                },
                new() {
                    Label = "Operations.Menu.Configurations.Settings",
                    IconClass = "mdi mdi-account-cog-outline",
                    Children = new List<MenuItem> {
                         new() {
                            Label = "Operations.Menu.Configurations.General",
                            IconClass = "mdi mdi-account-details-outline",
                            Controller = "OperationsConfiguration",
                            Action = "General",
                            Area = "Operations"
                         },
                         new() {
                            Label = "Operations.Menu.Configurations.Additional",
                            IconClass = "mdi mdi-account-details-outline",
                            Controller = "OperationsConfiguration",
                            Action = "Additional",
                            Area = "Operations"
                         }
                    }
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

        private static IEnumerable<MenuItem> Flatten(IEnumerable<MenuItem> items)
        {
            foreach (var item in items)
            {
                yield return item;
                foreach (var child in Flatten(item.Children))
                    yield return child;
            }
        }
    }
}

namespace Grc.ui.App.Routes {
    public class RouteProvider : IRouteProvider {
        public int Priority => 0;

        public void RegisterRoutes(IEndpointRouteBuilder routeBuilder) {
            
            routeBuilder.MapControllerRoute(
                name: "admin-departments",
                pattern: "admin/support/departments",
                defaults: new { area = "Admin", controller = "Support", action = "Departments" }
            );

            routeBuilder.MapControllerRoute(
                name: "admin-users",
                pattern: "admin/support/system-users",
                defaults: new { area = "Admin", controller = "Support", action = "Users" }
            );

            routeBuilder.MapControllerRoute(
                name: "admin-roles",
                pattern: "admin/support/system-roles",
                defaults: new { area = "Admin", controller = "Support", action = "Roles" }
            );

            routeBuilder.MapControllerRoute(
               name: "admin-rolesGroups",
               pattern: "admin/support/system-roles-groups",
               defaults: new { area = "Admin", controller = "Support", action = "RoleGroups" }
            );

            routeBuilder.MapControllerRoute(
               name: "admin-rolesDelegations",
               pattern: "admin/support/system-roles-delegations",
               defaults: new { area = "Admin", controller = "Support", action = "RoleDelegation" }
            );

            routeBuilder.MapControllerRoute(
                name: "admin-settings",
                pattern: "admin/settings",
                defaults: new { area = "Admin", controller = "Settings", action = "Index" }
            );

            /*----------------------- Operations dashboard routes*/
            routeBuilder.MapControllerRoute(
                name: "ops-dashboard",
                pattern: "/operations/dashboard",
                defaults: new { area = "Operations", controller = "OperationDashboard", action = "Index" }
            );

            routeBuilder.MapControllerRoute(
                name: "ops-dashboard-totalprocs",
                pattern: "/operations/dashboard/totalprocs",
                defaults: new { area = "Operations", controller = "OperationDashboard", action = "TotalProcesses" }
            );
            routeBuilder.MapControllerRoute(
                name: "ops-dashboard-proposed",
                pattern: "/operations/dashboard/proposed",
                defaults: new { area = "Operations", controller = "OperationDashboard", action = "Proposed" }
            );
            routeBuilder.MapControllerRoute(
                name: "ops-dashboard-completed",
                pattern: "/operations/dashboard/completed",
                defaults: new { area = "Operations", controller = "OperationDashboard", action = "Completed" }
            );
            routeBuilder.MapControllerRoute(
                name: "ops-dashboard-dormant",
                pattern: "/operations/dashboard/dormant",
                defaults: new { area = "Operations", controller = "OperationDashboard", action = "Dormant" }
            );
            routeBuilder.MapControllerRoute(
                name: "ops-dashboard-cancelled",
                pattern: "/operations/dashboard/cancelled",
                defaults: new { area = "Operations", controller = "OperationDashboard", action = "Cancelled" }
            );
            routeBuilder.MapControllerRoute(
                name: "ops-dashboard-unchanged",
                pattern: "/operations/dashboard/unchanged",
                defaults: new { area = "Operations", controller = "OperationDashboard", action = "Unchanged" }
            );
            routeBuilder.MapControllerRoute(
                name: "ops-dashboard-due",
                pattern: "/operations/dashboard/due",
                defaults: new { area = "Operations", controller = "OperationDashboard", action = "Due" }
            );
            routeBuilder.MapControllerRoute(
                name: "ops-dashboard-account-services",
                pattern: "/operations/dashboard/account-services",
                defaults: new { area = "Operations", controller = "OperationDashboard", action = "AccountServices" }
            );
            routeBuilder.MapControllerRoute(
                name: "ops-dashboard-reconciliation",
                pattern: "/operations/dashboard/reconciliation",
                defaults: new { area = "Operations", controller = "OperationDashboard", action = "RecordsManagement" }
            );
            routeBuilder.MapControllerRoute(
                name: "ops-dashboard-wallets",
                pattern: "/operations/dashboard/wallets",
                defaults: new { area = "Operations", controller = "OperationDashboard", action = "Wallets" }
            );
            routeBuilder.MapControllerRoute(
                name: "ops-dashboard-cash",
                pattern: "/operations/dashboard/cash",
                defaults: new { area = "Operations", controller = "OperationDashboard", action = "Cash" }
            );
            routeBuilder.MapControllerRoute(
                name: "ops-dashboard-payments",
                pattern: "/operations/dashboard/payments",
                defaults: new { area = "Operations", controller = "OperationDashboard", action = "Payments" }
            );
            routeBuilder.MapControllerRoute(
                name: "ops-dashboard-channels",
                pattern: "/operations/dashboard/channels",
                defaults: new { area = "Operations", controller = "OperationDashboard", action = "Channels" }
            );
            routeBuilder.MapControllerRoute(
                name: "ops-dashboard-customerExperience",
                pattern: "/operations/dashboard/customer-experience",
                defaults: new { area = "Operations", controller = "OperationDashboard", action = "CustomerExperience" }
            );

            /*----------------------- Compliance dashboard routes*/
            routeBuilder.MapControllerRoute(
                name: "app-compliance-dashboard",
                pattern: "/grc/compliance",
                defaults: new { controller = "Application", action = "Dashboard" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-regulations-received",
                pattern: "/grc/regulations/received",
                defaults: new { controller = "Regulations", action = "ReceivedRegulations" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-regulations-open",
                pattern: "/grc/regulations/open",
                defaults: new { controller = "Regulations", action = "OpenRegulations" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-regulations-closed",
                pattern: "/grc/regulations/closed",
                defaults: new { controller = "Regulations", action = "ClosedRegulations" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-regulations-applicable",
                pattern: "/grc/regulations/applicable",
                defaults: new { controller = "Regulations", action = "RegulatoryApplicable" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-regulations-gaps",
                pattern: "/grc/regulations/gaps",
                defaults: new { controller = "Regulations", action = "RegulatoryGaps" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-regulations-covered",
                pattern: "/grc/regulations/covered",
                defaults: new { controller = "Regulations", action = "RegulatoryCovered" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-regulations-issues",
                pattern: "/grc/regulations/issues",
                defaults: new { controller = "Regulations", action = "RegulatoryIssues" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-regulations-na",
                pattern: "/grc/regulations/not-applicable",
                defaults: new { controller = "Regulations", action = "RegulatoryNotApplicable" }
            );

            routeBuilder.MapControllerRoute(
               name: "app-compliance-register-returns",
               pattern: "/grc/register/register-returns",
               defaults: new { controller = "Register", action = "RegulationReturns" }
            );
            routeBuilder.MapControllerRoute(
               name: "app-compliance-register-policies",
               pattern: "/grc/register/policies",
               defaults: new { controller = "Register", action = "PoliciesRegisters" }
            );
            routeBuilder.MapControllerRoute(
               name: "app-compliance-register-circulars",
               pattern: "/grc/register/circulars",
               defaults: new { controller = "Register", action = "RegulationMaps" }
            );
            routeBuilder.MapControllerRoute(
               name: "app-compliance-register-obligations",
               pattern: "/grc/register/obligations",
               defaults: new { controller = "Register", action = "RegulationObligations" }
            );
            routeBuilder.MapControllerRoute(
               name: "app-compliance-register-list",
               pattern: "/grc/register/register-list",
               defaults: new { controller = "Register", action = "RegulationList" }
            );

            /*----------------------- Compliance settings routes*/
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-regulatory-types",
                pattern: "/grc/compliance/settings/regulatory-types",
                defaults: new { controller = "ComplianceSettings", action = "ComplianceRegulatoryTypes" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-regulatory-categories",
                pattern: "/grc/compliance/settings/regulatory-categories",
                defaults: new { controller = "ComplianceSettings", action = "ComplianceRegulatoryCategories" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-regulatory-authorities",
                pattern: "/grc/compliance/settings/regulatory-authorities",
                defaults: new { controller = "ComplianceSettings", action = "ComplianceAuthorities" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-support-document-types",
                pattern: "/grc/compliance/support/document-types",
                defaults: new { controller = "ComplianceSettings", action = "ComplianceDocumentType" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-support-users",
                pattern: "/grc/compliance/support/users",
                defaults: new { controller = "ComplianceSettings", action = "ComplianceUsers" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-support-delegation",
                pattern: "/grc/compliance/support/delegation",
                defaults: new { controller = "ComplianceSettings", action = "ComplianceDelegation" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-support-departments",
                pattern: "/grc/compliance/support/departments",
                defaults: new { controller = "ComplianceSettings", action = "ComplianceDepartments" }
            );
            routeBuilder.MapControllerRoute(
                name: "app-compliance-settings-support-responsibilities",
                pattern: "/grc/compliance/support/responsibilities",
                defaults: new { controller = "ComplianceSettings", action = "ComplianceResponsibilities" }
            );

            /*----------------------- Application login routes*/
            routeBuilder.MapControllerRoute(
                name: "login",
                pattern: "/login/validate-username",
                defaults: new { controller = "Application", action = "ValidateUsername" }
            );

            // login user
            routeBuilder.MapControllerRoute(
                name: "login",
                pattern: "/login/userlogin",
                defaults: new { controller = "Application", action = "Login" }
            );

            // logout user
            routeBuilder.MapControllerRoute(
                name: "logout",
                pattern: "/app/logout",
                defaults: new { controller = "Application", action = "Logout" }
            );

            // No service
            routeBuilder.MapControllerRoute(
                name: "noservice",
                pattern: "/org/noservice/",
                defaults: new { controller = "Application", action = "NoService" }
            );

            // Create organization
            routeBuilder.MapControllerRoute(
                name: "register",
                pattern: "/org/register/",
                defaults: new { controller = "Application", action = "Register" }
            );
            // Create organization
            routeBuilder.MapControllerRoute(
                name: "language-change",
                pattern: "/org/changeLanguage/",
                defaults: new { controller = "Application", action = "ChangeLanguage" }
            );

            // 404
            routeBuilder.MapControllerRoute(
                name: "404",
                pattern: "/Error/Status404/",
                defaults: new { controller = "Error", action = "Status404" }
            );
            
            //..admin areas
            routeBuilder.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller}/{action=Index}/{id?}"
            );

            // User login (default)
            routeBuilder.MapControllerRoute(
                name: "default",
                pattern: "{controller=Application}/{action=Login}/{id?}"
            );

        }
    }
}

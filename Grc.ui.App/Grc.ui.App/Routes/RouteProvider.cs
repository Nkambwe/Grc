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
                name: "admin-settings",
                pattern: "admin/settings",
                defaults: new { area = "Admin", controller = "Settings", action = "Index" }
            );

            routeBuilder.MapControllerRoute(
                name: "operations-home",
                pattern: "operations/workflow",
                defaults: new { area = "Operations", controller = "OperationDashboard", action = "Index" }
            );

            // Dashboard
            routeBuilder.MapControllerRoute(
                name: "dashboard",
                pattern: "/dashboard/",
                defaults: new { controller = "Application", action = "Dashboard" } 
            );

            // validate username
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
                pattern: "{area}/{controller=Support}/{action=Index}/{id?}"
            );

            // User login (default)
            routeBuilder.MapControllerRoute(
                name: "default",
                pattern: "{controller=Application}/{action=Login}/{id?}"
            );

        }
    }
}

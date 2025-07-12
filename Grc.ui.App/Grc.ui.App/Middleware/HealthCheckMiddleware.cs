using Grc.ui.App.Services;

namespace Grc.ui.App.Middleware {

    /// <summary>
    /// Custom midlleware class to manage Health Checks
    /// </summary>
    public class HealthCheckMiddleware {
        private readonly RequestDelegate _next;
    private readonly ILogger<HealthCheckMiddleware> _logger;

    public HealthCheckMiddleware(RequestDelegate next, ILogger<HealthCheckMiddleware> logger) {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context) {

        string path = context.Request.Path.ToString().ToLower();
        
        if (path == "/" || path == "/login" || path == "/userlogin") {
            _logger.LogInformation("Health check middleware triggered for path: {Path}", path);
            
            // Get health service from current scope
            var healthCheckService = context.RequestServices.GetRequiredService<IMiddlewareHealthService>();
            
            try {
                var (status, isConnected, hasCompanies) = await healthCheckService.CheckMiddlewareStatusAsync();
                
                _logger.LogInformation("Health check results - Status: {Status}, Connected: {Connected}, HasCompanies: {HasCompanies}", 
                    status, isConnected, hasCompanies);
                
                if (!status || !isConnected) {
                    _logger.LogWarning("Redirecting to /org/noservice - Status: {Status}, Connected: {Connected}", status, isConnected);
                    context.Response.Redirect("/org/noservice");
                    return;
                } 
                
                if (!hasCompanies) {
                    _logger.LogInformation("No companies found, redirecting to /org/register");
                    context.Response.Redirect("/org/register");
                    return;
                }
                
                _logger.LogInformation("Health check passed, redirecting to /login/userlogin");
                context.Response.Redirect("/login/userlogin");
                return;
            } catch (Exception ex) {
                _logger.LogError(ex, "Error during health check");
                context.Response.Redirect("/org/noservice");
                return;
            }
        }
        
        await _next(context);
    }
    }
}

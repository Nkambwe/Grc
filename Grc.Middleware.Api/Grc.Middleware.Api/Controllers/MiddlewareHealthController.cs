using Grc.Middleware.Api.Data;
using Grc.Middleware.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Grc.Middleware.Api.Controllers {

    /// <summary>
    /// Controller handles health checks on middleware API and informs the client of middleware state
    /// </summary>
    [Route("grc")]
    public class MiddlewareHealthController: ControllerBase {
        protected readonly IServiceLogger Logger;
        private readonly IDbContextFactory<GrcContext> _contextFactory;
        private readonly IConfiguration _configuration;
        private readonly IEnvironmentProvider _environment;
    
        public MiddlewareHealthController(
            IServiceLoggerFactory loggerFactory, 
            IEnvironmentProvider environment,
            IDbContextFactory<GrcContext> contextFactory,
            IConfiguration configuration) { 
            Logger = loggerFactory.CreateLogger();
            Logger.Channel = $"HEALTH-CHECK-{DateTime.Now:yyyyMMddHHmmss}";
            _contextFactory = contextFactory;
            _configuration = configuration;
            _environment = environment;
        }
    
        [HttpGet("health/checkstatus")]
        public async Task<IActionResult> CheckDatabaseConnection() {
            Logger.LogActivity("=== Starting database health check ===", "INFO");
        
            try {
                using var dbContext = _contextFactory.CreateDbContext();
                var connectionString = dbContext.Database.GetConnectionString();
                //..log connection string details (mask sensitive parts)
                var maskedConnectionString = MaskValue(connectionString);
                Logger.LogActivity($"Using connection string: {(_environment.IsLive? maskedConnectionString: connectionString)}", "INFO");
                Logger.LogActivity($"Database provider: {dbContext.Database.ProviderName}", "INFO");
            
            } catch (Exception ex) {
                Logger.LogActivity($"Error getting connection string: {ex.Message}", "ERROR");
                Logger.LogActivity($"Stack trace: {ex.StackTrace}", "ERROR");
            }
        
            //..default hasCompanies to false
            bool hasCompanies = false;
            bool canConnect = false;
            string errorDetails = "";
        
            try {
                Logger.LogActivity("Creating database context...", "INFO");
                using var dbContext = _contextFactory.CreateDbContext();
            
                Logger.LogActivity("Testing database connection...", "INFO");
                canConnect = await dbContext.Database.CanConnectAsync();
                Logger.LogActivity($"CanConnectAsync() result: {canConnect}", "INFO");

                if (canConnect) {
                    Logger.LogActivity("Connection successful! Checking for organizations...", "INFO");
                    try {
                        hasCompanies = await dbContext.Organizations.AnyAsync();
                        Logger.LogActivity($"Organizations found: {hasCompanies}", "INFO");
                    } catch (Exception orgEx) {
                        Logger.LogActivity($"Error checking organizations: {orgEx.Message}", "ERROR");
                        //..still return connection as true since we can connect, just can't query
                        Logger.LogActivity($"{orgEx.StackTrace}", "STACKTRACE");
                    }
                
                    Logger.LogActivity($"Final result: {{status:true, isConnected:true, hasCompanies:{hasCompanies}}}", "INFO");
                } else {
                    Logger.LogActivity("Database connection failed", "WARN");

                    try {
                        //..try to get more details about why connection failed
                        await dbContext.Database.OpenConnectionAsync();
                        Logger.LogActivity("OpenConnectionAsync succeeded despite CanConnectAsync being false", "WARNING");
                    } catch (Exception connEx) {
                        Logger.LogActivity($"OpenConnectionAsync failed: {connEx.Message}", "ERROR");
                        if (connEx.InnerException != null) {
                            Logger.LogActivity($"Inner exception: {connEx.InnerException.Message}", "ERROR");
                        }
                        Logger.LogActivity($"OpenConnectionAsync failed: {connEx.StackTrace}", "STACKTRACE");
                    }
                
                    Logger.LogActivity("Final result: {status:true, isConnected:false, hasCompanies:false}", "INFO");
                }
            
            } catch (Exception ex) {
                Logger.LogActivity($"Exception during health check: {ex.Message}", "ERROR");
                Logger.LogActivity($"Exception type: {ex.GetType().Name}", "ERROR");
                Logger.LogActivity($"Stack trace: {ex.StackTrace}", "ERROR");
            
                if (ex.InnerException != null) {
                    Logger.LogActivity($"Inner exception: {ex.InnerException.Message}", "ERROR");
                    Logger.LogActivity($"Inner exception type: {ex.InnerException.GetType().Name}", "ERROR");
                }
            
                errorDetails = ex.Message;
                Logger.LogActivity("Final result: {status:false, isConnected:false, hasCompanies:false}", "INFO");
            }
        
            Logger.LogActivity("=== Database health check completed ===", "INFO");
        
            var result = new { 
                status = true, 
                isConnected = canConnect, 
                hasCompanies,
                errorDetails = string.IsNullOrEmpty(errorDetails) ? null : errorDetails,
                timestamp = DateTime.UtcNow
            };
        
            return Ok(result);
        }
    
        /// <summary>
        /// Mask Value in connection string
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <returns></returns>
        private static string MaskValue(string connectionString) {
            if (string.IsNullOrEmpty(connectionString)) return "NULL";
        
            // Mask sensitive information in connection string
            var parts = connectionString.Split(';');
            var maskedParts = new List<string>();
        
            foreach (var part in parts) {
                var trimmedPart = part.Trim();
                if (trimmedPart.StartsWith("Password=", StringComparison.OrdinalIgnoreCase) ||
                    trimmedPart.StartsWith("Pwd=", StringComparison.OrdinalIgnoreCase)) {
                    maskedParts.Add("Password=***MASKED***");
                } else if (trimmedPart.StartsWith("User ID=", StringComparison.OrdinalIgnoreCase) ||
                           trimmedPart.StartsWith("UID=", StringComparison.OrdinalIgnoreCase)) {
                    var userPart = trimmedPart.Split('=');
                    if (userPart.Length > 1) {
                        maskedParts.Add($"{userPart[0]}={userPart[1].Substring(0, Math.Min(2, userPart[1].Length))}***");
                    }
                } else {
                    maskedParts.Add(trimmedPart);
                }
            }
        
            return string.Join("; ", maskedParts);
        }
    }
}

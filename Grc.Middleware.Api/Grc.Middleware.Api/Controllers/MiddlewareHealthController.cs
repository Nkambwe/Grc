using Grc.Middleware.Api.Data;
using Grc.Middleware.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Grc.Middleware.Api.Controllers {

    /// <summary>
    /// Controller handles health checks on middleware API and informs the client of middleware state
    /// </summary>
    [ApiController]
    [Route("grc")]
    public class MiddlewareHealthController: ControllerBase {
        protected readonly IServiceLogger Logger;
        private readonly IDbContextFactory<GrcContext> _contextFactory;

        public MiddlewareHealthController(IServiceLoggerFactory loggerFactory, IDbContextFactory<GrcContext> contextFactory) { 
            Logger = loggerFactory.CreateLogger();
            Logger.Channel = $"HEALTH-CHECK{DateTime.Now:yyyyMMddHHmmss}";
            _contextFactory = contextFactory;
            
        }

        [HttpGet("health/checkstatus")]
        public async Task<IActionResult> CheckDatabaseConnection() {
            Logger.LogActivity("Checking database availability...", "INFO");

            
            // Default hasCompanies to false
            bool hasCompanies = false;
            try {
                using var dbContext = _contextFactory.CreateDbContext();
                var canConnect = await dbContext.Database.CanConnectAsync();

                if (canConnect) {
                     hasCompanies = await dbContext.Organizations.AnyAsync();   
                    Logger.LogActivity($"DB Connection status: {{status:true, isConnected:true, hasCompanies:{hasCompanies}}}");
                } else {
                    Logger.LogActivity("DB Connection status: {status:true, isConnected:false, hasCompanies:false}");
                }

                return Ok(new { status = true, isConnected = canConnect, hasCompanies });
            } catch {
                 Logger.LogActivity("DB Connection status: {status:false, isConnected=false}");
                return Ok(new { status = true, isConnected = false, hasCompanies });
            }
        }
    }
}

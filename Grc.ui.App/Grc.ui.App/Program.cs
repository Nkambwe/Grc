
namespace Grc.ui.App { 
    
    public class Program { 
        public static void Main(string[] args) { 
            var builder = WebApplication.CreateBuilder(args);

            //..create and configure Startup
            var startup = new Startup(builder.Configuration);  
            startup.ConfigureServices(builder.Services);
        
            //..build the application
            var app = builder.Build();
        
            //..configure the application pipeline
            startup.Configure(app);
        
            app.Run();
        }
    }
}



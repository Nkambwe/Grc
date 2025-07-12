namespace Grc.ui.App.Infrastructure.AppSettings {

    public class GeneralSettings {

        /// <summary>
        /// Gets or sets a value indicating whether to display the full error in production environment. 
        /// It's ignored (always enabled) in development environment
        /// </summary>
        public bool DisplayFullErrorStack { get; set; } = false;
        
        /// <summary>
        /// Gets or sets a value indicating whether to display warning messages to users
        /// </summary>
        public bool DisplayWaringMessages { get; set; } = false;
    }
}

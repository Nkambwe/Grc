namespace Grc.ui.App.Routes {
    
    /// <summary>
    /// Route provider
    /// </summary>
    public interface IRouteProvider {
        
        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder);

    }
}

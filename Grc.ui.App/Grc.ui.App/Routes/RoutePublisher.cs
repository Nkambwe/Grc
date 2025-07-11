namespace Grc.ui.App.Routes {

    public class RoutePublisher: IRoutePublisher  {

        private readonly IEnumerable<IRouteProvider> _routeProviders;

        public RoutePublisher(IEnumerable<IRouteProvider> routeProviders) { 
            _routeProviders = routeProviders;
        }

        public virtual void RegisterRoutes(IEndpointRouteBuilder routeBuilder) {
            //register all provided routes
            foreach (var routeProvider in _routeProviders.OrderByDescending(rp => rp.Priority)) {
                routeProvider.RegisterRoutes(routeBuilder);
            }
        }

    }
}

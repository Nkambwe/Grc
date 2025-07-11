using Grc.Middleware.Api.Utils;
using Microsoft.EntityFrameworkCore;

namespace Grc.Middleware.Api.Data.Containers {

     public class UnitOfWorkFactory : IUnitOfWorkFactory {

        private readonly IServiceLoggerFactory _loggerFactory;
        private readonly IDbContextFactory<GrcContext> _contextFactory;
        private readonly IServiceProvider _serviceProvider;

        public UnitOfWorkFactory(IServiceLoggerFactory loggerFactory,
                                 IDbContextFactory<GrcContext> contextFactory,
                                 IServiceProvider serviceProvider) {
            _loggerFactory = loggerFactory;
            _contextFactory = contextFactory;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Creates a new instance of UnitOfWork.
        /// </summary>
        /// <returns>Created instance of IUnitOfWork</returns>
        public IUnitOfWork Create() => new UnitOfWork(_loggerFactory, _contextFactory, _serviceProvider);
     }
}

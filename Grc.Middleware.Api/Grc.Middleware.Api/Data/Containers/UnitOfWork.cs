using Grc.Middleware.Api.Data.Entities;
using Grc.Middleware.Api.Data.Repositories;
using Grc.Middleware.Api.Utils;
using Microsoft.EntityFrameworkCore;

namespace Grc.Middleware.Api.Data.Containers {

    public class UnitOfWork : IUnitOfWork {
        private readonly IServiceLogger _logger;
        private readonly IServiceLoggerFactory _loggerFactory;
        private readonly IDbContextFactory<GrcContext> _contextFactory;
        private readonly Dictionary<Type, object> _repositories;
        private readonly IServiceProvider _serviceProvider;
        private GrcContext _context; 
        private bool _disposed;

        public ICompanyRepository CompanyRepository { get; private set; }

        public UnitOfWork(IServiceLoggerFactory loggerFactory,
                          IDbContextFactory<GrcContext> contextFactory,
                          IServiceProvider serviceProvider) {
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger("grc_data");
            _logger.Channel = $"UOW-{DateTime.Now:yyyyMMddHHmmss}";
            _contextFactory = contextFactory;
            _serviceProvider = serviceProvider;
            _repositories = new Dictionary<Type, object>();
        
            //.. db context instance for this unit of work
            _context = _contextFactory.CreateDbContext();
        
            //..initalize repositories
            CompanyRepository = _serviceProvider.GetService<ICompanyRepository>() ?? new CompanyRepository(_loggerFactory, _context);
        }

        /// <summary>
        /// Get an instance of a repository class of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Entity type repository belongs to</typeparam>
        /// <returns>Repository of type specified</returns>
        public IRepository<T> GetRepository<T>() where T : BaseEntity {
            if (_repositories.TryGetValue(typeof(T), out var repo)) {
                return (IRepository<T>)repo;
            }

            // Create repository with shared context instead of context factory
            var repository = new Repository<T>(_loggerFactory, _context);
            _repositories[typeof(T)] = repository;
            return repository;
        }

        /// <summary>
        /// Save all changes made in this unit of work
        /// </summary>
        /// <returns>Number of affected records</returns>
        public async Task<int> SaveChangesAsync() {
            try {
                return await _context.SaveChangesAsync();
            } catch (Exception ex) {
                _logger.LogActivity($"SaveChangesAsync failed: {ex.Message}", "DBOPS");
                _logger.LogActivity($"STACKTRACE: {ex.StackTrace}", "ERROR");
                throw;
            }
        }

        /// <summary>
        /// Save all changes made in this unit of work synchronously
        /// </summary>
        /// <returns>Number of affected records</returns>
        public int SaveChanges() {
            try {
                return _context.SaveChanges();
            } catch (Exception ex) {
                _logger.LogActivity($"SaveChanges failed: {ex.Message}", "DBOPS");
                _logger.LogActivity($"STACKTRACE: {ex.StackTrace}", "ERROR");
                throw;
            }
        }

        /// <summary>
        /// Manual disposal of unit of work
        /// </summary>
        /// <param name="isManuallyDisposing"></param>
        protected virtual void Dispose(bool isManuallyDisposing) {
            if (!_disposed) {
                if (isManuallyDisposing) {
                    //..dispose the context
                    _context?.Dispose();
                
                    //..clear repositories
                    _repositories.Clear();
                
                    //..clear repository references
                    CompanyRepository = null;
                }
            }
            _disposed = true;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~UnitOfWork() {
            Dispose(false);
        }
    }
}

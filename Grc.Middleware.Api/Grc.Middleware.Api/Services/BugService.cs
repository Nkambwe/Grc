using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Utils;

namespace Grc.Middleware.Api.Services {
    public class BugService : BaseService, IBugService {
        public BugService(IServiceLoggerFactory loggerFactory, 
                            IUnitOfWorkFactory uowFactory, 
                            IMapper mapper) 
            : base(loggerFactory, uowFactory, mapper) {
        }

        public async Task<PagedResult<SystemError>> GetBugsAsync(BugListRequest request) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Retrieve all system bugs", "INFO");
            try {
                return await uow.SystemErrorRespository.PageAllAsync(
                    request.PageIndex, 
                    request.PageSize,false);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve system erors: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                await uow.SaveChangesAsync();
                throw;
            }
        }

        
        #region Private Methods

        private SystemError HandleError(IUnitOfWork uow, Exception ex) {
            var innerEx = ex.InnerException;
            while (innerEx != null) {
                Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                innerEx = innerEx.InnerException;
            }
            Logger.LogActivity($"{ex.StackTrace}", "ERROR");

            var company = uow.CompanyRepository.GetAll(false).FirstOrDefault();
            long companyId = company != null ? company.Id : 1;
            return new() {
                ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                ErrorSource = "GUG-SERVICE",
                StackTrace = ex.StackTrace,
                Severity = "CRITICAL",
                ReportedOn = DateTime.Now,
                CompanyId = companyId,
                CreatedBy = "SYSTEM",
                CreatedOn = DateTime.Now
            };

        }

        #endregion

    }
}

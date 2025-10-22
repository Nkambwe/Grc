using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Org;
using Grc.Middleware.Api.Utils;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Services.Organization {

    public class CompanyService : BaseService, ICompanyService {

        public CompanyService(IUnitOfWorkFactory unitOfWorkFactory, 
                             IServiceLoggerFactory loggerFactory, 
                             IMapper mapper)
            : base(loggerFactory, unitOfWorkFactory, mapper) {
            Logger.Channel = $"COMPANY-{DateTime.Now:yyyMMddHHmmss}";
        }

        public async Task<Company> GetDefaultCompanyAsync() {

            using var uow = UowFactory.Create();
            Logger.LogActivity("Get default compay record>>>>", "INFO");
            try {
                return(await uow.CompanyRepository.GetAllAsync()).FirstOrDefault();
            } catch (Exception ex) {
                Logger.LogActivity($"GetDefaultCompanyAsync failed: {ex.Message}", "ERROR");
        
                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
        
                //..re-throw to the controller handle
                throw; 
            }
        }

        public async Task<bool> CreateCompanyAsync(Company company) {

            using var uow = UowFactory.Create();
            Logger.LogActivity("Save company record >>>>", "INFO");
    
            try {
                //..log the company data being saved
                var companyJson = JsonSerializer.Serialize(company, new JsonSerializerOptions { 
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles 
                });
                Logger.LogActivity($"Company data: {companyJson}", "DEBUG");
        
                await uow.CompanyRepository.InsertAsync(company);
        
                //..check entity state
                var entityState = ((UnitOfWork)uow).Context.Entry(company).State;
                Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");
        
                var result = await uow.SaveChangesAsync();
                Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
        
                return result > 0;
            } catch (Exception ex) {
                Logger.LogActivity($"CreateCompanyAsync failed: {ex.Message}", "ERROR");
        
                //..log inner exceptions here too
                var innerEx = ex.InnerException;
                while (innerEx != null) {
                    Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                    innerEx = innerEx.InnerException;
                }
        
                //..re-throw to the controller handle
                throw; 
            }
        }

    }
}

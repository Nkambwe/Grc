using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities;
using Grc.Middleware.Api.Utils;
using System.Text.Json;

namespace Grc.Middleware.Api.Services {

    public class CompanyService : BaseService, ICompanyService {

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public CompanyService(IUnitOfWorkFactory unitOfWorkFactory, IServiceLoggerFactory loggerFactory)
            : base(loggerFactory) {
             _unitOfWorkFactory = unitOfWorkFactory;
            Logger.Channel = $"COMPANY-{DateTime.Now:yyyMMddHHmmss}";
        }

        public async Task<bool> CreateCompanyAsync(Company company) {
            using var uow = _unitOfWorkFactory.Create();
            Logger.LogActivity("Save company record >>>>", "INFO");
            Logger.LogActivity(JsonSerializer.Serialize(company), "INFO");
            await uow.CompanyRepository.InsertAsync(company);
            var result = await uow.SaveChangesAsync();
            return result > 0;
        }

        //public async Task<bool> TransferEmployees(long fromCompanyId, long toCompanyId) {
        //    using var uow = _unitOfWorkFactory.Create();
        
        //    var employeeRepo = uow.GetRepository<Employee>();
        //    var employees = await employeeRepo.GetAllAsync(e => e.CompanyId == fromCompanyId);
        
        //    foreach (var employee in employees) {
        //        employee.CompanyId = toCompanyId;
        //        await employeeRepo.UpdateAsync(employee);
        //    }
        
        //    var result = await uow.SaveChangesAsync();
        //    return result > 0;
        //}
    }
}

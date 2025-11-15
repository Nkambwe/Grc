using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Operations.Processes;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Utils;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Services
{
    public class ProcessApprovalService : BaseService, IProcessApprovalService
    {
        public ProcessApprovalService(IServiceLoggerFactory loggerFactory,
                                      IUnitOfWorkFactory uowFactory,
                                      IMapper mapper) : base(loggerFactory, uowFactory, mapper)
        {
        }

        public int Count() {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of approved processes in the database", "INFO");

            try {
                return uow.ProcessApprovalRepository.Count();
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count approved processes in the database: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                uow.SaveChanges();
                throw;
            }
        }

        public int Count(Expression<Func<OperationProcess, bool>> predicate)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of approved processes in the database", "INFO");

            try
            {
                return uow.OperationProcessRepository.Count(predicate);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count approved processes in the database: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                uow.SaveChanges();
                throw;
            }
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of approved processes in the database", "INFO");

            try
            {
                return await uow.OperationProcessRepository.CountAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Processes in the database: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                await uow.SaveChangesAsync();
                throw;
            }
        }

        public async Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of  Processes in the database", "INFO");

            try
            {
                return await uow.ProcessApprovalRepository.CountAsync(excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count approved processes in the database: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                await uow.SaveChangesAsync();
                throw;
            }
        }

        public async Task<ProcessApproval> GetAsync(long id, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get approved process with ID '{id}'", "INFO");

            try
            {
                return await uow.ProcessApprovalRepository.GetAsync(id, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve approved process : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                await uow.SaveChangesAsync();
                throw;
            }
        }

        public async Task<bool> InsertAsync(ProcessApprovalRequest request)
        {
            using var uow = UowFactory.Create();
            try
            {
                var approval = Mapper.Map<ProcessApprovalRequest, ProcessApproval>(request);

                //..log the Process Approval data being saved
                var approvalJson = JsonSerializer.Serialize(approval, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Process Approval data: {approvalJson}", "DEBUG");

                var added = await uow.ProcessApprovalRepository.InsertAsync(approval);
                if (added)
                {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(approval).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save Process Approval : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                await uow.SaveChangesAsync();
                throw;
            }
        }

        public async Task<bool> UpdateAsync(ProcessApprovalRequest request, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update Process Approval", "INFO");

            try {
                var approval = await uow.ProcessApprovalRepository.GetAsync(a => a.Id == request.Id);
                if (approval != null) {

                    approval.RequestDate = request.RequestDate;

                    if (request.HODStart.HasValue)
                        approval.HeadOfDepartmentStart = request.HODStart;

                    if (request.HODEnd.HasValue)
                        approval.HeadOfDepartmentEnd = request.HODEnd;

                    if (!string.IsNullOrWhiteSpace(request.HODStatus))
                        approval.HeadOfDepartmentStatus = (request.HODStatus ?? string.Empty).Trim();

                    if (!string.IsNullOrWhiteSpace(request.HODComment))
                        approval.HeadOfDepartmentComment = (request.HODComment ?? string.Empty).Trim();

                    if (request.RiskStart.HasValue)
                        approval.RiskStart = request.RiskStart;

                    if (request.RiskEnd.HasValue)
                        approval.RiskEnd = request.RiskEnd;

                    if (!string.IsNullOrWhiteSpace(request.RiskStatus))
                        approval.RiskStatus = (request.RiskStatus ?? string.Empty).Trim();

                    if (!string.IsNullOrWhiteSpace(request.RiskComment))
                        approval.RiskComment = (request.RiskComment ?? string.Empty).Trim();

                    if (request.ComplianceStart.HasValue)
                        approval.ComplianceStart = request.ComplianceStart;

                    if (request.ComplianceEnd.HasValue)
                        approval.ComplianceEnd = request.ComplianceEnd;

                    if (!string.IsNullOrWhiteSpace(request.ComplianceStatus))
                        approval.ComplianceStatus = (request.ComplianceStatus ?? string.Empty).Trim();

                    if (!string.IsNullOrWhiteSpace(request.ComplianceComment))
                        approval.ComplianceComment = (request.ComplianceComment ?? string.Empty).Trim();

                    if (request.BOPStart.HasValue)
                        approval.BranchOperationsStatusStart = request.BOPStart;

                    if (request.BOPEnd.HasValue)
                        approval.BranchOperationsStatusEnd = request.BOPEnd;

                    if (!string.IsNullOrWhiteSpace(request.BOPStatus))
                        approval.BranchOperationsStatus = (request.BOPStatus ?? string.Empty).Trim();

                    if (!string.IsNullOrWhiteSpace(request.BOPComment))
                        approval.BranchManagerComment = (request.BOPComment ?? string.Empty).Trim();

                    if (request.CreditStart.HasValue)
                        approval.CreditStart = request.CreditStart;

                    if (request.CreditEnd.HasValue)
                        approval.CreditEnd = request.CreditEnd;

                    if (!string.IsNullOrWhiteSpace(request.CreditStatus))
                        approval.CreditStatus = (request.CreditStatus ?? string.Empty).Trim();

                    if (!string.IsNullOrWhiteSpace(request.CreditComment))
                        approval.CreditComment = (request.CreditComment ?? string.Empty).Trim();

                    if (request.TreasuryStart.HasValue)
                        approval.TreasuryStart = request.TreasuryStart;

                    if (request.TreasuryEnd.HasValue)
                        approval.TreasuryEnd = request.TreasuryEnd;

                    if (!string.IsNullOrWhiteSpace(request.TreasuryStatus))
                        approval.TreasuryStatus = (request.TreasuryStatus ?? string.Empty).Trim();

                    if (!string.IsNullOrWhiteSpace(request.TreasuryComment))
                        approval.TreasuryComment = (request.TreasuryComment ?? string.Empty).Trim();

                    if (request.FintechStart.HasValue)
                        approval.FintechStart = request.FintechStart;

                    if (request.FintechEnd.HasValue)
                        approval.FintechEnd = request.FintechEnd;

                    if (!string.IsNullOrWhiteSpace(request.FintechStatus))
                        approval.FintechStatus = (request.FintechStatus ?? string.Empty).Trim();

                    if (!string.IsNullOrWhiteSpace(request.FintechComment))
                        approval.FintechComment = (request.FintechComment ?? string.Empty).Trim();

                    if (request.IsDeleted)
                        approval.IsDeleted = request.IsDeleted;

                    if(!string.IsNullOrWhiteSpace(request.ModifiedBy))
                        approval.LastModifiedBy = $"{request.ModifiedBy}";

                    if(request.ModifiedOn.HasValue)
                        approval.LastModifiedOn = request.ModifiedOn;

                    approval.ProcessId = request.ProcessId;


                    //..check entity state
                    _ = await uow.ProcessApprovalRepository.UpdateAsync(approval, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(approval).State;
                    Logger.LogActivity($"Process approval state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to update process approval record: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Delete(IdRequest request) {
            using var uow = UowFactory.Create();
            try {
                var processJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Process approval data: {processJson}", "DEBUG");

                var process = uow.ProcessApprovalRepository.Get(t => t.Id == request.RecordId);
                if (process != null) {
                    //..mark as delete this OperationProcess
                    _ = uow.ProcessApprovalRepository.Delete(process, request.markAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(process).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete process approval : {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> DeleteAsync(IdRequest request)
        {
            using var uow = UowFactory.Create();
            try
            {
                var processJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Process approval data: {processJson}", "DEBUG");

                var approval = await uow.ProcessApprovalRepository.GetAsync(t => t.Id == request.RecordId);
                if (approval != null)
                {
                    //..mark as delete this Process Approval
                    _ = await uow.ProcessApprovalRepository.DeleteAsync(approval, request.markAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(approval).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = await uow.SaveChangesAsync();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete Process Approval : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<ProcessApproval>> PageProcessApprovalStatusAsync(int page, int size, bool includeDeleted, params Expression<Func<ProcessApproval, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Retrieve paged approved processes", "INFO");
            try
            {
                return await uow.ProcessApprovalRepository.PageAllAsync(page, size, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve approved processes: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                await uow.SaveChangesAsync();
                throw;
            }
        }

        private SystemError HandleError(IUnitOfWork uow, Exception ex)
        {
            var innerEx = ex.InnerException;
            while (innerEx != null)
            {
                Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                innerEx = innerEx.InnerException;
            }
            Logger.LogActivity($"{ex.StackTrace}", "ERROR");

            var company = uow.CompanyRepository.GetAll(false).FirstOrDefault();
            long companyId = company != null ? company.Id : 1;
            return new()
            {
                ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                ErrorSource = "PROCESSES-APPROVAL-SERVICE",
                StackTrace = ex.StackTrace,
                Severity = "CRITICAL",
                ReportedOn = DateTime.Now,
                CompanyId = companyId,
                CreatedBy = "SYSTEM",
                CreatedOn = DateTime.Now
            };

        }

    }
}

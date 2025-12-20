using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Operations.Processes;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Enums;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Utils;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Services {

    public class ProcessApprovalService : BaseService, IProcessApprovalService {

        public ProcessApprovalService(IServiceLoggerFactory loggerFactory, IUnitOfWorkFactory uowFactory, IMapper mapper) 
            : base(loggerFactory, uowFactory, mapper) { }

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

        public async Task<bool> ExistsAsync(Expression<Func<ProcessApproval, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an Processes approval exists in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.ProcessApprovalRepository.ExistsAsync(predicate, excludeDeleted, token);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for Processes Approval in the database: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
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

        public async Task<ProcessApproval> GetAsync(Expression<Func<ProcessApproval, bool>> predicate, bool includeDeleted = false, params Expression<Func<ProcessApproval, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Process TAT that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.ProcessApprovalRepository.GetAsync(predicate, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Process TAT : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<ProcessApproval>> GetAllAsync(bool includeDeleted = false, params Expression<Func<ProcessApproval, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Processes that fit predicate '{includes}'", "INFO");

            try
            {
                return await uow.ProcessApprovalRepository.GetAllAsync(includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Processes : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
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

        public async Task<(bool, ApprovalStage)> ApproveProcessAsync(ApprovalRequest request, bool includeDeleted = false) {
            
            using var uow = UowFactory.Create();
           
            Logger.LogActivity($"Update Process Approval", "INFO");
            var stage = ApprovalStage.NONE;

            try {
                var approval = await uow.ProcessApprovalRepository.GetAsync(a => a.Id == request.Id, false, a => a.Process);
                if (approval == null) {
                    Logger.LogActivity($"Record not found: ID={request.Id}", "DEBUG");
                    return (false, stage);
                }

                //..update HOD section
                if (!string.IsNullOrEmpty(request.HodStatus)) {
                    if (request.HodStatus == "APPROVED" || request.HodStatus == "REJECTED") {

                        if(!approval.HeadOfDepartmentStart.HasValue) 
                            approval.HeadOfDepartmentStart = approval.RequestDate;

                        approval.HeadOfDepartmentStatus = request.HodStatus;
                        approval.HeadOfDepartmentComment = request.HodComment;
                        if (!approval.HeadOfDepartmentEnd.HasValue) {
                            stage = ApprovalStage.HOD;
                            approval.HeadOfDepartmentEnd = DateTime.Now;
                            approval.RiskStart = DateTime.Now;
                            approval.RiskStatus = "PENDING";
                        }
                            
                    }
                }

                //..update Risk only if HOD is approved
                if (approval.HeadOfDepartmentStatus == "APPROVED" && !string.IsNullOrEmpty(request.RiskStatus)) {
                    if (request.RiskStatus == "APPROVED" || request.RiskStatus == "REJECTED") {
                        approval.RiskStatus = request.RiskStatus;
                        approval.RiskComment = request.RiskComment;
                        if (!approval.RiskEnd.HasValue) {
                            approval.RiskEnd = DateTime.Now;
                        }
                        stage = ApprovalStage.RISK;
                        approval.ComplianceStart = DateTime.Now;
                        approval.ComplianceStatus = "PENDING";
                    }
                }

                //..update Compliance only if Risk is approved
                if (approval.HeadOfDepartmentStatus == "APPROVED" && approval.RiskStatus == "APPROVED" && !string.IsNullOrEmpty(request.ComplianceStatus)) {
                    if (request.ComplianceStatus == "APPROVED" || request.ComplianceStatus == "REJECTED") {
                        approval.ComplianceStatus = request.ComplianceStatus;
                        approval.ComplianceComment = request.ComplianceComment;
                        stage = ApprovalStage.COMP;
                        if (!approval.ComplianceEnd.HasValue) {
                            approval.ComplianceEnd = DateTime.Now;
                        }

                        //..approval stages completed, set process to uptodate stage
                        if (!request.BopRequired && !request.TreasuryRequired && !request.CreditRequired && !request.FintechRequired) {
                            approval.Process.ProcessStatus = "UPTODATE";
                            approval.Process.EffectiveDate = DateTime.Now;
                            approval.Process.LastUpdated = DateTime.Now;
                        } else if (request.BopRequired) {
                            approval.BranchOperationsStatusStart = DateTime.Now;
                            approval.BranchOperationsStatus = "PENDING";
                        }

                    }
                }

                //..update BOP only if required and Compliance is approved
                if (request.BopRequired && approval.ComplianceStatus == "APPROVED" && !string.IsNullOrEmpty(request.BopStatus)) {
                    if (request.BopStatus == "APPROVED" || request.BopStatus == "REJECTED")
                    {
                        approval.BranchOperationsStatus = request.BopStatus;
                        approval.BranchManagerComment = request.BopComment;
                        stage = ApprovalStage.BOM;
                        if (!approval.BranchOperationsStatusEnd.HasValue) {
                            approval.BranchOperationsStatusEnd = DateTime.Now;  
                        }

                        //..approval stages completed, set process to uptodate stage
                        if (!request.TreasuryRequired && !request.CreditRequired && !request.FintechRequired) {
                            approval.Process.ProcessStatus = "UPTODATE";
                            approval.Process.EffectiveDate = DateTime.Now;
                            approval.Process.LastUpdated = DateTime.Now;
                        } else if (request.TreasuryRequired) {
                            approval.TreasuryStart = DateTime.Now;
                            approval.TreasuryStatus = "PENDING";
                        }
                    }
                } else {
                    if (request.TreasuryRequired)
                    {
                        stage = ApprovalStage.TREA;
                        approval.TreasuryStart = DateTime.Now;
                        approval.TreasuryStatus = "PENDING";
                    }
                }

                    //..check if BOP is complete or not required
                    bool bopComplete = !request.BopRequired || approval.BranchOperationsStatus == "APPROVED";

                //..update Treasury only if required and all previous are approved
                if (request.TreasuryRequired && approval.ComplianceStatus == "APPROVED" && bopComplete && !string.IsNullOrEmpty(request.TreasuryStatus)) {
                    if (request.TreasuryStatus == "APPROVED" || request.TreasuryStatus == "REJECTED") {
                        approval.TreasuryStatus = request.TreasuryStatus;
                        approval.TreasuryComment = request.TreasuryComment;
                        stage = ApprovalStage.TREA;
                        if (!approval.TreasuryEnd.HasValue) {
                            approval.TreasuryEnd = DateTime.Now;
                        }

                        //..approval stages completed, set process to uptodate stage
                        if (!request.CreditRequired && !request.FintechRequired) {
                            approval.Process.ProcessStatus = "UPTODATE";
                            approval.Process.EffectiveDate = DateTime.Now;
                            approval.Process.LastUpdated = DateTime.Now;
                        } else if (request.CreditRequired) {
                            approval.CreditStart = DateTime.Now;
                            approval.CreditStatus = "PENDING";
                        }
                    }
                }
                else
                {
                    if (request.CreditRequired)
                    {
                        stage = ApprovalStage.CRT;
                        approval.CreditStart = DateTime.Now;
                        approval.CreditStatus = "PENDING";
                    }
                }

                //..check if Treasury is complete or not required
                bool treasuryComplete = !request.TreasuryRequired || approval.TreasuryStatus == "APPROVED";

                //..update Credit section if required and all previous are approved
                if (request.CreditRequired && approval.ComplianceStatus == "APPROVED" && bopComplete && treasuryComplete && !string.IsNullOrEmpty(request.CreditStatus)) {
                    if (request.CreditStatus == "APPROVED" || request.CreditStatus == "REJECTED") {
                        approval.CreditStatus = request.CreditStatus;
                        approval.CreditComment = request.CreditComment;
                        stage = ApprovalStage.CRT;
                        if (!approval.CreditEnd.HasValue) {
                            approval.CreditEnd = DateTime.Now;
                        }

                        //..approval stages completed, set process to uptodate stage
                        if (!request.FintechRequired) {
                            approval.Process.ProcessStatus = "UPTODATE";
                            approval.Process.EffectiveDate = DateTime.Now;
                            approval.Process.LastUpdated = DateTime.Now;
                        } else if (request.FintechRequired) {
                            approval.FintechStart = DateTime.Now;
                            approval.FintechStatus = "PENDING";
                        }
                    }
                }
                else
                {
                    if (request.FintechRequired)
                    {
                        stage = ApprovalStage.FIN;
                        approval.FintechStart = DateTime.Now;
                        approval.FintechStatus = "PENDING";
                    }
                }

                // Check if Credit is complete or not required
                bool creditComplete = !request.CreditRequired || approval.CreditStatus == "APPROVED";

                // Update Fintech section - only if required and all previous are approved
                if (request.FintechRequired && approval.ComplianceStatus == "APPROVED" && bopComplete && treasuryComplete && creditComplete && !string.IsNullOrEmpty(request.FintechStatus)) {
                    if (request.FintechStatus == "APPROVED" || request.FintechStatus == "REJECTED") {
                        approval.FintechStatus = request.FintechStatus;
                        approval.FintechEnd = DateTime.Now;
                        approval.FintechComment = request.FintechComment;
                        approval.Process.ProcessStatus = "UPTODATE";
                        approval.Process.EffectiveDate = DateTime.Now;
                        approval.Process.LastUpdated = DateTime.Now;
                    }
                }

                approval.LastModifiedBy = $"{request.ModifiedBy}";
                approval.LastModifiedOn = DateTime.Now;

                //..check entity state
                _ = await uow.ProcessApprovalRepository.UpdateAsync(approval, includeDeleted);
                var entityState = ((UnitOfWork)uow).Context.Entry(approval).State;
                Logger.LogActivity($"Process approval state after Update: {entityState}", "DEBUG");

                var result = uow.SaveChanges();
                Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                return (result > 0, stage);
            } catch (Exception ex) {
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
                return await uow.ProcessApprovalRepository.PageAllAsync(page, size, includeDeleted, null, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve approved processes: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                await uow.SaveChangesAsync();
                throw;
            }
        }

        #region Private Methods

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

        #endregion

    }
}

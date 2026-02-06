using AutoMapper;
using Azure.Core;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Compliance.Regulations;
using Grc.Middleware.Api.Data.Entities.Support;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Http.Responses;
using Grc.Middleware.Api.TaskHandler;
using Grc.Middleware.Api.Utils;
using Microsoft.Win32;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Services.Compliance.Regulations {

    public class RegulatoryDocumentService : BaseService, IRegulatoryDocumentService {

        private readonly IMailTaskQueue _mailTaskQueue;
        public RegulatoryDocumentService(IServiceLoggerFactory loggerFactory, 
                                         IUnitOfWorkFactory uowFactory, 
                                         IMailTaskQueue mailTaskQueue, 
                                         IMapper mapper)
            : base(loggerFactory, uowFactory, mapper) {
            _mailTaskQueue = mailTaskQueue;
        }

        #region Queries
        public int Count()
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Regulatory Document in the database", "INFO");

            try
            {
                return uow.RegulatoryDocumentRepository.Count();
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to Regulatory Document in the database: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public int Count(Expression<Func<RegulatoryDocument, bool>> predicate)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Regulatory Document in the database", "INFO");

            try
            {
                return uow.RegulatoryDocumentRepository.Count(predicate);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Regulatory Document in the database: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Regulatory Document in the database", "INFO");

            try
            {
                return await uow.RegulatoryDocumentRepository.CountAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Regulatory Document in the database: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of  Regulatory Documents in the database", "INFO");

            try
            {
                return await uow.StatutoryArticleRepository.CountAsync(excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Regulatory Document in the database: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<int> CountAsync(Expression<Func<RegulatoryDocument, bool>> predicate, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Regulatory Document in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.RegulatoryDocumentRepository.CountAsync(predicate, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Regulatory Document in the database: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<int> CountAsync(Expression<Func<RegulatoryDocument, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Regulatory Document in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.RegulatoryDocumentRepository.CountAsync(predicate, excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Regulatory Document in the database: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Exists(Expression<Func<RegulatoryDocument, bool>> predicate, bool excludeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an Regulatory Document exists in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.RegulatoryDocumentRepository.Exists(predicate, excludeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for Regulatory Document in the database: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<RegulatoryDocument, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an Regulatory Document exists in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.RegulatoryDocumentRepository.ExistsAsync(predicate, excludeDeleted, token);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for Regulatory Document in the database: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<RegulatoryDocument, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check for batch Regulatory Document if they exist in the database that fit predicate >> '{predicates}'", "INFO");

            try
            {
                return await uow.RegulatoryDocumentRepository.ExistsBatchAsync(predicates, excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for Regulatory Document in the database: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public RegulatoryDocument Get(long id, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Regulatory Document with ID '{id}'", "INFO");

            try
            {
                return uow.RegulatoryDocumentRepository.Get(id, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Regulatory Document: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public RegulatoryDocument Get(Expression<Func<RegulatoryDocument, bool>> predicate, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Regulatory Document that fits predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.RegulatoryDocumentRepository.Get(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Regulatory Document : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public RegulatoryDocument Get(Expression<Func<RegulatoryDocument, bool>> predicate, bool includeDeleted = false, params Expression<Func<RegulatoryDocument, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Regulatory Document that fits predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.RegulatoryDocumentRepository.Get(predicate, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Regulatory Document : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public IQueryable<RegulatoryDocument> GetAll(bool includeDeleted = false, params Expression<Func<RegulatoryDocument, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Regulatory Document", "INFO");

            try
            {
                return uow.RegulatoryDocumentRepository.GetAll(includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Regulatory Document : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public IList<RegulatoryDocument> GetAll(bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Regulatory Document", "INFO");

            try
            {
                return uow.RegulatoryDocumentRepository.GetAll(includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Regulatory Document : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public IList<RegulatoryDocument> GetAll(Expression<Func<RegulatoryDocument, bool>> predicate, bool includeDeleted)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Regulatory Document that fit predicate '{predicate}'", "INFO");

            try
            {
                return uow.RegulatoryDocumentRepository.GetAll(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Regulatory Document : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<RegulatoryDocument>> GetAllAsync(bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Regulatory Document", "INFO");

            try
            {
                return await uow.RegulatoryDocumentRepository.GetAllAsync(includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Regulatory Document : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<RegulatoryDocument>> GetAllAsync(Expression<Func<RegulatoryDocument, bool>> predicate, bool includeDeleted)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Regulatory Document that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.RegulatoryDocumentRepository.GetAllAsync(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Regulatory Document : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<RegulatoryDocument>> GetAllAsync(Expression<Func<RegulatoryDocument, bool>> predicate, bool includeDeleted = false, params Expression<Func<RegulatoryDocument, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Regulatory Document that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.RegulatoryDocumentRepository.GetAllAsync(predicate, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Regulatory Document : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<RegulatoryDocument>> GetAllAsync(bool includeDeleted = false, params Expression<Func<RegulatoryDocument, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Get all Regulatory Document", "INFO");

            try
            {
                return await uow.RegulatoryDocumentRepository.GetAllAsync(includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Regulatory Document : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<RegulatoryDocument> GetAsync(long id, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Regulatory Document with ID '{id}'", "INFO");

            try
            {
                return await uow.RegulatoryDocumentRepository.GetAsync(id, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Regulatory Document : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<RegulatoryDocument> GetAsync(Expression<Func<RegulatoryDocument, bool>> predicate, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Regulatory Document that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.RegulatoryDocumentRepository.GetAsync(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Regulatory Document : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<RegulatoryDocument> GetAsync(Expression<Func<RegulatoryDocument, bool>> predicate, bool includeDeleted = false, params Expression<Func<RegulatoryDocument, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Regulatory Document that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.RegulatoryDocumentRepository.GetAsync(predicate, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Regulatory Document : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<RegulatoryDocument>> GetTopAsync(Expression<Func<RegulatoryDocument, bool>> predicate, int top, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get top {top} Regulatory Documents that fit predicate >> {predicate}", "INFO");

            try
            {
                return await uow.RegulatoryDocumentRepository.GetTopAsync(predicate, top, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Regulatory Document : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Insert(PolicyDocumentRequest request, string username)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map Regulatory Document request to Regulatory Document entity
                var document = Mapper.Map<PolicyDocumentRequest, RegulatoryDocument>(request);

                //..log the Regulatory Document data being saved
                var documentJson = JsonSerializer.Serialize(document, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Regulatory Document data: {documentJson}", "DEBUG");

                document.CreatedBy = username;
                document.CreatedOn = DateTime.Now;
                var added = uow.RegulatoryDocumentRepository.Insert(document);
                if (added)
                {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(document).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save submission : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> InsertAsync(PolicyDocumentRequest request, string username) {
            using var uow = UowFactory.Create();
            try {
                //..map Regulatory Document request to Regulatory Document entity
                var document = Mapper.Map<PolicyDocumentRequest, RegulatoryDocument>(request);

                var frequency = await uow.FrequencyRepository.GetAsync(f => f.Id == request.FrequencyId);
                if (frequency != null) {
                    document.Frequency = frequency;
                    document.NextRevisionDate = CalculateNextDate(document.LastRevisionDate, frequency);
                }

                //..log the Regulatory Document data being saved
                var documentJson = JsonSerializer.Serialize(document, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Regulatory Document data: {documentJson}", "DEBUG");

                document.CreatedBy = username;
                document.CreatedOn = DateTime.Now;
                var added = await uow.RegulatoryDocumentRepository.InsertAsync(document);
                if (added)
                {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(document).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save Regulatory Document : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        private static DateTime? CalculateNextDate(DateTime lastRevisionDate, Frequency frequency) {
            DateTime? dateTime = frequency.FrequencyName.Trim().ToUpper() switch {
                "DAILY" => (DateTime?)lastRevisionDate.AddDays(7),
                "WEEKLY" => (DateTime?)lastRevisionDate.AddDays(7),
                "MONTHLY" => (DateTime?)lastRevisionDate.AddMonths(1),
                "QUARTERLY" => (DateTime?)lastRevisionDate.AddMonths(3),
                "BIANNUAL" => (DateTime?)lastRevisionDate.AddMonths(6),
                "ANNUAL" => (DateTime?)lastRevisionDate.AddYears(1),
                "BIENNIAL" => (DateTime?)lastRevisionDate.AddYears(2),
                "TRIENNIAL" => (DateTime?)lastRevisionDate.AddYears(3),
                _ => null,
            };
            return dateTime;
        }

        public bool Update(PolicyDocumentRequest request, string username, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update Regulatory Document request", "INFO");

            try {
                var document = uow.RegulatoryDocumentRepository.Get(a => a.Id == request.Id);
                if (document != null) {
                    var frequency = uow.FrequencyRepository.Get(f => f.Id == request.FrequencyId);
                    if (frequency != null) {
                        document.Frequency = frequency;
                        document.NextRevisionDate = CalculateNextDate(document.LastRevisionDate, frequency);
                    }

                    //..update Regulatory Document record
                    document.DocumentName = (request.DocumentName ?? string.Empty).Trim();
                    document.Status = (request.DocumentStatus ?? string.Empty).Trim();
                    document.Approver = (request.Approver ?? string.Empty).Trim();
                    document.ApprovalDate = request.ApprovalDate;
                    document.PolicyAligned = request.IsAligned;
                    document.IsLocked = request.IsLocked;
                    document.NeedMcrApproval = request.NeedMcrApproval;
                    document.NeedBoardApproval = request.NeedBoardApproval;
                    document.OnIntranet = request.OnIntranet;
                    document.IsApproved = request.IsApproved;
                    document.LastRevisionDate = request.LastRevisionDate;
                    document.FrequencyId = request.FrequencyId;
                    document.DocumentTypeId = request.DocumentTypeId;
                    document.ResponsibilityId = request.ResponsibilityId;
                    document.Comments = (request.Comments ?? string.Empty).Trim();
                    document.SendNotification = request.SendNotification;
                    document.Interval = (request.Interval ?? string.Empty).Trim();
                    document.IntervalType = (request.IntervalType ?? string.Empty).Trim();
                    document.SentMessages = request.SentMessages;
                    document.ReminderMessage = request.ReminderMessage;
                    document.IsDeleted = request.IsDeleted;
                    document.LastModifiedOn = DateTime.Now;
                    document.LastModifiedBy = $"{username}";

                    //..check entity state
                    _ = uow.RegulatoryDocumentRepository.Update(document, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(document).State;
                    Logger.LogActivity($"Regulatory Document state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to update Regulatory Document record: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> UpdateAsync(PolicyDocumentRequest request, string username, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update Regulatory Document", "INFO");

            try {
                var document = await uow.RegulatoryDocumentRepository.GetAsync(a => a.Id == request.Id);
                if (document != null) {

                    var frequency = await uow.FrequencyRepository.GetAsync(f => f.Id == request.FrequencyId);
                    if (frequency != null) {
                        document.Frequency = frequency;
                        document.NextRevisionDate = CalculateNextDate(document.LastRevisionDate, frequency);
                    }

                    //..update Regulatory Document record
                    document.DocumentName = (request.DocumentName ?? string.Empty).Trim();
                    document.Status = (request.DocumentStatus ?? string.Empty).Trim();
                    document.Approver = (request.Approver ?? string.Empty).Trim();
                    document.PolicyAligned = request.IsAligned;
                    document.IsLocked = request.IsLocked;
                    document.OnIntranet = request.OnIntranet;
                    document.NeedMcrApproval = request.NeedMcrApproval;
                    document.NeedBoardApproval = request.NeedBoardApproval;
                    document.IsApproved = request.IsApproved;
                    document.LastRevisionDate = request.LastRevisionDate;
                    document.FrequencyId = request.FrequencyId;
                    document.NextRevisionDate = request.NextRevisionDate;
                    document.DocumentTypeId = request.DocumentTypeId;
                    document.ResponsibilityId = request.ResponsibilityId;
                    document.Comments = (request.Comments ?? string.Empty).Trim();
                    document.SendNotification = request.SendNotification;
                    document.Interval = (request.Interval ?? string.Empty).Trim();
                    document.IntervalType = (request.IntervalType ?? string.Empty).Trim();
                    document.SentMessages = request.SentMessages;
                    document.ReminderMessage = request.ReminderMessage;
                    document.IsDeleted = request.IsDeleted;
                    document.LastModifiedOn = DateTime.Now;
                    document.ApprovalDate = request.ApprovalDate;
                    document.LastModifiedBy = $"{username}";

                    //..check entity state
                    _ = await uow.RegulatoryDocumentRepository.UpdateAsync(document, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(document).State;
                    Logger.LogActivity($"Regulatory Regulatory Document state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to update Regulatory Document record: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> LockDocumentAsync(LockPolicyDocumentRequest request, string username) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Lock Regulatory Document", "INFO");

            try {
                var document = await uow.RegulatoryDocumentRepository.GetAsync(a => a.Id == request.Id);
                if (document != null) {
                    document.IsLocked = request.IsLocked;
                    document.LastModifiedOn = DateTime.Now;
                    document.LastModifiedBy = $"{username}";

                    //..check entity state
                    _ = await uow.RegulatoryDocumentRepository.UpdateAsync(document, true);
                    var entityState = ((UnitOfWork)uow).Context.Entry(document).State;
                    Logger.LogActivity($"Regulatory Document state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to update Regulatory Document record: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Delete(IdRequest request) {
            using var uow = UowFactory.Create();
            try {
                var documentJson = JsonSerializer.Serialize(request, new JsonSerializerOptions {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Regulatory Document data: {documentJson}", "DEBUG");

                var statute = uow.RegulatoryDocumentRepository.Get(t => t.Id == request.RecordId);
                if (statute != null) {
                    //..mark as delete this Regulatory Document
                    _ = uow.RegulatoryDocumentRepository.Delete(statute, request.MarkAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(statute).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete Regulatory Document : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> DeleteAsync(IdRequest request) {
            using var uow = UowFactory.Create();
            try {
                var documentJson = JsonSerializer.Serialize(request, new JsonSerializerOptions {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Regulatory Document data: {documentJson}", "DEBUG");

                var documents = await uow.RegulatoryDocumentRepository.GetAsync(t => t.Id == request.RecordId);
                if (documents != null) {
                    //..mark as delete this Regulatory Document
                    _ = await uow.RegulatoryDocumentRepository.DeleteAsync(documents, request.MarkAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(documents).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = await uow.SaveChangesAsync();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to delete Regulatory Document : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> LockAsync(IdRequest request) {
            using var uow = UowFactory.Create();
            try {
                var documentJson = JsonSerializer.Serialize(request, new JsonSerializerOptions {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Regulatory Document data: {documentJson}", "DEBUG");

                var document = await uow.RegulatoryDocumentRepository.GetAsync(t => t.Id == request.RecordId);
                if (document != null) {
                    document.IsLocked = true;

                    //..check entity state
                    var entityState = await uow.RegulatoryDocumentRepository.UpdateAsync(document, false);
                    Logger.LogActivity($"Entity state after locking document: {entityState}", "DEBUG");

                    var result = await uow.SaveChangesAsync();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to update Regulatory Document record: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> DeleteAllAsync(IList<long> requestIds, bool markAsDeleted = false)
        {
            using var uow = UowFactory.Create();
            try
            {
                var documents = await uow.RegulatoryDocumentRepository.GetAllAsync(e => requestIds.Contains(e.Id));
                if (documents.Count == 0)
                {
                    Logger.LogActivity($"Records not found", "INFO");
                    return false;
                }
                return await uow.RegulatoryDocumentRepository.DeleteAllAsync(documents, markAsDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete Regulatory Document: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> BulkyInsertAsync(RegulatoryDocumentRequest[] requestItems)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map Regulatory Document to Regulatory Document entity
                var documents = requestItems.Select(Mapper.Map<RegulatoryDocumentRequest, RegulatoryDocument>).ToArray();
                var documentJson = JsonSerializer.Serialize(documents, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Regulatory Document data: {documentJson}", "DEBUG");
                return await uow.RegulatoryDocumentRepository.BulkyInsertAsync(documents);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save Regulatory Document : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> BulkyUpdateAsync(RegulatoryDocumentRequest[] requestItems)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map Regulatory Documents request to  Regulatory Documents entity
                var documents = requestItems.Select(Mapper.Map<RegulatoryDocumentRequest, RegulatoryDocument>).ToArray();
                var returnsJson = JsonSerializer.Serialize(documents, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Regulatory Document data: {returnsJson}", "DEBUG");
                return await uow.RegulatoryDocumentRepository.BulkyUpdateAsync(documents);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save Regulatory Document : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> BulkyUpdateAsync(RegulatoryDocumentRequest[] requestItems, params Expression<Func<RegulatoryDocument, object>>[] propertySelectors)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map Regulatory Document request to Regulatory Document entity
                var documents = requestItems.Select(Mapper.Map<RegulatoryDocumentRequest, RegulatoryDocument>).ToArray();
                var documentJson = JsonSerializer.Serialize(documents, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($" Regulatory Documents data: {documentJson}", "DEBUG");
                return await uow.RegulatoryDocumentRepository.BulkyUpdateAsync(documents, propertySelectors);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save Regulatory Documents : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<RegulatoryDocument>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<RegulatoryDocument, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Regulatory Document", "INFO");

            try
            {
                return await uow.RegulatoryDocumentRepository.PageAllAsync(page, size, includeDeleted, null, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Regulatory Document: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<RegulatoryDocument>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<RegulatoryDocument, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Regulatory Document", "INFO");

            try
            {
                return await uow.RegulatoryDocumentRepository.PageAllAsync(token, page, size, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieves Regulatory Document : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<RegulatoryDocument>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<RegulatoryDocument, bool>> predicate = null)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Regulatory Document", "INFO");

            try
            {
                return await uow.RegulatoryDocumentRepository.PageAllAsync(page, size, includeDeleted, predicate);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Regulatory Document: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<RegulatoryDocument>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<RegulatoryDocument, bool>> predicate = null, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Regulatory Document", "INFO");

            try
            {
                return await uow.RegulatoryDocumentRepository.PageAllAsync(token, page, size, predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Regulatory Document : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        #endregion

        #region Policy Reports

        public async Task<PolicySummeryResponse> GetPolicySummeryAsync(bool includeDeleted) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Generate policy reports", "INFO");

            var summary = new PolicySummeryResponse() {
                Count = new(),
                Percentage = new()
            };

            try {

                //..policies
                var policies = await uow.RegulatoryDocumentRepository.GetAllAsync(includeDeleted);
                var statusGroups = policies.GroupBy(p => p.Status).ToDictionary(g => g.Key, g => g.Count());
                var total = policies.Count;

                //..nested method
                void Add(string key, int count) {
                    summary.Count[key] = count;
                    summary.Percentage[key] = decimal.Round((count * 100m) / total, 2, MidpointRounding.AwayFromZero);
                }

                Add("On Hold", statusGroups.GetValueOrDefault("ON-HOLD", 0));
                Add("Department Review", statusGroups.GetValueOrDefault("DEPT-REVIEW", 0));
                Add("Not Uptodate", statusGroups.GetValueOrDefault("DUE", 0));
                Add("Board Review", statusGroups.GetValueOrDefault("PENDING-BOARD", 0));
                Add("SMT Review", statusGroups.GetValueOrDefault("PENDING-SMT", 0));
                Add("MRC Review", statusGroups.GetValueOrDefault("PENDING-MRC", 0));
                Add("Uptodate", statusGroups.GetValueOrDefault("UPTODATE", 0));
                Add("Standard", statusGroups.GetValueOrDefault("NA", 0));

                Add("Total", total);
                summary.Percentage["Total"] = 100;

            } catch (Exception ex) {
                 Logger.LogActivity($"Failed to generate report data: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }

            return summary;
        }

        public async Task<PolicySummeryResponse> GetBodSummeryAsync(bool includeDeleted) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Generate policy reports", "INFO");

            var summary = new PolicySummeryResponse() {
                Count = new(),
                Percentage = new()
            };

            try {
                var policies = await uow.RegulatoryDocumentRepository.GetAllAsync(
                    r => r.Status == "PENDING-BOARD" || r.Approver == "SMT",
                    includeDeleted
                );

                var statusGroups = policies.GroupBy(p => p.Status).ToDictionary(g => g.Key, g => g.Count());
                var total = policies.Count;

                //..nested method
                void Add(string key, int count) {
                    summary.Count[key] = count;
                    summary.Percentage[key] = decimal.Round((count * 100m) / total, 2, MidpointRounding.AwayFromZero);
                }

                Add("On Hold", statusGroups.GetValueOrDefault("ON-HOLD", 0));
                Add("Department Review", statusGroups.GetValueOrDefault("DEPT-REVIEW", 0));
                Add("Not Uptodate", statusGroups.GetValueOrDefault("DUE", 0));
                Add("Board Review", statusGroups.GetValueOrDefault("PENDING-BOARD", 0));
                Add("SMT Review", statusGroups.GetValueOrDefault("PENDING-SMT", 0));
                Add("MRC Review", statusGroups.GetValueOrDefault("PENDING-MRC", 0));
                Add("Uptodate", statusGroups.GetValueOrDefault("UPTODATE", 0));
                Add("Standard", statusGroups.GetValueOrDefault("NA", 0));

                Add("Total", total);
                summary.Percentage["Total"] = 100;

            } catch (Exception ex) {
                Logger.LogActivity($"Failed to generate report data: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }

            return summary;
        }

        public async Task<PolicySummeryResponse> GetSmtSummeryAsync(bool includeDeleted) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Generate policy reports", "INFO");

            var summary = new PolicySummeryResponse() {
                Count = new(),
                Percentage = new()
            };

            try {
                var policies = await uow.RegulatoryDocumentRepository.GetAllAsync(
                    r => r.Status == "PENDING-SMT" || r.Approver == "SMT",
                    includeDeleted
                );

                var statusGroups = policies.GroupBy(p => p.Status).ToDictionary(g => g.Key, g => g.Count());
                var total = policies.Count;

                //..nested method
                void Add(string key, int count) {
                    summary.Count[key] = count;
                    summary.Percentage[key] = decimal.Round((count * 100m) / total, 2, MidpointRounding.AwayFromZero);
                }

                Add("On Hold", statusGroups.GetValueOrDefault("ON-HOLD", 0));
                Add("Department Review", statusGroups.GetValueOrDefault("DEPT-REVIEW", 0));
                Add("Not Uptodate", statusGroups.GetValueOrDefault("DUE", 0));
                Add("Board Review", statusGroups.GetValueOrDefault("PENDING-BOARD", 0));
                Add("SMT Review", statusGroups.GetValueOrDefault("PENDING-SMT", 0));
                Add("MRC Review", statusGroups.GetValueOrDefault("PENDING-MRC", 0));
                Add("Uptodate", statusGroups.GetValueOrDefault("UPTODATE", 0));
                Add("Standard", statusGroups.GetValueOrDefault("NA", 0));

                Add("Total", total);
                summary.Percentage["Total"] = 100;

            } catch (Exception ex) {
                Logger.LogActivity($"Failed to generate report data: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }

            return summary;
        }

        #endregion

        #region Notification Mails

        public async Task SendNotificationMailsAsync() {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Generate and send Notification mails", "INFO");

            try {
                var includeStatuses = new List<string> {
                    "DUE",
                    "PENDING-MRC",
                    "PENDING-SMT",
                    "DEPT-REVIEW",
                    "PENDING-BOARD"
                };

                var policies = await uow.RegulatoryDocumentRepository.GetAllAsync(p => p.SendNotification && 
                includeStatuses.Contains(p.Status) && 
                !p.IsApproved, false,p => p.Owner);

                var policyList = policies.ToList();
                if (policyList == null || !policyList.Any()) {
                    Logger.LogActivity("No policies found for notification", "INFO");
                    return;
                }

                //..apply interval filter (based on how many times notification has been sent)
                Logger.LogActivity($"Found {policyList.Count} policies with SendNotification=true", "INFO");
                policyList = IntervalFilter(policyList);

                if (!policyList.Any()) {
                    Logger.LogActivity("No policies remain after interval filter", "INFO");
                    return;
                }

                //..apply interval type filter (Daily/Weekly/Monthly timing)
                Logger.LogActivity($"{policyList.Count} policies remain after interval filter", "INFO");
                policyList = await IntervalTypeFilter(policyList, uow);

                if (!policyList.Any()) {
                    Logger.LogActivity("No policies remain after interval type filter", "INFO");
                    return;
                }

                Logger.LogActivity($"{policyList.Count} policies ready for notification", "INFO");

                // Filter out policies where next review date is more than 2 months away
                var twoMonthsFromNow = DateTime.UtcNow.AddMonths(2);
                policyList = policyList.Where(p => p.NextRevisionDate.HasValue &&p.NextRevisionDate.Value <= twoMonthsFromNow).ToList();
                if (!policyList.Any()) {
                    Logger.LogActivity("No policies with NextReviewDate within 2 months", "INFO");
                    return;
                }

                //..get compliance users for CC
                Logger.LogActivity($"{policyList.Count} policies have NextReviewDate within 2 months", "INFO");
                var compliance = await uow.DepartmentRepository.GetAllAsync(d => d.DepartmentCode == "COMPLIANCE", false, d => d.Users);

                string compUsers = compliance != null && compliance.Any() ?
                      string.Join(",", compliance
                            .SelectMany(d => d.Users)
                            .Select(u => u.EmailAddress)
                            .Where(e => !string.IsNullOrEmpty(e))
                            .Distinct()): string.Empty;

                //..add test users
                compUsers += $"irene.nabuloli@pearlbank.com;apollo.olinga@pearlbank.com;Moses.Semanda@pearlbank.com;mark.nkambwe@pearlbank.com";
                //..enqueue notifications for each policy
                foreach (var policy in policyList) {
                    await EnqueuePolicyNotificationAsync(policy, compUsers, uow);
                }

                Logger.LogActivity($"Enqueued {policyList.Count} policy notifications", "INFO");
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to generate policy notifications: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        #endregion

        #region Private Methods
        private static void AddMailAddresses( MailAddressCollection collection,string addresses) {
            if (string.IsNullOrWhiteSpace(addresses))
                return;

            var normalized = addresses
                .Replace(";", ",")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(e => e.Trim());

            foreach (var email in normalized) {
                collection.Add(new MailAddress(email));
            }
        }

        private async Task EnqueuePolicyNotificationAsync(RegulatoryDocument policy, string complianceUsers, IUnitOfWork uow) {

            try {

                if (policy.Owner == null) {
                    Logger.LogActivity($"Policy {policy.Id} has no owner. Skipping notification.", "WARN");
                    return;
                }

                var ownerEmail = (policy.Owner.ContactEmail ?? string.Empty).Trim();
                var ownerName = (policy.Owner.ContactName ?? string.Empty).Trim();

                if (string.IsNullOrEmpty(ownerEmail)) {
                    Logger.LogActivity($"Policy {policy.Id} owner has no email. Skipping notification.", "WARN");
                    return;
                }

                var policyTitle = policy.DocumentName ?? "Policy";
                var policyId = policy.Id;
                var nextReviewDate = policy.NextRevisionDate;

                Logger.LogActivity($"Enqueuing notification for Policy {policyId} to {ownerEmail}", "INFO");

                //..enqueue the mail task
                _mailTaskQueue.Enqueue(async (sp, token) => {
                    var mailService = sp.GetRequiredService<IMailService>();
                    var logger = sp.GetRequiredService<IServiceLoggerFactory>().CreateLogger();
                    var docUow = sp.GetRequiredService<IUnitOfWorkFactory>().Create();

                    try {
                        var mailSettings = await mailService.GetMailSettingsAsync();
                        if (mailSettings == null) {
                            logger.LogActivity($"Mail settings not found. Cannot send notification for Policy {policyId}.", "WARN");
                            return;
                        }

                        var (sent, subject, body) = (false, string.Empty, string.Empty);
                        //var (sent, subject, body) = MailHandler.SendPolicyNotificationMail(
                        //    logger,
                        //    mailSettings.MailSender,
                        //    ownerEmail,
                        //    ownerName,
                        //    complianceUsers,
                        //    policyTitle,
                        //    nextReviewDate,
                        //    mailSettings.NetworkPort,
                        //    mailSettings.SystemPassword
                        //);

                        if (sent) {
                            logger.LogActivity($"Policy notification sent to {ownerEmail} for Policy {policyId}", "INFO");

                            // Save mail record
                            await mailService.InsertMailAsync(new MailRecord {
                                SentToEmail = ownerEmail,
                                CCMail = complianceUsers,
                                Subject = subject,
                                Mail = body,
                                DocumentId = policyId, 
                                IsDeleted = false,
                                CreatedBy = "SYSTEM",
                                CreatedOn = DateTime.Now,
                                LastModifiedBy = "SYSTEM",
                                LastModifiedOn = DateTime.Now,
                            });

                            //..increment SentMessages count
                            var policyToUpdate = await docUow.RegulatoryDocumentRepository
                                .GetAsync(p => p.Id == policyId, false);

                            if (policyToUpdate != null) {
                                policyToUpdate.SentMessages = policyToUpdate.SentMessages + 1;
                                policyToUpdate.LastModifiedBy = "SYSTEM";
                                policyToUpdate.LastModifiedOn = DateTime.Now;

                                await docUow.RegulatoryDocumentRepository.UpdateAsync(policyToUpdate);
                                await docUow.SaveChangesAsync();

                                logger.LogActivity($"Updated SentMessages count to {policyToUpdate.SentMessages} for Policy {policyId}", "INFO");
                            }
                        }
                    } catch (Exception ex) {
                        logger.LogActivity($"Exception sending policy notification for Policy {policyId}: {ex.Message}", "ERROR");
                    } finally {
                        docUow?.Dispose();
                    }
                });

            } catch (Exception ex) {
                Logger.LogActivity($"Error enqueuing notification for Policy {policy.Id}: {ex.Message}", "ERROR");
            }
        }
        private static List<RegulatoryDocument> IntervalFilter(List<RegulatoryDocument> documents) {
            static int GetIntervalCount(string interval) {
                return interval?.Trim().ToUpper() switch {
                    "ONCE" => 1,
                    "TWICE" => 2,
                    "THREE" => 3,
                    _ => 0
                };
            }

            // Keep only documents where sent messages is less than the interval count
            return documents.Where(d => {
                int intervalCount = GetIntervalCount(d.Interval);
                int sentCount = d.SentMessages;

                // If interval is 0 (invalid/unlimited), always include
                if (intervalCount == 0)
                    return true;

                // Otherwise, only include if we haven't reached the limit
                return sentCount < intervalCount;
            }).ToList();
        }

        private static async Task<List<RegulatoryDocument>> IntervalTypeFilter(List<RegulatoryDocument> documents, IUnitOfWork uow) {

            var result = new List<RegulatoryDocument>();
            var now = DateTime.UtcNow;

            foreach (var doc in documents) {
                var intervalType = (doc.IntervalType ?? string.Empty).Trim().ToUpper();

                // Get the last notification date for this document
                var lastNotification = await uow.MailRecordRepository.GetLastNotificationDateAsync(doc.Id, "Policy");
                bool shouldSend = intervalType switch {
                    "DAILY" => ShouldSendDaily(lastNotification, now),
                    "WEEKLY" => ShouldSendWeekly(lastNotification, now),
                    "MONTHLY" => ShouldSendMonthly(lastNotification, now),
                    _ => true 
                };

                if (shouldSend) {
                    result.Add(doc);
                }
            }

            return result;
        }

        private static bool ShouldSendDaily(DateTime? lastNotification, DateTime now) {
            if (!lastNotification.HasValue)
                return true; // Never sent before

            // Check if last notification was on a different day
            return lastNotification.Value.Date < now.Date;
        }

        private static bool ShouldSendWeekly(DateTime? lastNotification, DateTime now) {
            if (!lastNotification.HasValue)
                return true;

            // Week is Monday to Saturday
            // Get the start of current week (Monday)
            int daysUntilMonday = ((int)now.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            DateTime currentWeekStart = now.Date.AddDays(-daysUntilMonday);

            // Get the start of last notification's week
            int lastDaysUntilMonday = ((int)lastNotification.Value.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            DateTime lastWeekStart = lastNotification.Value.Date.AddDays(-lastDaysUntilMonday);

            // Send if we're in a different week
            return currentWeekStart > lastWeekStart;
        }

        private static bool ShouldSendMonthly(DateTime? lastNotification, DateTime now) {
            if (!lastNotification.HasValue)
                return true;

            // Month is from 1st to end of month
            // Send if we're in a different month
            return lastNotification.Value.Year != now.Year ||
                   lastNotification.Value.Month != now.Month;
        }
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
                ErrorSource = "REGULATORY-DOCUMENT-SERVICE",
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

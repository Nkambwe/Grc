using AutoMapper;
using Azure.Core;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Compliance.Regulations;
using Grc.Middleware.Api.Data.Entities.Support;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Utils;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Services.Compliance.Regulations {

    public class RegulatoryDocumentService : BaseService, IRegulatoryDocumentService {

        public RegulatoryDocumentService(IServiceLoggerFactory loggerFactory, IUnitOfWorkFactory uowFactory, IMapper mapper)
            : base(loggerFactory, uowFactory, mapper) {
        }

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
                    document.ApprovedBy = (request.ApprovedBy ?? string.Empty).Trim();
                    document.ApprovalDate = request.ApprovalDate;
                    document.PolicyAligned = request.IsAligned;
                    document.IsLocked = request.IsLocked;
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
                    document.ApprovedBy = (request.ApprovedBy ?? string.Empty).Trim();
                    document.PolicyAligned = request.IsAligned;
                    document.IsLocked = request.IsLocked;
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

        public bool Delete(IdRequest request)
        {
            using var uow = UowFactory.Create();
            try
            {
                var documentJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Regulatory Document data: {documentJson}", "DEBUG");

                var statute = uow.RegulatoryDocumentRepository.Get(t => t.Id == request.RecordId);
                if (statute != null)
                {
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

        public async Task<bool> DeleteAsync(IdRequest request)
        {
            using var uow = UowFactory.Create();
            try
            {
                var documentJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Regulatory Document data: {documentJson}", "DEBUG");

                var documents = await uow.RegulatoryDocumentRepository.GetAsync(t => t.Id == request.RecordId);
                if (documents != null)
                {
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
            }
            catch (Exception ex)
            {
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

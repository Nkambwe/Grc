using AutoMapper;
using Azure.Core;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Compliance.Returns;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.TaskHandler;
using Grc.Middleware.Api.Utils;
using RTools_NTS.Util;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Services.Compliance.Regulations {

    public class ReturnsSubmissionService : BaseService, IReturnsSubmissionService {

        private readonly IMailTaskQueue _mailTaskQueue;
        public ReturnsSubmissionService(IServiceLoggerFactory loggerFactory, 
                                         IUnitOfWorkFactory uowFactory,
                                         IMailTaskQueue mailTaskQueue,
                                         IMapper mapper) : base(loggerFactory, uowFactory, mapper) {
            _mailTaskQueue = mailTaskQueue;
        }

        #region Queries

        public int Count() {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Submission in the database", "INFO");

            try {
                return uow.RegulatorySubmissionRepository.Count();
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to Submission in the database: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public int Count(Expression<Func<ReturnSubmission, bool>> predicate)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Submission in the database", "INFO");

            try
            {
                return uow.RegulatorySubmissionRepository.Count(predicate);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count submission in the database: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Submission in the database", "INFO");

            try
            {
                return await uow.RegulatorySubmissionRepository.CountAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of submissions in the database", "INFO");

            try
            {
                return await uow.StatutoryArticleRepository.CountAsync(excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Submission in the database: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<int> CountAsync(Expression<Func<ReturnSubmission, bool>> predicate, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Submission in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.RegulatorySubmissionRepository.CountAsync(predicate, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count submission in the database: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<int> CountAsync(Expression<Func<ReturnSubmission, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Submission in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.RegulatorySubmissionRepository.CountAsync(predicate, excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count submission in the database: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Exists(Expression<Func<ReturnSubmission, bool>> predicate, bool excludeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an submission exists in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.RegulatorySubmissionRepository.Exists(predicate, excludeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for submission in the database: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<ReturnSubmission, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an submission exists in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.RegulatorySubmissionRepository.ExistsAsync(predicate, excludeDeleted, token);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for submission in the database: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<ReturnSubmission, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check for batch Submission if they exist in the database that fit predicate >> '{predicates}'", "INFO");

            try
            {
                return await uow.RegulatorySubmissionRepository.ExistsBatchAsync(predicates, excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for Submission in the database: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public ReturnSubmission Get(long id, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get submission with ID '{id}'", "INFO");

            try
            {
                return uow.RegulatorySubmissionRepository.Get(id, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve submission: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public ReturnSubmission Get(Expression<Func<ReturnSubmission, bool>> predicate, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get submission that fits predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.RegulatorySubmissionRepository.Get(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve submission : {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public ReturnSubmission Get(Expression<Func<ReturnSubmission, bool>> predicate, bool includeDeleted = false, params Expression<Func<ReturnSubmission, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get submission that fits predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.RegulatorySubmissionRepository.Get(predicate, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve submission : {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public IQueryable<ReturnSubmission> GetAll(bool includeDeleted = false, params Expression<Func<ReturnSubmission, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Submission", "INFO");

            try
            {
                return uow.RegulatorySubmissionRepository.GetAll(includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Submission : {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public IList<ReturnSubmission> GetAll(bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Submission", "INFO");

            try
            {
                return uow.RegulatorySubmissionRepository.GetAll(includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Submission : {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public IList<ReturnSubmission> GetAll(Expression<Func<ReturnSubmission, bool>> predicate, bool includeDeleted)
        {
            using var uow = UowFactory.Create();

            try
            {
                return uow.RegulatorySubmissionRepository.GetAll(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve submission : {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<ReturnSubmission>> GetAllAsync(bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            try
            {
                return await uow.RegulatorySubmissionRepository.GetAllAsync(includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Submission : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<ReturnSubmission>> GetAllAsync(Expression<Func<ReturnSubmission, bool>> predicate, bool includeDeleted) {
            using var uow = UowFactory.Create();
            try
            {
                return await uow.RegulatorySubmissionRepository.GetAllAsync(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Submission : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<ReturnSubmission>> GetAllAsync(Expression<Func<ReturnSubmission, bool>> predicate, bool includeDeleted = false, params Expression<Func<ReturnSubmission, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Submission that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.RegulatorySubmissionRepository.GetAllAsync(predicate, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Submission : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<ReturnSubmission>> GetAllAsync(bool includeDeleted = false, params Expression<Func<ReturnSubmission, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Get all Submission", "INFO");

            try
            {
                return await uow.RegulatorySubmissionRepository.GetAllAsync(includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Submission : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<ReturnSubmission> GetAsync(long id, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get submission with ID '{id}'", "INFO");

            try
            {
                return await uow.RegulatorySubmissionRepository.GetAsync(id, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve submission : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<ReturnSubmission> GetAsync(Expression<Func<ReturnSubmission, bool>> predicate, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get submission that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.RegulatorySubmissionRepository.GetAsync(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve submission : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<ReturnSubmission> GetAsync(Expression<Func<ReturnSubmission, bool>> predicate, bool includeDeleted = false, params Expression<Func<ReturnSubmission, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get submission that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.RegulatorySubmissionRepository.GetAsync(predicate, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve submission : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<ReturnSubmission>> GetTopAsync(Expression<Func<ReturnSubmission, bool>> predicate, int top, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get top {top} Submission that fit predicate >> {predicate}", "INFO");

            try
            {
                return await uow.RegulatorySubmissionRepository.GetTopAsync(predicate, top, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve submission : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Insert(ReturnSubmissionRequest request)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map submission request to submission entity
                var article = Mapper.Map<ReturnSubmissionRequest, ReturnSubmission>(request);

                //..log the submission data being saved
                var articleJson = JsonSerializer.Serialize(article, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Submission data: {articleJson}", "DEBUG");

                var added = uow.RegulatorySubmissionRepository.Insert(article);
                if (added)
                {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(article).State;
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
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> InsertAsync(ReturnSubmissionRequest request)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map submission request to submission entity
                var article = Mapper.Map<ReturnSubmissionRequest, ReturnSubmission>(request);

                //..log the submission data being saved
                var articleJson = JsonSerializer.Serialize(article, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Submission data: {articleJson}", "DEBUG");

                var added = await uow.RegulatorySubmissionRepository.InsertAsync(article);
                if (added)
                {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(article).State;
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
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Update(ReturnSubmissionRequest request, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update submission request", "INFO");

            try
            {
                var submission = uow.RegulatorySubmissionRepository.Get(a => a.Id == request.Id);
                if (submission != null)
                {
                    //..update submission record
                    submission.Status = (request.Status ?? string.Empty).Trim();
                    submission.FilePath = (request.FilePath ?? string.Empty).Trim();
                    submission.SubmittedBy = (request.SubmittedBy ?? string.Empty).Trim();
                    submission.Comments = (request.Comments ?? string.Empty).Trim();
                    submission.ReturnId = request.ReturnId;
                    submission.Deadline = request.Deadline;
                    submission.SubmissionDate = request.SubmissionDate;
                    submission.IsDeleted = request.IsDeleted;
                    submission.LastModifiedOn = DateTime.Now;
                    submission.LastModifiedBy = $"{request.UserId}";

                    //..check entity state
                    _ = uow.RegulatorySubmissionRepository.Update(submission, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(submission).State;
                    Logger.LogActivity($"Submission state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to update submission record: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> UpdateAsync(ReturnSubmissionRequest request, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update submission", "INFO");

            try
            {
                var submission = await uow.RegulatorySubmissionRepository.GetAsync(a => a.Id == request.Id);
                if (submission != null)
                {
                    submission.Status = (request.Status ?? string.Empty).Trim();
                    submission.FilePath = (request.FilePath ?? string.Empty).Trim();
                    submission.SubmittedBy = (request.SubmittedBy ?? string.Empty).Trim();
                    submission.Comments = (request.Comments ?? string.Empty).Trim();
                    submission.ReturnId = request.ReturnId;
                    submission.Deadline = request.Deadline;
                    submission.SubmissionDate = request.SubmissionDate;
                    submission.IsDeleted = request.IsDeleted;
                    submission.LastModifiedOn = DateTime.Now;
                    submission.LastModifiedBy = $"{request.UserId}";

                    //..check entity state
                    _ = await uow.RegulatorySubmissionRepository.UpdateAsync(submission, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(submission).State;
                    Logger.LogActivity($"Submission state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to update submission record: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Delete(IdRequest request)
        {
            using var uow = UowFactory.Create();
            try
            {
                var auditJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Submission data: {auditJson}", "DEBUG");

                var statute = uow.RegulatorySubmissionRepository.Get(t => t.Id == request.RecordId);
                if (statute != null)
                {
                    //..mark as delete this submission
                    _ = uow.RegulatorySubmissionRepository.Delete(statute, request.MarkAsDeleted);

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
                Logger.LogActivity($"Failed to delete Submission : {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> DeleteAsync(IdRequest request)
        {
            using var uow = UowFactory.Create();
            try
            {
                var statuteJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Submission data: {statuteJson}", "DEBUG");

                var tasktask = await uow.RegulatorySubmissionRepository.GetAsync(t => t.Id == request.RecordId);
                if (tasktask != null)
                {
                    //..mark as delete this submission
                    _ = await uow.RegulatorySubmissionRepository.DeleteAsync(tasktask, request.MarkAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(tasktask).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = await uow.SaveChangesAsync();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete submission : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> DeleteAllAsync(IList<long> requestIds, bool markAsDeleted = false)
        {
            using var uow = UowFactory.Create();
            try
            {
                var statutes = await uow.RegulatorySubmissionRepository.GetAllAsync(e => requestIds.Contains(e.Id));
                if (statutes.Count == 0)
                {
                    Logger.LogActivity($"Records not found", "INFO");
                    return false;
                }
                return await uow.RegulatorySubmissionRepository.DeleteAllAsync(statutes, markAsDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete Submission: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> BulkyInsertAsync(ReturnSubmissionRequest[] requestItems)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map submission to submission entity
                var submission = requestItems.Select(Mapper.Map<ReturnSubmissionRequest, ReturnSubmission>).ToArray();
                var submissionJson = JsonSerializer.Serialize(submission, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Submission record count: {requestItems.Length}", "DEBUG");
                return await uow.RegulatorySubmissionRepository.BulkyInsertAsync(submission);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save submission : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> BulkyUpdateAsync(ReturnSubmissionRequest[] requestItems)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map submissions request to submissions entity
                var submission = requestItems.Select(Mapper.Map<ReturnSubmissionRequest, ReturnSubmission>).ToArray();
                var susbmissionJson = JsonSerializer.Serialize(submission, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Submission record count: {requestItems.Length}", "DEBUG");
                return await uow.RegulatorySubmissionRepository.BulkyUpdateAsync(submission);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save submission : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> BulkyUpdateAsync(ReturnSubmissionRequest[] requestItems, params Expression<Func<ReturnSubmission, object>>[] propertySelectors)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map Submission request to Submission entity
                var submissions = requestItems.Select(Mapper.Map<ReturnSubmissionRequest, ReturnSubmission>).ToArray();
                var submissionJson = JsonSerializer.Serialize(submissions, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Submission record count: {requestItems.Length}", "DEBUG");
                return await uow.RegulatorySubmissionRepository.BulkyUpdateAsync(submissions, propertySelectors);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save Submissions : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<ReturnSubmission>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<ReturnSubmission, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Submission", "INFO");

            try
            {
                return await uow.RegulatorySubmissionRepository.PageAllAsync(page, size, includeDeleted, null, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Submission: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<ReturnSubmission>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<ReturnSubmission, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Submission", "INFO");

            try
            {
                return await uow.RegulatorySubmissionRepository.PageAllAsync(token, page, size, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieves Submission : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<ReturnSubmission>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<ReturnSubmission, bool>> predicate = null)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Submission", "INFO");

            try
            {
                return await uow.RegulatorySubmissionRepository.PageAllAsync(page, size, includeDeleted, predicate);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Submission: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<ReturnSubmission>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<ReturnSubmission, bool>> predicate = null, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Submission", "INFO");

            try {
                return await uow.RegulatorySubmissionRepository.PageAllAsync(token, page, size, predicate, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Submission : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<ReturnSubmission>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<ReturnSubmission, bool>> predicate = null, params Expression<Func<ReturnSubmission, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Submission", "INFO");

            try {
                return await uow.RegulatorySubmissionRepository.PageAllAsync(page, size, includeDeleted,  predicate, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Submission : {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        #endregion

        #region Background Tasks

        public async Task<bool> UpdateAsync(SubmissionRequest request, string username) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update submission", "INFO");

            try {
                var record = await uow.RegulatorySubmissionRepository.GetAsync(a => a.Id == request.Id);
                if (record != null) {
                    record.Status = (request.Status ?? string.Empty).Trim();
                    record.FilePath = (request.File ?? string.Empty).Trim();
                    record.SubmittedBy = (request.SubmittedBy ?? string.Empty).Trim();
                    record.Comments = (request.Comments ?? string.Empty).Trim();
                    record.SubmissionDate = request.SubmittedOn;
                    record.IsBreached = !string.IsNullOrWhiteSpace(request.BreachReason);
                    record.BreachReason = (request.BreachReason ?? string.Empty).Trim();
                    record.LastModifiedOn = DateTime.Now;
                    record.LastModifiedBy = $"{username}";

                    //..check entity state
                    _ = await uow.RegulatorySubmissionRepository.UpdateAsync(record, true);
                    var entityState = ((UnitOfWork)uow).Context.Entry(record).State;
                    Logger.LogActivity($"Submission state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to update submission record: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task GenerateMissingSubmissionsAsync(DateTime today, CancellationToken ct) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Generating submissions for {today:yyyy-MM-dd}", "INFO");

            var excludeFrequencies = new[] { "NA", "PERIODIC", "ONE-OFF", "ON OCCURRENCE" };

            try {
                var reports = await uow.ReturnRepository.GetAllAsync(
                    r => r.Frequency != null,
                    false,
                    r => r.Frequency,
                    r => r.Owner);

                var newSubmissions = new List<ReturnSubmissionRequest>();
                var breachedUpdates = new List<ReturnSubmissionRequest>();

                foreach (var report in reports) {
                    if (excludeFrequencies.Contains(report.Frequency.FrequencyName))
                        continue;

                    //..daily frequency handling
                    var (start, end) = FrequencyPeriodCalculator.GetCurrentPeriod(report.Frequency.FrequencyName, today, report.ReturnName);

                    //..skip invalid periods; Sunday for daily, Saturday for non-NPS daily
                    if (start == DateTime.MinValue || end == DateTime.MinValue) {
                        Logger.LogActivity($"Skipping {report.ReturnName} - not a valid day for this frequency", "DEBUG");
                        continue;
                    }

                    //..calculate the actual deadline based on RequiredSubmissionDay
                    var deadline = DeadlineCalculator.CalculateDeadline(report.Frequency.FrequencyName, start, end, report.RequiredSubmissionDay);

                    // Check if submission already exists for this period
                    if (!await ExistsAsync(r => r.ReturnId == report.Id && r.PeriodStart == start && r.PeriodEnd == end, false, ct)) {

                        newSubmissions.Add(new ReturnSubmissionRequest {
                            ReturnId = report.Id,
                            Status = "OPEN",
                            PeriodStart = start,
                            IsBreached = false,
                            PeriodEnd = end,
                            Deadline = deadline, 
                            SubmissionDate = null,
                            CreatedBy = "SYSTEM",
                            CreatedOn = DateTime.Now,
                            ModifiedBy = "SYSTEM",
                            ModifiedOn = DateTime.Now,
                            Owner = report.ReturnName
                        });

                        Logger.LogActivity($"New submission created: {report.ReturnName} | Period: {start:yyyy-MM-dd} to {end:yyyy-MM-dd} | Deadline: {deadline:yyyy-MM-dd}", "INFO");
                    }

                    //..find overdue submissions, using Deadline, not PeriodEnd
                    var overdue = await GetAllAsync(r => r.ReturnId == report.Id && r.Status == "OPEN" && r.SubmissionDate == null && r.Deadline < today && !r.IsBreached, false);
                    foreach (var sub in overdue) {
                        breachedUpdates.Add(new ReturnSubmissionRequest {
                            Id = sub.Id,
                            ReturnId = report.Id,
                            PeriodStart = sub.PeriodStart,
                            PeriodEnd = sub.PeriodEnd,
                            Deadline = sub.Deadline,
                            IsBreached = true,
                            Status = sub.Status,
                            SubmissionDate = sub.SubmissionDate,
                            CreatedBy = sub.CreatedBy,
                            CreatedOn = sub.CreatedOn,
                            ModifiedBy = "SYSTEM",
                            ModifiedOn = DateTime.Now,
                        });

                        Logger.LogActivity($"Submission breached: {report.ReturnName} | ID: {sub.Id} | Deadline was: {sub.Deadline:yyyy-MM-dd}", "WARN");
                    }
                }

                //..bulk operations
                if (newSubmissions.Any()) {
                    await BulkyInsertAsync(newSubmissions.ToArray());
                    Logger.LogActivity($"Inserted {newSubmissions.Count} new submissions", "INFO");
                }

                if (breachedUpdates.Any()) {
                    await BulkyUpdateAsync(breachedUpdates.ToArray());
                    Logger.LogActivity($"Updated {breachedUpdates.Count} breached submissions", "INFO");
                }

                //..enqueue mail notifications
                if (newSubmissions.Any()) {
                    await EnqueueSubmissionNotificationMailsAsync(newSubmissions, uow);
                }

                if (breachedUpdates.Any()) {
                    await EnqueueBreachNotificationMailsAsync(breachedUpdates, uow);
                }

            } catch (Exception ex) {
                Logger.LogActivity($"Failed to generate submissions: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        private async Task EnqueueSubmissionNotificationMailsAsync(List<ReturnSubmissionRequest> submissions, IUnitOfWork uow) {

            // Group by ReturnId to avoid sending duplicate emails
            var submissionsByReturn = submissions.GroupBy(s => s.ReturnId);

            foreach (var group in submissionsByReturn) {
                var returnId = group.Key;

                try {
                    // Get return with owner details
                    var returnDoc = await uow.ReturnRepository.GetAsync(
                        r => r.Id == returnId,
                        false,
                        r => r.Owner);

                    if (returnDoc?.Owner == null) {
                        Logger.LogActivity($"Return {returnId} has no owner. Skipping notification.", "WARN");
                        continue;
                    }

                    //..should we send reminders for this return?
                    if (!returnDoc.SendReminder) {
                        Logger.LogActivity($"Return {returnId} has SendReminder=false. Skipping notification.", "INFO");
                        continue;
                    }

                    var ownerEmail = (returnDoc.Owner.ContactEmail ?? string.Empty).Trim();
                    var ownerName = (returnDoc.Owner.ContactName ?? string.Empty).Trim();

                    if (string.IsNullOrEmpty(ownerEmail)) {
                        Logger.LogActivity($"Return {returnId} owner has no email. Skipping notification.", "WARN");
                        continue;
                    }

                    //..apply interval filter [ONCE, TWICE, THREE]
                    if (!ShouldSendReturnNotification(returnDoc, group.ToList())) {
                        Logger.LogActivity(
                            $"Return {returnId} has reached notification limit. Interval: {returnDoc.Interval}, Sent: {returnDoc.SentMessages}",
                            "INFO");
                        continue;
                    }

                    //..apply interval type filter [DAILY, WEEKLY, MONTHLY]
                    var lastNotificationDate = await uow.MailRecordRepository.GetLastNotificationDateAsync(returnId, "Return");
                    if (!ShouldSendBasedOnIntervalType(returnDoc.IntervalType, lastNotificationDate)) {
                        Logger.LogActivity(
                            $"Return {returnId} notification skipped based on IntervalType: {returnDoc.IntervalType}. Last sent: {lastNotificationDate:yyyy-MM-dd}",
                            "INFO");
                        continue;
                    }

                    // Get compliance users for CC
                    var compliance = await uow.DepartmentRepository.GetAllAsync(
                        d => d.DepartmentCode == "COMPLIANCE",
                        false,
                        d => d.Users);

                    string compUsers = compliance != null && compliance.Any()
                        ? string.Join(";", compliance
                            .SelectMany(d => d.Users)
                            .Select(u => u.EmailAddress)
                            .Where(e => !string.IsNullOrEmpty(e))
                            .Distinct())
                        : string.Empty;

                    // Test mails (remove in production)
                    compUsers += ";irene.nabuloli@pearlbank.com;apollo.olinga@pearlbank.com;Moses.Semanda@pearlbank.com;mark.nkambwe@pearlbank.com";

                    var returnTitle = returnDoc.ReturnName ?? "Return Submission";
                    var submissionCount = group.Count();

                    Logger.LogActivity(
                        $"Enqueuing notification for Return {returnId} to {ownerEmail}",
                        "INFO");

                    // Get the first submission ID for tracking
                    var submissionId = group.First().Id;

                    // Enqueue the mail task
                    _mailTaskQueue.Enqueue(async (sp, token) => {
                        var mailService = sp.GetRequiredService<IMailService>();
                        var logger = sp.GetRequiredService<IServiceLoggerFactory>().CreateLogger();
                        var returnUow = sp.GetRequiredService<IUnitOfWorkFactory>().Create();

                        try {
                            var mailSettings = await mailService.GetMailSettingsAsync();
                            if (mailSettings == null) {
                                logger.LogActivity(
                                    $"Mail settings not found. Cannot send notification for Return {returnId}.",
                                    "WARN");
                                return;
                            }

                            var (sent, subject, body) = MailHandler.SendSubmissionMail(
                                logger,
                                mailSettings.MailSender,
                                ownerEmail,
                                ownerName,
                                compUsers,
                                "RETURN REPORT SUBMISSION",
                                returnTitle,
                                mailSettings.NetworkPort,
                                mailSettings.SystemPassword
                            );

                            if (sent) {
                                logger.LogActivity(
                                    $"Submission notification sent to {ownerEmail} for Return {returnId}",
                                    "INFO");

                                // Save mail record to database
                                await mailService.InsertMailAsync(new MailRecord {
                                    ReturnId = returnId,
                                    SubmissionId = submissionId,
                                    SentToEmail = ownerEmail,
                                    CCMail = compUsers,
                                    Subject = subject,
                                    Mail = body,
                                    IsDeleted = false,
                                    CreatedBy = "SYSTEM",
                                    CreatedOn = DateTime.Now,
                                    LastModifiedBy = "SYSTEM",
                                    LastModifiedOn = DateTime.Now,
                                });

                                //..update SentMessages count
                                var returnToUpdate = await returnUow.ReturnRepository.GetAsync(r => r.Id == returnId, false);
                                if (returnToUpdate != null) {
                                    returnToUpdate.SentMessages = returnToUpdate.SentMessages + 1;
                                    returnToUpdate.LastModifiedBy = "SYSTEM";
                                    returnToUpdate.LastModifiedOn = DateTime.Now;

                                    await returnUow.ReturnRepository.UpdateAsync(returnToUpdate);
                                    await returnUow.SaveChangesAsync();

                                    logger.LogActivity(
                                        $"Updated SentMessages count to {returnToUpdate.SentMessages} for Return {returnId}",
                                        "INFO");
                                }

                                logger.LogActivity($"Mail record saved for Return {returnId}", "INFO");
                            } else {
                                logger.LogActivity(
                                    $"Failed to send notification for Return {returnId}",
                                    "ERROR");
                            }
                        } catch (Exception ex) {
                            logger.LogActivity(
                                $"Exception sending notification for Return {returnId}: {ex.Message}",
                                "ERROR");
                        } finally {
                            returnUow?.Dispose();
                        }
                    });
                } catch (Exception ex) {
                    Logger.LogActivity(
                        $"Error enqueuing notification for Return {returnId}: {ex.Message}",
                        "ERROR");
                }
            }
        }

        private async Task EnqueueBreachNotificationMailsAsync(List<ReturnSubmissionRequest> breachedSubmissions, IUnitOfWork uow) {

            var submissionsByReturn = breachedSubmissions.GroupBy(s => s.ReturnId);

            foreach (var group in submissionsByReturn) {
                var returnId = group.Key;

                try {
                    var returnDoc = await uow.ReturnRepository.GetAsync(
                        r => r.Id == returnId,
                        false,
                        r => r.Owner);

                    if (returnDoc?.Owner == null) {
                        Logger.LogActivity($"Return {returnId} has no owner. Skipping breach notification.", "WARN");
                        continue;
                    }

                    //..should we send reminders for this return?
                    if (!returnDoc.SendReminder) {
                        Logger.LogActivity($"Return {returnId} has SendReminder=false. Skipping breach notification.", "INFO");
                        continue;
                    }

                    var ownerEmail = (returnDoc.Owner.ContactEmail ?? string.Empty).Trim();
                    var ownerName = (returnDoc.Owner.ContactName ?? string.Empty).Trim();

                    if (string.IsNullOrEmpty(ownerEmail)) {
                        Logger.LogActivity($"Return {returnId} owner has no email. Skipping breach notification.", "WARN");
                        continue;
                    }

                    //..apply interval filter
                    if (!ShouldSendReturnNotification(returnDoc, group.ToList())) {
                        Logger.LogActivity(
                            $"Return {returnId} breach notification skipped - reached notification limit",
                            "INFO");
                        continue;
                    }

                    //..apply interval type filter
                    var lastNotificationDate = await uow.MailRecordRepository.GetLastNotificationDateAsync(returnId, "Return");
                    if (!ShouldSendBasedOnIntervalType(returnDoc.IntervalType, lastNotificationDate)) {
                        Logger.LogActivity(
                            $"Return {returnId} breach notification skipped based on IntervalType: {returnDoc.IntervalType}",
                            "INFO");
                        continue;
                    }

                    // Add compliance users
                    var compliance = await uow.DepartmentRepository.GetAllAsync(
                        d => d.DepartmentCode == "COMPLIANCE",
                        false,
                        d => d.Users);

                    string compUsers = compliance != null && compliance.Any()
                        ? string.Join(";", compliance
                            .SelectMany(d => d.Users)
                            .Select(u => u.EmailAddress)
                            .Where(e => !string.IsNullOrEmpty(e))
                            .Distinct())
                        : string.Empty;

                    // Test mails
                    compUsers += ";irene.nabuloli@pearlbank.com;apollo.olinga@pearlbank.com;Moses.Semanda@pearlbank.com;mark.nkambwe@pearlbank.com";

                    var returnTitle = returnDoc.ReturnName ?? "Return Submission";
                    Logger.LogActivity(
                        $"Enqueuing BREACH notification for Return {returnId} to {ownerEmail}",
                        "INFO");

                    var submissionId = group.First().Id;

                    // Enqueue breach notification
                    _mailTaskQueue.Enqueue(async (sp, token) => {
                        var mailService = sp.GetRequiredService<IMailService>();
                        var logger = sp.GetRequiredService<IServiceLoggerFactory>().CreateLogger();
                        var returnUow = sp.GetRequiredService<IUnitOfWorkFactory>().Create();

                        try {
                            var mailSettings = await mailService.GetMailSettingsAsync();
                            if (mailSettings == null) {
                                logger.LogActivity(
                                    $"Mail settings not found. Cannot send breach notification for Return '{returnTitle}'.",
                                    "WARN");
                                return;
                            }

                            var (sent, subject, body) = MailHandler.SendSubmissionMail(
                                logger,
                                mailSettings.MailSender,
                                ownerEmail,
                                ownerName,
                                compUsers,
                                "COMPLIANCE RETURN REPORT - OVERDUE",
                                $"OVERDUE: {returnTitle}",
                                mailSettings.NetworkPort,
                                mailSettings.SystemPassword
                            );

                            if (sent) {
                                logger.LogActivity(
                                    $"Breach notification sent to {ownerEmail} for Return '{returnTitle}'",
                                    "INFO");

                                await mailService.InsertMailAsync(new MailRecord {
                                    ReturnId = returnId,
                                    SubmissionId = submissionId,
                                    SentToEmail = ownerEmail,
                                    CCMail = compUsers,
                                    Subject = subject,
                                    Mail = body,
                                    IsDeleted = false,
                                    CreatedBy = "SYSTEM",
                                    CreatedOn = DateTime.Now,
                                    LastModifiedBy = "SYSTEM",
                                    LastModifiedOn = DateTime.Now,
                                });

                                //..update SentMessages count
                                var returnToUpdate = await returnUow.ReturnRepository.GetAsync(r => r.Id == returnId, false);
                                if (returnToUpdate != null) {
                                    returnToUpdate.SentMessages = returnToUpdate.SentMessages + 1;
                                    returnToUpdate.LastModifiedBy = "SYSTEM";
                                    returnToUpdate.LastModifiedOn = DateTime.Now;

                                    await returnUow.ReturnRepository.UpdateAsync(returnToUpdate);
                                    await returnUow.SaveChangesAsync();

                                    logger.LogActivity(
                                        $"Updated SentMessages count to {returnToUpdate.SentMessages} for Return {returnId}",
                                        "INFO");
                                }
                            } else {
                                logger.LogActivity(
                                    $"Failed to send breach notification for Return '{returnTitle}'",
                                    "ERROR");
                            }
                        } catch (Exception ex) {
                            logger.LogActivity(
                                $"Exception sending breach notification for Return '{returnTitle}': {ex.Message}",
                                "ERROR");
                        } finally {
                            returnUow?.Dispose();
                        }
                    });
                } catch (Exception ex) {
                    Logger.LogActivity(
                        $"Error enqueuing breach notification for Return {returnId}: {ex.Message}",
                        "ERROR");
                }
            }
        }

        private static bool ShouldSendReturnNotification(ReturnReport returnDoc, List<ReturnSubmissionRequest> submissions) {
            // Get interval count
            int intervalCount = (returnDoc.Interval ?? string.Empty).Trim().ToUpper() switch {
                "ONCE" => 1,
                "TWICE" => 2,
                "THREE" => 3,
                _ => int.MaxValue // No limit if interval is not specified or invalid
            };

            int sentCount = returnDoc.SentMessages;

            // Check if we've reached the notification limit
            return sentCount < intervalCount;
        }

        private static bool ShouldSendBasedOnIntervalType(string intervalType, DateTime? lastNotification) {
            var now = DateTime.UtcNow;
            var type = (intervalType ?? string.Empty).Trim().ToUpper();

            return type switch {
                "DAILY" => ShouldSendDaily(lastNotification, now),
                "WEEKLY" => ShouldSendWeekly(lastNotification, now),
                "MONTHLY" => ShouldSendMonthly(lastNotification, now),
                _ => true // If no interval type specified, always send
            };
        }

        private static bool ShouldSendDaily(DateTime? lastNotification, DateTime now) {
            if (!lastNotification.HasValue)
                return true;

            // Check if last notification was on a different day
            return lastNotification.Value.Date < now.Date;
        }

        private static bool ShouldSendWeekly(DateTime? lastNotification, DateTime now) {
            if (!lastNotification.HasValue)
                return true;

            // Week is Monday to Saturday
            int daysUntilMonday = ((int)now.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            DateTime currentWeekStart = now.Date.AddDays(-daysUntilMonday);

            int lastDaysUntilMonday = ((int)lastNotification.Value.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            DateTime lastWeekStart = lastNotification.Value.Date.AddDays(-lastDaysUntilMonday);

            return currentWeekStart > lastWeekStart;
        }

        private static bool ShouldSendMonthly(DateTime? lastNotification, DateTime now) {
            if (!lastNotification.HasValue)
                return true;

            // Send if we're in a different month
            return lastNotification.Value.Year != now.Year ||
                   lastNotification.Value.Month != now.Month;
        }
        #endregion

        #region private methods
        private SystemError HandleError(IUnitOfWork uow, Exception ex) {

            //..log inner exceptions here too
            var innerEx = ex.InnerException;
            while (innerEx != null) {
                Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                innerEx = innerEx.InnerException;
            }

            var company = uow.CompanyRepository.GetAll(false).FirstOrDefault();
            long companyId = company != null ? company.Id : 1;
            return new() {
                ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                ErrorSource = "STATUTORY-ARTICLES-SERVICE",
                StackTrace = ex.StackTrace,
                Severity = "CRITICAL",
                ReportedOn = DateTime.Now,
                CompanyId = companyId
            };

        }

        #endregion
    }
}

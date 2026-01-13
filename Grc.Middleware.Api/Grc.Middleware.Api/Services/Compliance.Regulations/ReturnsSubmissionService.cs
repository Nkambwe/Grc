using AutoMapper;
using Azure.Core;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Compliance.Returns;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Utils;
using RTools_NTS.Util;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Services.Compliance.Regulations {

    public class ReturnsSubmissionService : BaseService, IReturnsSubmissionService {
        public ReturnsSubmissionService(IServiceLoggerFactory loggerFactory, 
                                         IUnitOfWorkFactory uowFactory, 
                                         IMapper mapper) : base(loggerFactory, uowFactory, mapper) {
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
                    _ = uow.RegulatorySubmissionRepository.Delete(statute, request.markAsDeleted);

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
                    _ = await uow.RegulatorySubmissionRepository.DeleteAsync(tasktask, request.markAsDeleted);

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
            Logger.LogActivity($"Count number of Submission in the database", "INFO");

            //..exclude from processing
            var excludeFrequencies = new[] {"NA", "PERIODIC", "ONE-OFF", "ON OCCURRENCE"};

            try {
                var reports = await uow.ReturnRepository.GetAllAsync(r => r.Frequency != null, false, r => r.Frequency);
                var newSubmissions = new List<ReturnSubmissionRequest>();
                var breachedUpdates = new List<ReturnSubmissionRequest>();

                foreach (var report in reports) {
                    
                    if (excludeFrequencies.Contains(report.Frequency.FrequencyName))
                        continue;

                    var (start, end) = FrequencyPeriodCalculator.GetCurrentPeriod(report.Frequency.FrequencyName, today);

                    //..check if submission exists for this period
                    if (!await ExistsAsync(r => r.ReturnId == report.Id && r.PeriodStart == start && r.PeriodEnd == end, false, ct)) {
                        newSubmissions.Add(new ReturnSubmissionRequest {
                            ReturnId = report.Id,
                            Status = "OPEN",
                            PeriodStart = start,
                            IsBreached = false,
                            PeriodEnd = end,
                            Deadline = end,
                            SubmissionDate = null,
                            CreatedBy = "SYSTEM",
                            CreatedOn = DateTime.Now,
                            ModifiedBy = "SYSTEM",
                            ModifiedOn = DateTime.Now
                        });
                    }

                    //..breach old submissions
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
                            ModifiedOn = DateTime.Now
                        });
                    }
                }

                if (newSubmissions.Any())
                    await BulkyInsertAsync(newSubmissions.ToArray());

                if (breachedUpdates.Any())
                    await BulkyUpdateAsync(breachedUpdates.ToArray());
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to Submission in the database: {ex.Message}", "ERROR");
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }

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

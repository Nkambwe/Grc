using AutoMapper;
using Azure;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Compliance.Regulations;
using Grc.Middleware.Api.Data.Entities.Compliance.Returns;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Enums;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Http.Responses;
using Grc.Middleware.Api.Utils;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Services.Compliance.Regulations {
    public class ReturnService : BaseService, IReturnService {

        public ReturnService(IServiceLoggerFactory loggerFactory,
            IUnitOfWorkFactory uowFactory, IMapper mapper)
            : base(loggerFactory, uowFactory, mapper) {
        }

        #region Statistics

        public async Task<ComplianceStatisticsResponse> GetComplianceStatisticsAsync(bool includeDeleted) {

            using var uow = UowFactory.Create();
            Logger.LogActivity($"Generate compliance statistics", "INFO");

            var statistics = new ComplianceStatisticsResponse() {
                CircularStatuses = new(),
                ReturnStatuses = new(),
                TaskStatuses = new(),
                Policies = new()
            };

            try {

                //..policies
                var policies = await uow.RegulatoryDocumentRepository.GetAllAsync(includeDeleted);
                var statusGroups = policies.GroupBy(p => p.Status).ToDictionary(g => g.Key, g => g.Count());
                statistics.Policies.Add("On Hold", statusGroups.GetValueOrDefault("ON-HOLD", 0));
                statistics.Policies.Add("Department Review", statusGroups.GetValueOrDefault("DEPT-REVIEW", 0));
                statistics.Policies.Add("Not Uptodate", statusGroups.GetValueOrDefault("DUE", 0));
                statistics.Policies.Add("Board Review", statusGroups.GetValueOrDefault("PENDING-BOARD", 0));
                statistics.Policies.Add("Uptodate", statusGroups.GetValueOrDefault("UPTODATE", 0));
                statistics.Policies.Add("Standard", statusGroups.GetValueOrDefault("NA", 0));
                statistics.Policies.Add("Total", policies.Count);

                //..returns
                var returns = await uow.ReturnRepository.GetAllAsync(includeDeleted, a=> a.Frequency);
                statistics.ReturnStatuses = returns.GroupBy(d => d.Frequency?.FrequencyName?? "NA").ToDictionary(g => g.Key,g => g.Count());
                statistics.ReturnStatuses.Add("TOTALS", returns.Count);

                //..circulars
                var circulars = await uow.CircularRepository.GetAllAsync(includeDeleted, c => c.Authority, c => c.Department);
                statistics.CircularStatuses = circulars.GroupBy(d => d.Authority?.AuthorityAlias ?? "OTHER").ToDictionary(g => g.Key, g => g.Count());
                statistics.CircularStatuses.Add("TOTALS", circulars.Count);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve statistics: {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }


            return statistics;
        }

        public async Task<PolicyDashboardResponse> GetPolicyStatisticsAsync(bool includeDeleted, PolicyStatus status) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Generate compliance statistics", "INFO");

            var response = new PolicyDashboardResponse {
                Statistics = new Dictionary<string, int>(),
                Policies = new List<PolicyItemResponse>()
            };

            try {
                var statusFilter = MapPolicyStatusToFilter(status);

                var documents = statusFilter != null?
                    await uow.RegulatoryDocumentRepository.GetAllAsync(p => p.Status == statusFilter, includeDeleted, p => p.Owner, p => p.Owner.Department):
                    await uow.RegulatoryDocumentRepository.GetAllAsync(includeDeleted, p => p.Owner, p => p.Owner.Department);

                if (documents?.Any() == true) {
                    response.Statistics = documents
                        .GroupBy(d => d.Owner?.Department?.DepartmentName ?? "Unassigned")
                        .ToDictionary(g => g.Key, g => g.Count());

                    response.Policies = documents.Select(d => new PolicyItemResponse {
                        Id = d.Id,
                        Title = d.DocumentName,
                        OwnerId = d.ResponsibilityId,
                        Department = d.Owner?.Department?.DepartmentName,
                        ReviewDate = d.LastRevisionDate
                    }).ToList();
                }

                return response;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to generate policy statistics: {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public async Task<ReturnDashboardResponse> GetReturnStatisticsAsync(bool includeDeleted, ReportPeriod period) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Generate returns statistics", "INFO");

            var response = new ReturnDashboardResponse {
                Statistics = new Dictionary<string, int>(),
                Reports = new List<ReturnReportResponse>()
            };

            try {
                //get frequency name from enum
                var frequencyName = MapReturnStatusToFilter(period);

                //..get records
                var reports = await uow.ReturnRepository.GetAllAsync(
                    p => p.Frequency.FrequencyName == frequencyName, includeDeleted,
                    p => p.Department,p => p.ReturnType,p => p.Frequency);
                if (reports?.Any() == true) {
                    //..department statistics
                    response.Statistics = reports.GroupBy(d => d.Department?.DepartmentName ?? "Unassigned")
                                                 .ToDictionary(g => g.Key, g => g.Count());

                    //..returns list
                    response.Reports = reports.Select(r => new ReturnReportResponse {
                        Id = r.Id,
                        Title = r.ReturnName,
                        Department = r.Department?.DepartmentName,
                        Type = r.ReturnType?.TypeName,
                    }).ToList();
                }

                return response;
            } catch (Exception ex) {
                Logger.LogActivity(
                    $"Failed to generate returns statistics: {ex.Message}",
                    "ERROR");

                LogError(uow, ex);
                throw;
            }
        }

        public async Task<ReturnExtensionResponse> GetReturnExtensionStatisticsAsync(bool includeDeleted, ReportPeriod period) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Generate returns statistics", "INFO");

            try {
                var frequencyName = MapReturnStatusToFilter(period);
                var reports = await uow.ReturnRepository.GetAllAsync(r => r.Frequency.FrequencyName == frequencyName, includeDeleted,
                    r => r.Department, r => r.Frequency, r => r.Submissions);

                if (reports == null || reports.Count == 0) {
                    return new ReturnExtensionResponse {
                        Periods = new Dictionary<string, int>(),
                        Reports = new List<ReturnSubmissionResponse>()
                    };
                }

                var response = new ReturnExtensionResponse {
                    Periods = reports.GroupBy(r => r.Frequency?.FrequencyName ?? "NA").ToDictionary(g => g.Key, g => g.Count()),

                    Reports = reports
                        .SelectMany(r => r.Submissions ?? Enumerable.Empty<ReturnSubmission>(),
                            (r, s) => new ReturnSubmissionResponse {
                                Id = r.Id,
                                Title = r.ReturnName,
                                Status = s.Status ?? "OPEN",
                                Department = r.Department?.DepartmentName,
                                Risk = string.Empty
                            })
                        .ToList()
                };

                return response;
            } catch (Exception ex) {
                Logger.LogActivity(
                    $"Failed to generate returns statistics: {ex.Message}",
                    "ERROR");

                LogError(uow, ex);
                throw;
            }
        }


        public async Task<ReturnsStatisticsResponses> GetReturnDashboardStatisticsAsync(bool includeDeleted) {

            using var uow = UowFactory.Create();
            Logger.LogActivity($"Generate compliance statistics", "INFO");

            var statistics = new ReturnsStatisticsResponses() {
                Periods = new Dictionary<string, int>(),
                Statuses = new Dictionary<string, Dictionary<string, int>> ()
            };

            try {
                // ..returns
                var returns = await uow.ReturnRepository.GetAllAsync(includeDeleted,r => r.Frequency,r => r.Submissions);
                statistics.Periods = returns.GroupBy(d => d.Frequency?.FrequencyName ?? "NA").ToDictionary(g => g.Key, g => g.Count());
                statistics.Periods.Add("TOTALS", returns.Count);

                //..graph data from submissions
                statistics.Statuses = returns.GroupBy(r => r.Frequency?.FrequencyName ?? "NA").ToDictionary(
                    periodGroup => periodGroup.Key,
                    periodGroup => periodGroup
                        .SelectMany(r => r.Submissions ?? Enumerable.Empty<ReturnSubmission>())
                        .GroupBy(s => s.Status) 
                        .ToDictionary(
                            statusGroup => statusGroup.Key.ToString(),
                            statusGroup => statusGroup.Count()
                        )
                );

            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve statistics: {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }


            return statistics;
        }

        public async Task<CircularStatisticsResponse> GetCircularDashboardStatisticsAsync(bool includeDeleted) {

            using var uow = UowFactory.Create();
            Logger.LogActivity($"Circular statistics", "INFO");

            var statistics = new CircularStatisticsResponse() {
                Authorities = new Dictionary<string, int>(),
                Statuses = new Dictionary<string, Dictionary<string, int>>()
            };

            try {
                // ..circulars
                var circulars = await uow.CircularRepository.GetAllAsync(includeDeleted, c => c.Authority, c => c.Department);
                statistics.Authorities = circulars.GroupBy(d => d.Authority?.AuthorityAlias ?? "OTHER").ToDictionary(g => g.Key, g => g.Count());
                statistics.Authorities.Add("TOTALS", circulars.Count);

                //..graph data from submissions
                statistics.Statuses = circulars.GroupBy(c => c.Authority?.AuthorityAlias ?? "OTHER")
                                               .ToDictionary(authorityGroup => authorityGroup.Key, authorityGroup => authorityGroup
                                                    .GroupBy(c => c.Status)
                                                    .ToDictionary(
                                                        statusGroup => statusGroup.Key?.ToString() ?? "OPEN",
                                                        statusGroup => statusGroup.Count()
                                                    )
                                               );


            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve statistics: {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }

            return statistics;
        }

        public async Task<CircularDashboardResponse> GetCircularStatisticsAsync(bool includeDeleted, string authority) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Generate circular statistics", "INFO");

            var response = new CircularDashboardResponse {
                Statistics = new Dictionary<string, int>(),
                Circulars = new List<CircularReportResponse>()
            };

            try {

                //..get records
                var reports = await uow.CircularRepository.GetAllAsync(p => p.Authority.AuthorityAlias == authority, includeDeleted,p => p.Department, p => p.Authority);
                if (reports?.Any() == true) {
                    //..department statistics
                    response.Statistics = reports.GroupBy(d => d.Status ?? "OPEN")
                                                 .ToDictionary(g => g.Key, g => g.Count());

                    //..returns list
                    response.Circulars = reports.Select(r => new CircularReportResponse {
                        Id = r.Id,
                        Title = r.CircularTitle,
                        Status = r.Status,
                        AuthorityAlias = r.Authority?.AuthorityAlias,
                        Authority = r.Authority?.AuthorityName ?? string.Empty,
                        Department = r.Department?.DepartmentName ?? string.Empty,
                    }).ToList();
                }

                return response;
            } catch (Exception ex) {
                Logger.LogActivity(
                    $"Failed to generate returns statistics: {ex.Message}",
                    "ERROR");

                LogError(uow, ex);
                throw;
            }
        }

        public async Task<CircularExtensionResponses> GetCircularExtensionStatisticsAsync(bool includeDeleted, string authority) {

            using var uow = UowFactory.Create();
            Logger.LogActivity($"Generate circular statistics", "INFO");

            var statistics = new CircularExtensionResponses() {
                Statuses = new(),
                Reports = new()
            };

            try {
                //..get records
                var reports = await uow.CircularRepository.GetAllAsync(p => p.Authority.AuthorityAlias == authority, includeDeleted, p => p.Department, p => p.Authority);
                if (reports?.Any() == true) {
                    //..department statistics
                    statistics.Statuses = reports.GroupBy(d => d.Status ?? "OPEN")
                                                 .ToDictionary(g => g.Key, g => g.Count());

                    //..returns list
                    statistics.Reports = reports.Select(r => new CircularExtensionResponse {
                        Id = r.Id,
                        Title = r.CircularTitle,
                        Status = r.Status,
                        AuthorityAlias = r.Authority?.AuthorityAlias,
                        Authority = r.Authority?.AuthorityName ?? string.Empty,
                        Department = r.Department?.DepartmentName ?? string.Empty,
                    }).ToList();
                }
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve statistics: {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }

            return statistics;

        }

        #endregion

        #region Queries
        public int Count()
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Regulatory Return in the database", "INFO");

            try
            {
                return uow.ReturnRepository.Count();
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to Regulatory Return in the database: {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public int Count(Expression<Func<ReturnReport, bool>> predicate)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Regulatory Return in the database", "INFO");

            try
            {
                return uow.ReturnRepository.Count(predicate);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Regulatory Return in the database: {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Regulatory Return in the database", "INFO");

            try
            {
                return await uow.ReturnRepository.CountAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Regulatory Return in the database: {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of  Regulatory Returns in the database", "INFO");

            try
            {
                return await uow.StatutoryArticleRepository.CountAsync(excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Regulatory Return in the database: {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<int> CountAsync(Expression<Func<ReturnReport, bool>> predicate, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Regulatory Return in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.ReturnRepository.CountAsync(predicate, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Regulatory Return in the database: {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<int> CountAsync(Expression<Func<ReturnReport, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Regulatory Return in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.ReturnRepository.CountAsync(predicate, excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Regulatory Return in the database: {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public bool Exists(Expression<Func<ReturnReport, bool>> predicate, bool excludeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an Regulatory Return exists in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.ReturnRepository.Exists(predicate, excludeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for Regulatory Return in the database: {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<ReturnReport, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an Regulatory Return exists in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.ReturnRepository.ExistsAsync(predicate, excludeDeleted, token);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for Regulatory Return in the database: {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<ReturnReport, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check for batch Regulatory Return if they exist in the database that fit predicate >> '{predicates}'", "INFO");

            try
            {
                return await uow.ReturnRepository.ExistsBatchAsync(predicates, excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for Regulatory Return in the database: {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public ReturnReport Get(long id, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Regulatory Return with ID '{id}'", "INFO");

            try {
                return uow.ReturnRepository.Get(id, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Regulatory Return: {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public ReturnReport Get(Expression<Func<ReturnReport, bool>> predicate, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Regulatory Return that fits predicate >> '{predicate}'", "INFO");

            try {
                return uow.ReturnRepository.Get(predicate, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Regulatory Return : {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public ReturnReport Get(Expression<Func<ReturnReport, bool>> predicate, bool includeDeleted = false, params Expression<Func<ReturnReport, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Regulatory Return that fits predicate >> '{predicate}'", "INFO");

            try {
                return uow.ReturnRepository.Get(predicate, includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Regulatory Return : {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public IQueryable<ReturnReport> GetAll(bool includeDeleted = false, params Expression<Func<ReturnReport, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Regulatory Return", "INFO");

            try {
                return uow.ReturnRepository.GetAll(includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Regulatory Return : {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public IList<ReturnReport> GetAll(bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all regulatory return", "INFO");

            try {
                return uow.ReturnRepository.GetAll(includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve regulatory return : {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public IList<ReturnReport> GetAll(Expression<Func<ReturnReport, bool>> predicate, bool includeDeleted) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Regulatory Return that fit predicate '{predicate}'", "INFO");

            try {
                return uow.ReturnRepository.GetAll(predicate, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Regulatory Return : {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public async Task<IList<ReturnReport>> GetAllAsync(bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Regulatory Return", "INFO");

            try {
                return await uow.ReturnRepository.GetAllAsync(includeDeleted);
            } catch (Exception ex)  {
                Logger.LogActivity($"Failed to retrieve Regulatory Return : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<IList<ReturnReport>> GetAllAsync(Expression<Func<ReturnReport, bool>> predicate, bool includeDeleted) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Regulatory Return that fit predicate '{predicate}'", "INFO");

            try {
                return await uow.ReturnRepository.GetAllAsync(predicate, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Regulatory Return : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<IList<ReturnReport>> GetAllAsync(Expression<Func<ReturnReport, bool>> predicate, bool includeDeleted = false, params Expression<Func<ReturnReport, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Regulatory Return that fit predicate '{predicate}'", "INFO");

            try {
                return await uow.ReturnRepository.GetAllAsync(predicate, includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Regulatory Return : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<IList<ReturnReport>> GetAllAsync(bool includeDeleted = false, params Expression<Func<ReturnReport, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Get all Regulatory Return", "INFO");

            try {
                return await uow.ReturnRepository.GetAllAsync(includeDeleted, includes);
            } catch (Exception ex)  {
                Logger.LogActivity($"Failed to retrieve Regulatory Return : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<ReturnReport> GetAsync(long id, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Regulatory Return with ID '{id}'", "INFO");

            try {
                return await uow.ReturnRepository.GetAsync(id, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Regulatory Return : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<ReturnReport> GetAsync(Expression<Func<ReturnReport, bool>> predicate, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Regulatory Return that fit predicate '{predicate}'", "INFO");

            try {
                return await uow.ReturnRepository.GetAsync(predicate, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Regulatory Return : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<ReturnReport> GetAsync(Expression<Func<ReturnReport, bool>> predicate, bool includeDeleted = false, params Expression<Func<ReturnReport, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Regulatory Return that fit predicate '{predicate}'", "INFO");

            try {
                return await uow.ReturnRepository.GetAsync(predicate, includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Regulatory Return : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<IList<ReturnReport>> GetTopAsync(Expression<Func<ReturnReport, bool>> predicate, int top, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get top {top} Regulatory Returns that fit predicate >> {predicate}", "INFO");

            try {
                return await uow.ReturnRepository.GetTopAsync(predicate, top, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Regulatory Return : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public bool Insert(ReturnRequest request) {
            using var uow = UowFactory.Create();
            try {
                //..map Regulatory Return request to Regulatory Return entity
                var article = new ReturnReport() {
                    ArticleId = request.ArticleId,
                    ReturnName = request.ReturnName,
                    TypeId = request.TypeId,
                    AuthorityId = request.AuthorityId,
                    FrequencyId = request.FrequencyId,
                    DepartmentId = request.DepartmentId,
                    CreatedBy = request.UserName,
                    CreatedOn = DateTime.Now,
                    LastModifiedBy = request.UserName,
                    LastModifiedOn = DateTime.Now,
                    Comments = request.Comments,
                    IsDeleted = request.IsDeleted,
                };

                //..log the Regulatory Return data being saved
                var articleJson = JsonSerializer.Serialize(article, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Regulatory Return data: {articleJson}", "DEBUG");

                var added = uow.ReturnRepository.Insert(article);
                if (added) {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(article).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to save submission : {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public async Task<bool> InsertAsync(ReturnRequest request) {
            using var uow = UowFactory.Create();
            try
            {
                //..map Regulatory Return request to Regulatory Return entity
                var article = new ReturnReport() {
                    ArticleId = request.ArticleId,
                    ReturnName = request.ReturnName,
                    TypeId = request.TypeId,
                    AuthorityId = request.AuthorityId,
                    FrequencyId = request.FrequencyId,
                    DepartmentId = request.DepartmentId,
                    CreatedBy = request.UserName,
                    CreatedOn = DateTime.Now,
                    LastModifiedBy = request.UserName,
                    LastModifiedOn = DateTime.Now,
                    Comments = request.Comments,
                    IsDeleted = request.IsDeleted,
                };

                //..log the Regulatory Return data being saved
                var articleJson = JsonSerializer.Serialize(article, new JsonSerializerOptions {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Regulatory Return data: {articleJson}", "DEBUG");

                var added = await uow.ReturnRepository.InsertAsync(article);
                if (added) {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(article).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to save Regulatory Return : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public bool Update(ReturnRequest request, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update Regulatory Return request", "INFO");

            try {
                var regReturn = uow.ReturnRepository.Get(a => a.Id == request.Id);
                if (regReturn != null) {
                    //..update record
                    regReturn.ReturnName = (request.ReturnName ?? string.Empty).Trim();
                    regReturn.TypeId = request.TypeId;
                    regReturn.FrequencyId = request.FrequencyId;
                    regReturn.ArticleId = request.ArticleId;
                    regReturn.AuthorityId = request.AuthorityId;
                    regReturn.DepartmentId = request.DepartmentId;
                    regReturn.Comments = (request.Comments ?? string.Empty).Trim();
                    regReturn.IsDeleted = request.IsDeleted;
                    regReturn.LastModifiedOn = DateTime.Now;
                    regReturn.LastModifiedBy = $"{request.UserName}";

                    //..check entity state
                    _ = uow.ReturnRepository.Update(regReturn, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(regReturn).State;
                    Logger.LogActivity($"Regulatory Return state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to update Regulatory Return record: {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(ReturnRequest request, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update Regulatory Return", "INFO");

            try {
                var regReturn = await uow.ReturnRepository.GetAsync(a => a.Id == request.Id);
                if (regReturn != null) {
                    //..update record
                    regReturn.ReturnName = (request.ReturnName ?? string.Empty).Trim();
                    regReturn.TypeId = request.TypeId;
                    regReturn.FrequencyId = request.FrequencyId;
                    regReturn.ArticleId = request.ArticleId;
                    regReturn.AuthorityId = request.AuthorityId;
                    regReturn.DepartmentId = request.DepartmentId;
                    regReturn.Comments = (request.Comments ?? string.Empty).Trim();
                    regReturn.IsDeleted = request.IsDeleted;
                    regReturn.LastModifiedOn = DateTime.Now;
                    regReturn.LastModifiedBy = $"{request.UserName}";

                    //..check entity state
                    _ = await uow.ReturnRepository.UpdateAsync(regReturn, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(regReturn).State;
                    Logger.LogActivity($"Regulatory Regulatory Return state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to update Regulatory Return record: {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public bool Delete(IdRequest request) {
            using var uow = UowFactory.Create();
            try {
                var auditJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Regulatory Return data: {auditJson}", "DEBUG");

                var statute = uow.ReturnRepository.Get(t => t.Id == request.RecordId);
                if (statute != null)
                {
                    //..mark as delete this Regulatory Return
                    _ = uow.ReturnRepository.Delete(statute, request.markAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(statute).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to delete Regulatory Return : {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(IdRequest request) {
            using var uow = UowFactory.Create();
            try {
                var statuteJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Regulatory Return data: {statuteJson}", "DEBUG");

                var tasktask = await uow.ReturnRepository.GetAsync(t => t.Id == request.RecordId);
                if (tasktask != null)
                {
                    //..mark as delete this Regulatory Return
                    _ = await uow.ReturnRepository.DeleteAsync(tasktask, request.markAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(tasktask).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = await uow.SaveChangesAsync();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to delete Regulatory Return : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<bool> DeleteAllAsync(IList<long> requestIds, bool markAsDeleted = false) {
            using var uow = UowFactory.Create();
            try {
                var statutes = await uow.ReturnRepository.GetAllAsync(e => requestIds.Contains(e.Id));
                if (statutes.Count == 0) {
                    Logger.LogActivity($"Records not found", "INFO");
                    return false;
                }
                return await uow.ReturnRepository.DeleteAllAsync(statutes, markAsDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to delete Regulatory Return: {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<bool> BulkyInsertAsync(ReturnRequest[] requestItems) {
            using var uow = UowFactory.Create();
            try {
                //..map Regulatory Return to Regulatory Return entity
                var returns = requestItems.Select(Mapper.Map<ReturnRequest, ReturnReport>).ToArray();
                var returnsJson = JsonSerializer.Serialize(returns, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Regulatory Return data: {returnsJson}", "DEBUG");
                return await uow.ReturnRepository.BulkyInsertAsync(returns);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to save Regulatory Return : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<bool> BulkyUpdateAsync(ReturnRequest[] requestItems) {
            using var uow = UowFactory.Create();
            try {
                //..map  Regulatory Returns request to  Regulatory Returns entity
                var returns = requestItems.Select(Mapper.Map<ReturnRequest, ReturnReport>).ToArray();
                var returnsJson = JsonSerializer.Serialize(returns, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Regulatory Return data: {returnsJson}", "DEBUG");
                return await uow.ReturnRepository.BulkyUpdateAsync(returns);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to save Regulatory Return : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<bool> BulkyUpdateAsync(ReturnRequest[] requestItems, params Expression<Func<ReturnReport, object>>[] propertySelectors) {
            using var uow = UowFactory.Create();
            try {
                //..map regulatory return request to regulatory return entity
                var returns = requestItems.Select(Mapper.Map<ReturnRequest, ReturnReport>).ToArray();
                var returnsJson = JsonSerializer.Serialize(returns, new JsonSerializerOptions {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($" Regulatory Returns data: {returnsJson}", "DEBUG");
                return await uow.ReturnRepository.BulkyUpdateAsync(returns, propertySelectors);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to save  Regulatory Returns : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<PagedResult<ReturnReport>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<ReturnReport, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged regulatory return", "INFO");

            try {
                return await uow.ReturnRepository.PageAllAsync(page, size, includeDeleted, null, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve regulatory return: {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<PagedResult<ReturnReport>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<ReturnReport, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged regulatory return", "INFO");

            try {
                return await uow.ReturnRepository.PageAllAsync(token, page, size, includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieves regulatory return : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<PagedResult<ReturnReport>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<ReturnReport, bool>> predicate = null) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged regulatory return", "INFO");
            try {
                return await uow.ReturnRepository.PageAllAsync(page, size, includeDeleted, predicate);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve regulatory return: {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }
        
        public async Task<PagedResult<ReturnReport>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<ReturnReport, bool>> predicate = null, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged regulatory return", "INFO");
            try {
                return await uow.ReturnRepository.PageAllAsync(token, page, size, predicate, includeDeleted);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve regulatory return : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        #endregion

        #region private methods

        private static string MapPolicyStatusToFilter(PolicyStatus status) => status switch {
            PolicyStatus.ONHOLD => "ON-HOLD",
            PolicyStatus.NEEDREVIEW => "DUE",
            PolicyStatus.PENDINGBOARD => "PENDING-BOARD",
            PolicyStatus.PENDINGDEPARTMENT => "DEPT-REVIEW",
            PolicyStatus.UPTODATE => "UPTODATE",
            PolicyStatus.STANDARD => "NA",
            _ => null
        };

        private static string MapReturnStatusToFilter(ReportPeriod status) => status switch {
            ReportPeriod.DAILY => "DAILY",
            ReportPeriod.WEEKLY => "WEEKLY",
            ReportPeriod.MONTHLY => "MONTHLY",
            ReportPeriod.QUARTERLY => "QUARTERLY",
            ReportPeriod.BIANNUAL => "BIANNUAL",
            ReportPeriod.ANNUAL => "ANNUAL",
            ReportPeriod.BIENNIAL => "BIENNIAL",
            ReportPeriod.TRIENNIAL => "TRIENNIAL",
            ReportPeriod.PERIODIC => "PERIODIC",
            ReportPeriod.NA => "NA",
            ReportPeriod.ONEOFF => "ONE-OFF",
            _ => "ON OCCURRENCE"
        };

        private void LogError(IUnitOfWork uow, Exception ex) {
            var currentEx = ex.InnerException;
            while (currentEx != null) {
                Logger.LogActivity($"Service Inner Exception: {currentEx.Message}", "ERROR");
                currentEx = currentEx.InnerException;
            }

            // Get company ID efficiently
            var company = uow.CompanyRepository.GetAll(c => true, false);
            long companyId = company.FirstOrDefault()?.Id ?? 1L;

            // Get innermost exception
            var innermostException = ex;
            while (innermostException.InnerException != null)
                innermostException = innermostException.InnerException;

            var errorObj = new SystemError {
                ErrorMessage = innermostException.Message,
                ErrorSource = "REGULATORY-RETURN-SERVICE",
                StackTrace = ex.StackTrace,
                Severity = "CRITICAL",
                ReportedOn = DateTime.Now,
                CompanyId = companyId
            };

            uow.SystemErrorRespository.Insert(errorObj);
            uow.SaveChanges();
        }

        private async Task LogErrorAsync(IUnitOfWork uow, Exception ex) {
            var innerEx = ex.InnerException;
            while (innerEx != null) {
                Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                innerEx = innerEx.InnerException;
            }
            Logger.LogActivity($"{ex.StackTrace}", "ERROR");

            var company = uow.CompanyRepository.GetAll(false).FirstOrDefault();
            long companyId = company != null ? company.Id : 1;
            SystemError errorObj = new() {
                ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                ErrorSource = "REGULATORY-RETURN-SERVICE",
                StackTrace = ex.StackTrace,
                Severity = "CRITICAL",
                ReportedOn = DateTime.Now,
                CompanyId = companyId
            };

            //..save error object to the database
            _ = await uow.SystemErrorRespository.InsertAsync(errorObj);
        }

        #endregion
    }
}

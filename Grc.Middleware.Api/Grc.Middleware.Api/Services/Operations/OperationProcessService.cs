using AutoMapper;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Operations.Processes;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Enums;
using Grc.Middleware.Api.Extensions;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Http.Responses;
using Grc.Middleware.Api.Utils;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Services.Operations {

    public class OperationProcessService : BaseService, IOperationProcessService {

        public OperationProcessService(IServiceLoggerFactory loggerFactory, IUnitOfWorkFactory uowFactory, IMapper mapper) : base(loggerFactory, uowFactory, mapper) { }

        #region Count Methods
        public int Count()
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Processes in the database", "INFO");

            try
            {
                return uow.OperationProcessRepository.Count();
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to Processes in the database: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public int Count(Expression<Func<OperationProcess, bool>> predicate)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Processes in the database", "INFO");

            try
            {
                return uow.OperationProcessRepository.Count(predicate);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Processes in the database: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Processes in the database", "INFO");

            try
            {
                return await uow.OperationProcessRepository.CountAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Processes in the database: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of  Processes in the database", "INFO");

            try
            {
                return await uow.OperationProcessRepository.CountAsync(excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Processes in the database: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<int> CountAsync(Expression<Func<OperationProcess, bool>> predicate, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Processes in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.OperationProcessRepository.CountAsync(predicate, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Processes in the database: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<int> CountAsync(Expression<Func<OperationProcess, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Processes in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.OperationProcessRepository.CountAsync(predicate, excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Processes in the database: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        #endregion

        #region Statistics Methods

        public async Task<ServiceOperationUnitCountResponse> GetOperationUnitStatisticsAsync(bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            var processes = await uow.OperationProcessRepository.GetAllAsync(p => includeDeleted || !p.IsDeleted, false, p => p.Unit);

            //..initialize all units
            var allUnits = new[] {
                ServiceOperationUnit.Cash,
                ServiceOperationUnit.AccountServices,
                ServiceOperationUnit.Channels,
                ServiceOperationUnit.Payments,
                ServiceOperationUnit.Wallets,
                ServiceOperationUnit.RecordsMgt,
                ServiceOperationUnit.CustomerExp,
                ServiceOperationUnit.Reconciliation
            };

            //..initialize all categories
            var allCategories = new[] {
                ServiceProcessCategories.Draft,
                ServiceProcessCategories.UpToDate,
                ServiceProcessCategories.Unchanged,
                ServiceProcessCategories.Proposed,
                ServiceProcessCategories.Due,
                ServiceProcessCategories.Review,
                ServiceProcessCategories.Dormant,
                ServiceProcessCategories.Cancelled
            };

            //..map processes to units and categories
            var processData = processes
                .Select(p => new { Unit = AdjustedUnitName(p.Unit.UnitName), Category = AdjustedStatus(p.ProcessStatus) })
                .Where(p => p.Unit != ServiceOperationUnit.Unknown).ToList();

            //..helper method to count by unit and optional category filter
            ServiceUnitCountResponse CountByUnit(Func<ServiceProcessCategories, bool> categoryFilter = null) {
                var filtered = categoryFilter != null ? processData.Where(p => categoryFilter(p.Category)): processData;
                var counts = filtered.GroupBy(p => p.Unit).ToDictionary(g => g.Key, g => g.Count());

                return new ServiceUnitCountResponse {
                    CashProcesses = counts.GetValueOrDefault(ServiceOperationUnit.Cash, 0),
                    AccountServiceProcesses = counts.GetValueOrDefault(ServiceOperationUnit.AccountServices, 0),
                    ChannelProcesses = counts.GetValueOrDefault(ServiceOperationUnit.Channels, 0),
                    PaymentProcesses = counts.GetValueOrDefault(ServiceOperationUnit.Payments, 0),
                    WalletProcesses = counts.GetValueOrDefault(ServiceOperationUnit.Wallets, 0),
                    RecordsManagementProcesses = counts.GetValueOrDefault(ServiceOperationUnit.RecordsMgt, 0),
                    CustomerExperienceProcesses = counts.GetValueOrDefault(ServiceOperationUnit.CustomerExp, 0),
                    ReconciliationProcesses = counts.GetValueOrDefault(ServiceOperationUnit.Reconciliation, 0),
                    TotalProcesses = counts.Values.Sum()
                };
            }

            //..helper method to count by category for a specific unit
            ServiceCategoriesCountResponse CountByCategory(ServiceOperationUnit unit) {
                var unitProcesses = processData.Where(p => p.Unit == unit).ToList();
                var counts = unitProcesses.GroupBy(p => p.Category).ToDictionary(g => g.Key, g => g.Count());

                return new ServiceCategoriesCountResponse {
                    Unclassified = counts.GetValueOrDefault(ServiceProcessCategories.Draft, 0),
                    UpToDate = counts.GetValueOrDefault(ServiceProcessCategories.UpToDate, 0),
                    Unchanged = counts.GetValueOrDefault(ServiceProcessCategories.Unchanged, 0),
                    Proposed = counts.GetValueOrDefault(ServiceProcessCategories.Proposed, 0),
                    Due = counts.GetValueOrDefault(ServiceProcessCategories.Due, 0),
                    InReview = counts.GetValueOrDefault(ServiceProcessCategories.Review, 0),
                    Dormant = counts.GetValueOrDefault(ServiceProcessCategories.Dormant, 0),
                    Cancelled = counts.GetValueOrDefault(ServiceProcessCategories.Cancelled, 0),
                    Total = counts.Values.Sum()
                };
            }

            //..helper method to count totals by category across all units
            ServiceCategoriesCountResponse CountTotalsByCategory() {
                var counts = processData.GroupBy(p => p.Category).ToDictionary(g => g.Key, g => g.Count());

                return new ServiceCategoriesCountResponse {
                    Unclassified = counts.GetValueOrDefault(ServiceProcessCategories.Draft, 0),
                    UpToDate = counts.GetValueOrDefault(ServiceProcessCategories.UpToDate, 0),
                    Unchanged = counts.GetValueOrDefault(ServiceProcessCategories.Unchanged, 0),
                    Proposed = counts.GetValueOrDefault(ServiceProcessCategories.Proposed, 0),
                    Due = counts.GetValueOrDefault(ServiceProcessCategories.Due, 0),
                    InReview = counts.GetValueOrDefault(ServiceProcessCategories.Review, 0),
                    Dormant = counts.GetValueOrDefault(ServiceProcessCategories.Dormant, 0),
                    Cancelled = counts.GetValueOrDefault(ServiceProcessCategories.Cancelled, 0),
                    Total = counts.Values.Sum()
                };
            }

            //..build the response
            var response = new ServiceOperationUnitCountResponse {
                UnitProcesses = new ServiceOperationUnitStatisticsResponse {
                    TotalUnitProcess = CountByUnit(),
                    CompletedProcesses = CountByUnit(c => c == ServiceProcessCategories.UpToDate),
                    ProposedProcesses = CountByUnit(c => c == ServiceProcessCategories.Proposed),
                    UnchangedProcesses = CountByUnit(c => c == ServiceProcessCategories.Unchanged),
                    ProcessesDueForReview = CountByUnit(c => c == ServiceProcessCategories.Due),
                    DormantProcesses = CountByUnit(c => c == ServiceProcessCategories.Dormant),
                    CancelledProcesses = CountByUnit(c => c == ServiceProcessCategories.Cancelled),
                    UnclassifiedProcesses = CountByUnit(c => c == ServiceProcessCategories.Draft)
                },
                ProcessCategories = new ServiceProcessCategoryStatisticsResponse {
                    CashProcesses = CountByCategory(ServiceOperationUnit.Cash),
                    AccountServiceProcesses = CountByCategory(ServiceOperationUnit.AccountServices),
                    ChannelProcesses = CountByCategory(ServiceOperationUnit.Channels),
                    PaymentProcesses = CountByCategory(ServiceOperationUnit.Payments),
                    WalletProcesses = CountByCategory(ServiceOperationUnit.Wallets),
                    RecordsMgtProcesses = CountByCategory(ServiceOperationUnit.RecordsMgt),
                    CustomerExperienceProcesses = CountByCategory(ServiceOperationUnit.CustomerExp),
                    ReconciliationProcesses = CountByCategory(ServiceOperationUnit.Reconciliation),
                    TotalCategoryProcesses = CountTotalsByCategory()
                }
            };

            return response;
        }

        public async Task<ServiceCategoriesCountResponse> GetProcessCategoryStatisticsAsync(bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            var processes = await uow.OperationProcessRepository.GetAllAsync(p => includeDeleted || !p.IsDeleted, false);

            //..group and count by mapped category
            var categoryCounts = processes.Where(p => !string.IsNullOrEmpty(p.ProcessStatus))
                                .GroupBy(p => AdjustedStatus(p.ProcessStatus))
                                .ToDictionary(g => g.Key, g => g.Count());

            //..helper inner function to get count safely
            int GetCount(ServiceProcessCategories category) 
                => categoryCounts.TryGetValue(category, out var count) ? count : 0;

            var response = new ServiceCategoriesCountResponse {
                Unclassified = GetCount(ServiceProcessCategories.Draft),
                UpToDate = GetCount(ServiceProcessCategories.UpToDate),
                Unchanged = GetCount(ServiceProcessCategories.Unchanged),
                Proposed = GetCount(ServiceProcessCategories.Proposed),
                Due = GetCount(ServiceProcessCategories.Due),
                InReview = GetCount(ServiceProcessCategories.Review),
                Dormant = GetCount(ServiceProcessCategories.Dormant),
                Cancelled = GetCount(ServiceProcessCategories.Cancelled),
                Total = categoryCounts.Values.Sum()
            };

            return response;
        }


        public async Task<ServiceUnitExtensionCountResponse> GetUnitStatisticExtensionsAsync(string unitName, bool includeDeleted) {
            using var uow = UowFactory.Create();
            var processes = await uow.OperationProcessRepository.GetAllAsync(p => includeDeleted || !p.IsDeleted, false,p => p.Unit);

            //..map the unit name to the enum
            var targetUnit = AdjustedUnitName(unitName);

            //..filter processes by the target unit
            var filteredProcesses = processes.Where(p => AdjustedUnitName(p.Unit.UnitName) == targetUnit);

            //..initialize all categories with 0
            var results = new Dictionary<string, int> {
                { ServiceProcessCategories.Draft.GetDescription(), 0 },
                { ServiceProcessCategories.UpToDate.GetDescription(), 0 },
                { ServiceProcessCategories.Proposed.GetDescription(), 0 },
                { ServiceProcessCategories.Unchanged.GetDescription(), 0 },
                { ServiceProcessCategories.Due.GetDescription(), 0 },
                { ServiceProcessCategories.Dormant.GetDescription(), 0 },
                { ServiceProcessCategories.Cancelled.GetDescription(), 0 },
                { ServiceProcessCategories.OnHold.GetDescription(), 0 }
            };

            //..create a mapping helper to convert ProcessStatus to category description
            var categoryMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                [ServiceProcessCategories.Draft.ToString()] = ServiceProcessCategories.Draft.GetDescription(),
                [ServiceProcessCategories.UpToDate.ToString()] = ServiceProcessCategories.UpToDate.GetDescription(),
                [ServiceProcessCategories.Proposed.ToString()] = ServiceProcessCategories.Proposed.GetDescription(),
                [ServiceProcessCategories.Unchanged.ToString()] = ServiceProcessCategories.Unchanged.GetDescription(),
                [ServiceProcessCategories.Due.ToString()] = ServiceProcessCategories.Due.GetDescription(),
                [ServiceProcessCategories.Dormant.ToString()] = ServiceProcessCategories.Dormant.GetDescription(),
                [ServiceProcessCategories.Cancelled.ToString()] = ServiceProcessCategories.Cancelled.GetDescription(),
                [ServiceProcessCategories.OnHold.ToString()] = ServiceProcessCategories.OnHold.GetDescription()
            };

            //..count processes by category
            foreach (var process in filteredProcesses) {
                if (!string.IsNullOrEmpty(process.ProcessStatus) &&
                    categoryMap.TryGetValue(process.ProcessStatus, out var categoryDescription)) {
                    results[categoryDescription]++;
                }
            }

            var response = new ServiceUnitExtensionCountResponse {
                Banner = targetUnit.GetDescription(),
                UnitProcesses = results
            };

            return response;
        }

        public async Task<ServiceCategoryExtensionCountResponse> GetCategoryStatisticExtensionsAsync(string category, bool includeDeleted) {
            using var uow = UowFactory.Create();
            var processes = await uow.OperationProcessRepository.GetAllAsync(p => includeDeleted || !p.IsDeleted, false, p => p.Unit);

            //..get the filter value for the category
            var filter = GetDescriptionFromCategory(category);

            //..initialize the results dictionary for counting occurrences
            var allUnits = new[] {
                ServiceOperationUnit.AccountServices,
                ServiceOperationUnit.Cash,
                ServiceOperationUnit.Channels,
                ServiceOperationUnit.CustomerExp,
                ServiceOperationUnit.Reconciliation,
                ServiceOperationUnit.RecordsMgt,
                ServiceOperationUnit.Payments,
                ServiceOperationUnit.Wallets
            };

            //..create dictionary from it
            var results = allUnits.ToDictionary(u => u.GetDescription(), u => 0);

            //..count filtered processes by unit
            var counts = processes.Where(p => p.ProcessStatus?.ToUpper() == filter.ToUpper())
                .Select(p => AdjustedUnitName(p.Unit.UnitName))
                .Where(unit => unit != ServiceOperationUnit.Unknown)
                .GroupBy(unit => unit).ToDictionary(g => g.Key.GetDescription(), g => g.Count());

            //..merge counts into results
            foreach (var kvp in counts) {
                results[kvp.Key] = kvp.Value;
            }

            return new ServiceCategoryExtensionCountResponse { Banner = category,
                CategoryProcesses = results.ToDictionary(r => r.Key.ToString(), r => r.Value)
            };
        }

        public async Task<List<ServiceStatisticTotalResponse>> GetProcessTotalStatisticsAsync(bool includeDeleted) {
            using var uow = UowFactory.Create();

            var processes = await uow.OperationProcessRepository.GetAllAsync(p => includeDeleted || !p.IsDeleted, false, p => p.Unit);
            var results = new Dictionary<ServiceProcessCategories, Dictionary<ServiceOperationUnit, int>>();

            //..initialize all categories & all units to zero
            foreach (var category in Enum.GetValues<ServiceProcessCategories>()) {
                results[category] = Enum.GetValues<ServiceOperationUnit>().ToDictionary(u => u, u => 0);
            }

            //..count processes
            foreach (var p in processes) {
                var status = AdjustedStatus(p.ProcessStatus);       
                var unit = AdjustedUnitName(p.Unit.UnitName);        

                results[status][unit]++;
            }

            //..compute category totals
            foreach (var cat in results.Keys.ToList()) {
                int total = results[cat].Where(x => x.Key != ServiceOperationUnit.CategoryTotal).Sum(x => x.Value);
                results[cat][ServiceOperationUnit.CategoryTotal] = total;
            }

            //..convert to response model
            var response = results.Where(x => x.Key != ServiceProcessCategories.UnitTotal) 
                .Select(kvp => new ServiceStatisticTotalResponse {
                    Banner = kvp.Key.GetDescription(),
                    Categories = kvp.Value.Where(u => u.Key != ServiceOperationUnit.Unknown) 
                        .ToDictionary(u => u.Key.GetDescription(), u => u.Value)
                }).ToList();

            return response;
        }

        #endregion

        #region General Operations
        public bool Exists(Expression<Func<OperationProcess, bool>> predicate, bool excludeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an Processes exists in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.OperationProcessRepository.Exists(predicate, excludeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for Processes in the database: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<OperationProcess, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an Processes exists in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.OperationProcessRepository.ExistsAsync(predicate, excludeDeleted, token);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for Processes in the database: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<OperationProcess, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check for batch Processes if they exist in the database that fit predicate >> '{predicates}'", "INFO");

            try
            {
                return await uow.OperationProcessRepository.ExistsBatchAsync(predicates, excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for Processes in the database: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public OperationProcess Get(long id, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get  Operation Process with ID '{id}'", "INFO");

            try
            {
                return uow.OperationProcessRepository.Get(id, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Operation Process: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public OperationProcess Get(Expression<Func<OperationProcess, bool>> predicate, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get  Operation Process that fits predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.OperationProcessRepository.Get(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Operation Process : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public OperationProcess Get(Expression<Func<OperationProcess, bool>> predicate, bool includeDeleted = false, params Expression<Func<OperationProcess, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Processes that fits predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.OperationProcessRepository.Get(predicate, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Processes : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public IQueryable<OperationProcess> GetAll(bool includeDeleted = false, params Expression<Func<OperationProcess, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Processes", "INFO");

            try
            {
                return uow.OperationProcessRepository.GetAll(includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Processes : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public IList<OperationProcess> GetAll(bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Processes", "INFO");

            try
            {
                return uow.OperationProcessRepository.GetAll(includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Processes : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public IList<OperationProcess> GetAll(Expression<Func<OperationProcess, bool>> predicate, bool includeDeleted)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Processes that fit predicate '{predicate}'", "INFO");

            try
            {
                return uow.OperationProcessRepository.GetAll(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Processes : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<OperationProcess>> GetAllAsync(bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Processes", "INFO");

            try
            {
                return await uow.OperationProcessRepository.GetAllAsync(includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Processes : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<OperationProcess>> GetAllAsync(Expression<Func<OperationProcess, bool>> predicate, bool includeDeleted)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Processes that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.OperationProcessRepository.GetAllAsync(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Processes : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<OperationProcess>> GetAllAsync(Expression<Func<OperationProcess, bool>> predicate, bool includeDeleted = false, params Expression<Func<OperationProcess, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Processes that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.OperationProcessRepository.GetAllAsync(predicate, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Processes : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<IList<OperationProcess>> GetAllAsync(bool includeDeleted = false, params Expression<Func<OperationProcess, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Get all Processes", "INFO");

            try
            {
                return await uow.OperationProcessRepository.GetAllAsync(includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Processes : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<OperationProcess> GetAsync(long id, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Operation Process with ID '{id}'", "INFO");

            try
            {
                return await uow.OperationProcessRepository.GetAsync(id, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Operation Process : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<OperationProcess> GetAsync(Expression<Func<OperationProcess, bool>> predicate, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Operation Process that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.OperationProcessRepository.GetAsync(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Operation Process : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<OperationProcess> GetAsync(Expression<Func<OperationProcess, bool>> predicate, bool includeDeleted = false, params Expression<Func<OperationProcess, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Operation Process that fit predicate '{predicate}'", "INFO");

            try {
                return await uow.OperationProcessRepository.GetAsync(predicate, includeDeleted, includes);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Process TAT : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }

        }

        public async Task<IList<OperationProcess>> GetTopAsync(Expression<Func<OperationProcess, bool>> predicate, int top, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get top {top} Processes that fit predicate >> {predicate}", "INFO");

            try
            {
                return await uow.OperationProcessRepository.GetTopAsync(predicate, top, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Processes : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Insert(ProcessRequest request)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map Operation Process request to Operation Process entity
                var process = Mapper.Map<ProcessRequest, OperationProcess>(request);

                //..log the Operation Process data being saved
                var processJson = JsonSerializer.Serialize(process, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Operation Process data: {processJson}", "DEBUG");

                var added = uow.OperationProcessRepository.Insert(process);
                if (added)
                {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(process).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");
                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save Operation Process : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> InsertAsync(ProcessRequest request)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map Operation Process request to Operation Process entity
                var process = Mapper.Map<ProcessRequest, OperationProcess>(request);

                //..log the Operation Process data being saved
                var processJson = JsonSerializer.Serialize(process, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Operation Process data: {processJson}", "DEBUG");

                var added = await uow.OperationProcessRepository.InsertAsync(process);
                if (added)
                {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(process).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save Operation Process : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public bool Update(ProcessRequest request, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update Operation Process request", "INFO");

            try
            {
                var process = uow.OperationProcessRepository.Get(a => a.Id == request.Id);
                if (process != null) {
                    //..update  Operation Process record
                    process.ProcessName = (request.ProcessName ?? string.Empty).Trim();
                    process.Description = (request.Description ?? string.Empty).Trim();
                    process.CurrentVersion = (request.CurrentVersion ?? string.Empty).Trim();
                    process.FileName = (request.FileName ?? string.Empty).Trim();
                    process.EffectiveDate = request.EffectiveDate;
                    process.LastUpdated = request.LastUpdated;
                    process.ProcessStatus = (request.ProcessStatus ?? string.Empty).Trim();
                    process.TypeId = request.TypeId;
                    process.UnitId = request.UnitId;
                    process.OwnerId = request.OwnerId;
                    process.ResponsibilityId = request.ResponsibilityId;
                    process.ReasonOnhold = (request.OnholdReason ?? string.Empty).Trim(); 
                    process.IsLockProcess = request.IsLockProcess;
                    process.NeedsBranchReview = request.NeedsBranchReview;
                    process.NeedsCreditReview = request.NeedsCreditReview;
                    process.NeedsTreasuryReview = request.NeedsTreasuryReview;
                    process.NeedsFintechReview = request.NeedsFintechReview;
                    process.Comments = (request.Comments ?? string.Empty).Trim();
                    process.IsDeleted = request.IsDeleted;
                    process.LastModifiedOn = DateTime.Now;
                    process.LastModifiedBy = $"{request.UserId}";

                    //..check entity state
                    _ = uow.OperationProcessRepository.Update(process, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(process).State;
                    Logger.LogActivity($"Operation Process state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to update  OperationProcess record: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = uow.SystemErrorRespository.Insert(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> UpdateAsync(ProcessRequest request, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update Operation Process", "INFO");

            try
            {
                var process = await uow.OperationProcessRepository.GetAsync(a => a.Id == request.Id);
                if (process != null)
                {
                    //..update  Operation Process record
                    process.ProcessName = (request.ProcessName ?? string.Empty).Trim();
                    process.Description = (request.Description ?? string.Empty).Trim();
                    process.CurrentVersion = (request.CurrentVersion ?? string.Empty).Trim();
                    process.FileName = (request.FileName ?? string.Empty).Trim();
                    process.EffectiveDate = request.EffectiveDate;
                    process.LastUpdated = request.LastUpdated;
                    process.ProcessStatus = (request.ProcessStatus ?? string.Empty).Trim();
                    process.TypeId = request.TypeId;
                    process.UnitId = request.UnitId;
                    process.OwnerId = request.OwnerId;
                    process.ResponsibilityId = request.ResponsibilityId;
                    process.ReasonOnhold = (request.OnholdReason ?? string.Empty).Trim();
                    process.IsLockProcess = request.IsLockProcess;
                    process.NeedsBranchReview = request.NeedsBranchReview;
                    process.NeedsCreditReview = request.NeedsCreditReview;
                    process.NeedsTreasuryReview = request.NeedsTreasuryReview;
                    process.NeedsFintechReview = request.NeedsFintechReview;
                    process.Comments = (request.Comments ?? string.Empty).Trim();
                    process.IsDeleted = request.IsDeleted;
                    process.LastModifiedOn = DateTime.Now;
                    process.LastModifiedBy = $"{request.UserId}";

                    //..check entity state
                    _ = await uow.OperationProcessRepository.UpdateAsync(process, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(process).State;
                    Logger.LogActivity($"Operation Process state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to update Operation Process record: {ex.Message}", "ERROR");
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
                var processJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Operation Process data: {processJson}", "DEBUG");

                var process = uow.OperationProcessRepository.Get(t => t.Id == request.RecordId);
                if (process != null)
                {
                    //..mark as delete this OperationProcess
                    _ = uow.OperationProcessRepository.Delete(process, request.markAsDeleted);

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
                Logger.LogActivity($"Failed to delete Operation Process : {ex.Message}", "ERROR");
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
                var processJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Operation Process data: {processJson}", "DEBUG");

                var process = await uow.OperationProcessRepository.GetAsync(t => t.Id == request.RecordId);
                if (process != null)
                {
                    //..mark as delete this OperationProcess
                    _ = await uow.OperationProcessRepository.DeleteAsync(process, request.markAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(process).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = await uow.SaveChangesAsync();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete  OperationProcess : {ex.Message}", "ERROR");
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
                var processes = await uow.OperationProcessRepository.GetAllAsync(e => requestIds.Contains(e.Id));
                if (processes.Count == 0)
                {
                    Logger.LogActivity($"Records not found", "INFO");
                    return false;
                }
                return await uow.OperationProcessRepository.DeleteAllAsync(processes, markAsDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete Processes: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }

        }

        public async Task<bool> BulkyInsertAsync(ProcessRequest[] requestItems)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map Processes to Processes entity
                var processes = requestItems.Select(Mapper.Map<ProcessRequest, OperationProcess>).ToArray();
                var processJson = JsonSerializer.Serialize(processes, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Processes data: {processJson}", "DEBUG");
                return await uow.OperationProcessRepository.BulkyInsertAsync(processes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save Processes : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> BulkyUpdateAsync(ProcessRequest[] requestItems)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map Processes request to Processes entity
                var processes = requestItems.Select(Mapper.Map<ProcessRequest, OperationProcess>).ToArray();
                var processJson = JsonSerializer.Serialize(processes, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Processes data: {processJson}", "DEBUG");
                return await uow.OperationProcessRepository.BulkyUpdateAsync(processes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save Processes : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> BulkyUpdateAsync(ProcessRequest[] requestItems, params Expression<Func<OperationProcess, object>>[] propertySelectors)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map Processes request to Processes entity
                var process = requestItems.Select(Mapper.Map<ProcessRequest, OperationProcess>).ToArray();
                var processJson = JsonSerializer.Serialize(process, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Processes data: {processJson}", "DEBUG");
                return await uow.OperationProcessRepository.BulkyUpdateAsync(process, propertySelectors);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save Processes : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<ProcessSupportResponse> GetSupportItemsAsync(bool includeDeleted) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Admin Dashboard Statistics", "INFO");

            try {

                ProcessSupportResponse response = new() {
                    Units = new(),
                    ProcessTypes = new(),
                    Responsibilities = new(),
                };

                // get process types
                var types = await uow.ProcessTypeRepository.GetAllAsync(false);

                //..get department details
                var operationsDept = await uow.DepartmentRepository.GetAllAsync(d => 
                    d.DepartmentName.Contains("Operations"),
                    false, d => d.Units, d => d.Responsibilities);
                

                //..get operation department details
                if (operationsDept !=null && operationsDept.Count > 0)
                {
                    //operation department
                    var dept = operationsDept.FirstOrDefault();

                    //..operations owners
                    var owners = dept.Responsibilities;
                    if(owners != null && owners.Count > 0) {
                        response.Responsibilities.AddRange(
                            from owner in owners
                            select new ResponsibilityItemResponse
                            {
                                Id = owner.Id,
                                DepartmentName = dept.DepartmentName,
                                ResponsibilityRole = owner.ContactPosition
                            });
                        Logger.LogActivity($"Operations Department Responsibilities found: {owners.Count}", "DEBUG");
                    }

                    //operations units
                    var opsDeptUnits = dept.Units;
                    if(opsDeptUnits != null && opsDeptUnits.Count > 0) {
                        response.Units.AddRange(
                            from unit in opsDeptUnits
                            select new UnitItemResponse
                            {
                                Id = unit.Id,
                                UnitName = unit.UnitName
                            });
                        Logger.LogActivity($"Operations Department Units found: {opsDeptUnits.Count}", "DEBUG");
                    }
                }

                if (types != null && types.Count > 0) {
                    response.ProcessTypes.AddRange(
                        from type in types
                        select new ProcessTypeItemResponse
                        {
                            Id = type.Id,
                            TypeName = type.TypeName
                        });
                    Logger.LogActivity($"Department units found: {types.Count}", "DEBUG");
                }

                return response;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Process support items: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<OperationProcess>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<OperationProcess, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Processes", "INFO");

            try
            {
                return await uow.OperationProcessRepository.PageAllAsync(page, size, includeDeleted, null, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Processes: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<OperationProcess>> PageNewProcessesAsync(int page, int size, bool includeDeleted, params Expression<Func<OperationProcess, object>>[] includes) {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Retrieve paged Processes", "INFO");
            try {
                var result = await uow.OperationProcessRepository.PageAllAsync(page,
                    size,includeDeleted, p => p.ProcessStatus == "DRAFT", 
                    includes);

                return result;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Processes: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<OperationProcess>> PageReviewProcessesAsync(int page, int size, bool includeDeleted, params Expression<Func<OperationProcess, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Retrieve paged Processes", "INFO");
            try
            {
                var result = await uow.OperationProcessRepository.PageAllAsync(page,
                    size, includeDeleted, p => p.ProcessStatus == "INREVIEW",
                    includes);

                return result;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Processes: {ex.Message}", "ERROR");
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<OperationProcess>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<OperationProcess, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Processes", "INFO");

            try
            {
                return await uow.OperationProcessRepository.PageAllAsync(token, page, size, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Processes : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<OperationProcess>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<OperationProcess, bool>> predicate = null)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Processes", "INFO");

            try
            {
                return await uow.OperationProcessRepository.PageAllAsync(page, size, includeDeleted, predicate);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Processes: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<PagedResult<OperationProcess>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<OperationProcess, bool>> predicate = null, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Processes", "INFO");

            try
            {
                return await uow.OperationProcessRepository.PageAllAsync(token, page, size, predicate, includeDeleted);
            }
            catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve Processes : {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> InitiateReviewAsync(InitiateRequest request) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Initiate Process review", "INFO");
            try {
                var process = await uow.OperationProcessRepository.GetAsync(a => a.Id == request.Id);
                if (process != null) {
                    process.ProcessStatus = (request.ProcessStatus ?? string.Empty).Trim();
                    process.UnlockReason = (request.UnlockReason ?? string.Empty).Trim();
                    process.Comments = "Process review initiated";
                    process.IsLockProcess = false;
                    process.IsDeleted = false;
                    process.LastModifiedOn = request.ModifiedOn;
                    process.LastModifiedBy = (request.ModifiedBy ?? string.Empty).Trim();

                    //..add new approval record
                    process.Approvals.Add(new ProcessApproval() {
                        ProcessId = request.Id,
                        RequestDate = DateTime.Now,
                        HeadOfDepartmentStart = DateTime.Now,
                        HeadOfDepartmentStatus = "PENDING",
                        CreatedBy = request.ModifiedBy,
                        CreatedOn = request.ModifiedOn,
                        LastModifiedBy = request.ModifiedBy,
                        LastModifiedOn = request.ModifiedOn,
                    });

                    //..check entity state
                    _ = await uow.OperationProcessRepository.UpdateAsync(process, true);
                    var entityState = ((UnitOfWork)uow).Context.Entry(process).State;
                    Logger.LogActivity($"Operation Process state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to update Operation Process record: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<bool> HoldProcessReviewAsync(HoldRequest request) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Hold Process review", "INFO");
            try {
                var approval = await uow.ProcessApprovalRepository.GetAsync(p => p.Id == request.Id, false, p => p.Process );
                if (approval != null) {
                    approval.HeadOfDepartmentStatus = (request.ProcessStatus ?? string.Empty).Trim();
                    approval.HeadOfDepartmentStart = DateTime.Now;
                    approval.HeadOfDepartmentEnd = DateTime.Now;
                    approval.HeadOfDepartmentComment = (request.HoldReason ?? string.Empty).Trim();
                    approval.ComplianceStatus = (request.ProcessStatus ?? string.Empty).Trim();
                    approval.ComplianceStart = DateTime.Now;
                    approval.ComplianceEnd = DateTime.Now;
                    approval.ComplianceComment = (request.HoldReason ?? string.Empty).Trim();
                    approval.RiskStatus = (request.ProcessStatus ?? string.Empty).Trim();
                    approval.RiskStart = DateTime.Now;
                    approval.RiskEnd = DateTime.Now;
                    approval.RiskComment = (request.HoldReason ?? string.Empty).Trim();
                    approval.IsDeleted = false;
                    approval.LastModifiedOn = request.ModifiedOn;
                    approval.LastModifiedBy = (request.ModifiedBy ?? string.Empty).Trim();

                    if(approval.Process != null) {
                        approval.Process.ProcessStatus = (request.HoldReason ?? string.Empty).Trim();
                        approval.Process.ReasonOnhold = (request.HoldReason ?? string.Empty).Trim();
                        approval.Process.LastModifiedOn = request.ModifiedOn;
                        approval.Process.LastModifiedBy = (request.ModifiedBy ?? string.Empty).Trim();
                    }

                    //..check entity state
                    _ = await uow.ProcessApprovalRepository.UpdateAsync(approval, true);
                    var entityState = ((UnitOfWork)uow).Context.Entry(approval).State;
                    Logger.LogActivity($"Approval Process state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to update Operation Process record: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        public async Task<(bool, string, long)> RequestApprovalAsync(long recordId, string requestedBy) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Initiate Process review", "INFO");
            try {
                var process = await uow.OperationProcessRepository.GetAsync(a => a.Id == recordId, false, a => a.Unit);
                if (process != null) {
                    process.ProcessStatus = "PROPOSED";
                    process.Comments = $"Newly proposed process for {process.Unit?.UnitName??string.Empty} unit";
                    process.IsLockProcess = false;
                    process.IsDeleted = false;
                    process.LastModifiedOn = DateTime.Now;
                    process.LastModifiedBy = (requestedBy??string.Empty).Trim();

                    //..add new approval record
                    process.Approvals.Add(new ProcessApproval() {
                        ProcessId = recordId,
                        RequestDate = DateTime.Now,
                        HeadOfDepartmentStart = DateTime.Now,
                        HeadOfDepartmentStatus = "PENDING",
                        CreatedBy = (requestedBy ?? string.Empty).Trim(),
                        CreatedOn = DateTime.Now,
                        LastModifiedBy = (requestedBy ?? string.Empty).Trim(),
                        LastModifiedOn = DateTime.Now,
                    });

                    //..check entity state
                    _ = await uow.OperationProcessRepository.UpdateAsync(process, true);
                    var entityState = ((UnitOfWork)uow).Context.Entry(process).State;
                    Logger.LogActivity($"Operation Process state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return (result > 0, process.ProcessName, process.Id);
                }

                return (false, string.Empty, recordId);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to update Operation Process record: {ex.Message}", "ERROR");
                //..save error object to the database
                _ = await uow.SystemErrorRespository.InsertAsync(HandleError(uow, ex));
                throw;
            }
        }

        #endregion

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
                ErrorSource = "OPERATION-PROCESSES-SERVICE",
                StackTrace = ex.StackTrace,
                Severity = "CRITICAL",
                ReportedOn = DateTime.Now,
                CompanyId = companyId,
                CreatedBy = "SYSTEM",
                CreatedOn = DateTime.Now
            };

        }

        private static ServiceOperationUnit AdjustedUnitName(string unitName) {
            unitName = unitName?.ToLower() ?? "";

            if (unitName.Contains("account")) return ServiceOperationUnit.AccountServices;
            if (unitName.Contains("cash")) return ServiceOperationUnit.Cash;
            if (unitName.Contains("channel")) return ServiceOperationUnit.Channels;
            if (unitName.Contains("customer")) return ServiceOperationUnit.CustomerExp;
            if (unitName.Contains("reconciliation")) return ServiceOperationUnit.Reconciliation;
            if (unitName.Contains("records")) return ServiceOperationUnit.RecordsMgt;
            if (unitName.Contains("payment")) return ServiceOperationUnit.Payments;
            if (unitName.Contains("wallet")) return ServiceOperationUnit.Wallets;

            return ServiceOperationUnit.Unknown;
        }

        private static ServiceProcessCategories AdjustedStatus(string status) {

            if (string.IsNullOrWhiteSpace(status))
                return ServiceProcessCategories.Proposed;

            //..review special case first
            if (status.Equals("INREVIEW", StringComparison.OrdinalIgnoreCase))
                return ServiceProcessCategories.Review;

            //..try to match by description
            foreach (var value in Enum.GetValues<ServiceProcessCategories>()) {
                if (value.GetDescription().Equals(status, StringComparison.OrdinalIgnoreCase))
                    return value;
            }

            //..try to match by enum name as fallback
            if (Enum.TryParse<ServiceProcessCategories>(status, true, out var result))
                return result;

            //..default to Proposed if no match
            return ServiceProcessCategories.Proposed;
        }

        private static readonly Lazy<Dictionary<string, string>> CategoryMap = new(() =>
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
                ["Draft"] = ServiceProcessCategories.Draft.ToString().ToUpper(),
                ["UpToDate"] = ServiceProcessCategories.UpToDate.ToString().ToUpper(),
                ["Unchanged"] = ServiceProcessCategories.Unchanged.ToString().ToUpper(),
                ["Unclassified"] = ServiceProcessCategories.Proposed.ToString().ToUpper(),
                ["Obsolete"] = ServiceProcessCategories.Due.ToString().ToUpper(),
                ["Cancelled"] = ServiceProcessCategories.Cancelled.ToString().ToUpper(),
                ["Dormant"] = ServiceProcessCategories.Dormant.ToString().ToUpper(),
                ["On Hold"] = ServiceProcessCategories.OnHold.ToString().ToUpper(),
                ["Review"] = "INREVIEW"
            });

        private static string GetDescriptionFromCategory(string category) {
            return CategoryMap.Value.TryGetValue(category, out var result) ? result : "Unknown";
        }


        #endregion

    }
}

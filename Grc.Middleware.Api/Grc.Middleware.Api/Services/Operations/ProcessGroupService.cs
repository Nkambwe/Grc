using AutoMapper;
using Azure;
using Grc.Middleware.Api.Data.Containers;
using Grc.Middleware.Api.Data.Entities.Operations.Processes;
using Grc.Middleware.Api.Data.Entities.Support;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Enums;
using Grc.Middleware.Api.Helpers;
using Grc.Middleware.Api.Http.Requests;
using Grc.Middleware.Api.Utils;
using System.Linq.Expressions;
using System.Security;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Services.Operations {
    public class ProcessGroupService : BaseService, IProcessGroupService
    {
        public ProcessGroupService(IServiceLoggerFactory loggerFactory,
            IUnitOfWorkFactory uowFactory,
            IMapper mapper) : base(loggerFactory, uowFactory, mapper) {
        }

        public int Count()
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Process Groups in the database", "INFO");

            try
            {
                return uow.ProcessGroupRepository.Count();
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to Process Groups in the database: {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public int Count(Expression<Func<ProcessGroup, bool>> predicate)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Process Groups in the database", "INFO");

            try
            {
                return uow.ProcessGroupRepository.Count(predicate);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Process Groups in the database: {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Process Groups in the database", "INFO");

            try
            {
                return await uow.ProcessGroupRepository.CountAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Process Groups in the database: {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<int> CountAsync(bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of  Process Groups in the database", "INFO");

            try
            {
                return await uow.StatutoryArticleRepository.CountAsync(excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Process Groups in the database: {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<int> CountAsync(Expression<Func<ProcessGroup, bool>> predicate, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Process Groups in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.ProcessGroupRepository.CountAsync(predicate, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Process Groups in the database: {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<int> CountAsync(Expression<Func<ProcessGroup, bool>> predicate, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Count number of Process Groups in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.ProcessGroupRepository.CountAsync(predicate, excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to count Process Groups in the database: {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public bool Exists(Expression<Func<ProcessGroup, bool>> predicate, bool excludeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an Process Groups exists in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.ProcessGroupRepository.Exists(predicate, excludeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for Process Groups in the database: {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<ProcessGroup, bool>> predicate, bool excludeDeleted = false, CancellationToken token = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check if an Process Groups exists in the database that fit predicate >> '{predicate}'", "INFO");

            try
            {
                return await uow.ProcessGroupRepository.ExistsAsync(predicate, excludeDeleted, token);
            }
            catch (Exception ex)
            {
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<Dictionary<string, bool>> ExistsBatchAsync(Dictionary<string, Expression<Func<ProcessGroup, bool>>> predicates, bool excludeDeleted = true, CancellationToken cancellationToken = default)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Check for batch Process Groups if they exist in the database that fit predicate >> '{predicates}'", "INFO");

            try
            {
                return await uow.ProcessGroupRepository.ExistsBatchAsync(predicates, excludeDeleted, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to check for Process Groups in the database: {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public ProcessGroup Get(long id, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Process Group with ID '{id}'", "INFO");

            try
            {
                return uow.ProcessGroupRepository.Get(id, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Process Group: {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public ProcessGroup Get(Expression<Func<ProcessGroup, bool>> predicate, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Process Group that fits predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.ProcessGroupRepository.Get(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Process Group : {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public ProcessGroup Get(Expression<Func<ProcessGroup, bool>> predicate, bool includeDeleted = false, params Expression<Func<ProcessGroup, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Process Groups that fits predicate >> '{predicate}'", "INFO");

            try
            {
                return uow.ProcessGroupRepository.Get(predicate, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Process Groups : {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public IQueryable<ProcessGroup> GetAll(bool includeDeleted = false, params Expression<Func<ProcessGroup, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Process Groups", "INFO");

            try
            {
                return uow.ProcessGroupRepository.GetAll(includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Process Groups : {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public IList<ProcessGroup> GetAll(bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Process Groups", "INFO");

            try
            {
                return uow.ProcessGroupRepository.GetAll(includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Process Groups : {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public IList<ProcessGroup> GetAll(Expression<Func<ProcessGroup, bool>> predicate, bool includeDeleted)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Process Groups that fit predicate '{predicate}'", "INFO");

            try
            {
                return uow.ProcessGroupRepository.GetAll(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Process Groups : {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public async Task<IList<ProcessGroup>> GetAllAsync(bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Process Groups", "INFO");

            try
            {
                return await uow.ProcessGroupRepository.GetAllAsync(includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Process Groups : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<IList<ProcessGroup>> GetAllAsync(Expression<Func<ProcessGroup, bool>> predicate, bool includeDeleted)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Process Groups that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.ProcessGroupRepository.GetAllAsync(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Process Groups : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<IList<ProcessGroup>> GetAllAsync(Expression<Func<ProcessGroup, bool>> predicate, bool includeDeleted = false, params Expression<Func<ProcessGroup, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get all Process Groups that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.ProcessGroupRepository.GetAllAsync(predicate, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Process Groups : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<IList<ProcessGroup>> GetAllAsync(bool includeDeleted = false, params Expression<Func<ProcessGroup, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity("Get all Process Groups", "INFO");

            try
            {
                return await uow.ProcessGroupRepository.GetAllAsync(includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Process Groups : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<ProcessGroup> GetAsync(long id, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Process Group with ID '{id}'", "INFO");

            try
            {
                return await uow.ProcessGroupRepository.GetAsync(id, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Process Group : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<ProcessGroup> GetAsync(Expression<Func<ProcessGroup, bool>> predicate, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Process Group that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.ProcessGroupRepository.GetAsync(predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Process Group : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<ProcessGroup> GetAsync(Expression<Func<ProcessGroup, bool>> predicate, bool includeDeleted = false, params Expression<Func<ProcessGroup, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get Process Group that fit predicate '{predicate}'", "INFO");

            try
            {
                return await uow.ProcessGroupRepository.GetAsync(predicate, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Process Group : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<IList<ProcessGroup>> GetTopAsync(Expression<Func<ProcessGroup, bool>> predicate, int top, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Get top {top} Process Groups that fit predicate >> {predicate}", "INFO");

            try
            {
                return await uow.ProcessGroupRepository.GetTopAsync(predicate, top, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Process Groups : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public bool Insert(ProcessGroupRequest request)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map Process Group request to Process Group entity
                var group = Mapper.Map<ProcessGroupRequest, ProcessGroup>(request);

                //..log the Process Group data being saved
                var groupJson = JsonSerializer.Serialize(group, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Process Group data: {groupJson}", "DEBUG");

                //..link processes if provided
                if (request.Processes != null && request.Processes.Any())
                {
                    var processes = uow.OperationProcessRepository.GetAll(false, p => request.Processes.Contains(p.Id));
                    foreach (var process in processes)
                    {
                        group.Processes.Add(new ProcessProcessGroup
                        {
                            Process = process,
                            Group = group
                        });
                    }
                }

                var added = uow.ProcessGroupRepository.Insert(group);
                if (added)
                {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(group).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save Process Group : {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public async Task<bool> InsertAsync(ProcessGroupRequest request) {
            using var uow = UowFactory.Create();
            try {
                //..map process group request to group entity
                var group = new ProcessGroup()
                {
                    GroupName = request.GroupName,
                    Description = request.GroupDescription,
                    IsDeleted = request.IsDeleted,
                    CreatedOn = request.CreatedOn,
                    CreatedBy = request.CreatedBy,
                    LastModifiedOn = request.ModifiedOn,
                    LastModifiedBy = request.ModifiedBy
                };

                //..log the Process Group data being saved
                var groupJson = JsonSerializer.Serialize(group, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Process Group data: {groupJson}", "DEBUG");

                //..link processes if provided
                if (request.Processes != null && request.Processes.Any()) {
                    var processes = await uow.OperationProcessRepository.GetAllAsync(p => request.Processes.Contains(p.Id));
                    foreach (var process in processes) {
                        group.Processes.Add(new ProcessProcessGroup {
                            Process = process,
                            Group = group
                        });
                    }
                }

                var added = await uow.ProcessGroupRepository.InsertAsync(group);
                if (added) {
                    //..check object state
                    var entityState = ((UnitOfWork)uow).Context.Entry(group).State;
                    Logger.LogActivity($"Entity state after insert: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save Process Group : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public bool Update(ProcessGroupRequest request, bool includeDeleted = false) {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update Process Group request", "INFO");

            try
            {
                var group = uow.ProcessGroupRepository.Get(a => a.Id == request.Id, includeDeleted, g => g.Processes);
                if (group != null)
                {
                    var existingIds = group.Processes.Select(p => p.ProcessId).ToList();
                    var (toAdd, toRemove) = GetChanges(existingIds, request.Processes);

                    //..remove outdated links
                    group.Processes = group.Processes.Where(p => !toRemove.Contains(p.ProcessId)).ToList();

                    //..add new links
                    foreach (var processId in toAdd)
                    {
                        group.Processes.Add(new ProcessProcessGroup {
                            GroupId = group.Id,
                            ProcessId = processId
                        });
                    }

                    //..update Process Group record
                    group.GroupName = (request.GroupName ?? string.Empty).Trim();
                    group.Description = (request.GroupDescription ?? string.Empty).Trim();
                    group.IsDeleted = request.IsDeleted;
                    group.LastModifiedOn = DateTime.Now;
                    group.LastModifiedBy = $"{request.UserId}";

                    //..check entity state
                    _ = uow.ProcessGroupRepository.Update(group, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(group).State;
                    Logger.LogActivity($"Process Group state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to update Process Group record: {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(ProcessGroupRequest request, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Update Process Group", "INFO");

            try
            {
                var group = await uow.ProcessGroupRepository.GetAsync(a => a.Id == request.Id, includeDeleted, g => g.Processes);
                if (group != null) {
                    var existingIds = group.Processes.Select(p => p.ProcessId).ToList();
                    var (toAdd, toRemove) = GetChanges(existingIds, request.Processes);

                    //..remove outdated links
                    group.Processes = group.Processes.Where(p => !toRemove.Contains(p.ProcessId)).ToList();

                    //..add new links
                    foreach (var processId in toAdd) {
                        group.Processes.Add(new ProcessProcessGroup {
                            GroupId = group.Id,
                            ProcessId = processId
                        });
                    }

                    //..update Process Group record
                    group.GroupName = (request.GroupName ?? string.Empty).Trim();
                    group.Description = (request.GroupDescription ?? string.Empty).Trim();
                    group.IsDeleted = request.IsDeleted;
                    group.LastModifiedOn = request.ModifiedOn;
                    group.LastModifiedBy = $"{(request.ModifiedBy ?? string.Empty).Trim()}";

                    //..check entity state
                    _ = await uow.ProcessGroupRepository.UpdateAsync(group, includeDeleted);
                    var entityState = ((UnitOfWork)uow).Context.Entry(group).State;
                    Logger.LogActivity($"Process Group state after Update: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to update Process Group record: {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public bool Delete(IdRequest request)
        {
            using var uow = UowFactory.Create();
            try
            {
                var groupJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Process Group data: {groupJson}", "DEBUG");

                var groups = uow.ProcessGroupRepository.Get(t => t.Id == request.RecordId);
                if (groups != null)
                {
                    //..mark as delete this Process Group
                    _ = uow.ProcessGroupRepository.Delete(groups, request.MarkAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(groups).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = uow.SaveChanges();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete Process Group : {ex.Message}", "ERROR");
                LogError(uow, ex);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(IdRequest request)
        {

            using var uow = UowFactory.Create();
            try
            {
                var groupJson = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Process Group data: {groupJson}", "DEBUG");

                var group = await uow.ProcessGroupRepository.GetAsync(t => t.Id == request.RecordId);
                if (group != null)
                {
                    //..mark as delete this Process Group
                    _ = await uow.ProcessGroupRepository.DeleteAsync(group, request.MarkAsDeleted);

                    //..check entity state
                    var entityState = ((UnitOfWork)uow).Context.Entry(group).State;
                    Logger.LogActivity($"Entity state after deletion: {entityState}", "DEBUG");

                    var result = await uow.SaveChangesAsync();
                    Logger.LogActivity($"SaveChanges result: {result}", "DEBUG");
                    return result > 0;
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete Process Group : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<bool> DeleteAllAsync(IList<long> requestIds, bool markAsDeleted = false)
        {
            using var uow = UowFactory.Create();
            try
            {
                var groups = await uow.ProcessGroupRepository.GetAllAsync(e => requestIds.Contains(e.Id));
                if (groups.Count == 0)
                {
                    Logger.LogActivity($"Records not found", "INFO");
                    return false;
                }
                return await uow.ProcessGroupRepository.DeleteAllAsync(groups, markAsDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to delete Process Groups: {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }

        }

        public async Task<bool> BulkyInsertAsync(ProcessGroupRequest[] requestItems)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map Process Groups to Process Groups entity
                var groups = requestItems.Select(Mapper.Map<ProcessGroupRequest, ProcessGroup>).ToArray();
                var groupJson = JsonSerializer.Serialize(groups, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Process Groups data: {groupJson}", "DEBUG");
                return await uow.ProcessGroupRepository.BulkyInsertAsync(groups);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save Process Groups : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<bool> BulkyUpdateAsync(ProcessGroupRequest[] requestItems)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map Process Groups request to Process Groups entity
                var groups = requestItems.Select(Mapper.Map<ProcessGroupRequest, ProcessGroup>).ToArray();
                var groupJson = JsonSerializer.Serialize(groups, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Process Groups data: {groupJson}", "DEBUG");
                return await uow.ProcessGroupRepository.BulkyUpdateAsync(groups);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save Process Groups : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<bool> BulkyUpdateAsync(ProcessGroupRequest[] requestItems, params Expression<Func<ProcessGroup, object>>[] propertySelectors)
        {
            using var uow = UowFactory.Create();
            try
            {
                //..map Process Groups request to Process Groups entity
                var groups = requestItems.Select(Mapper.Map<ProcessGroupRequest, ProcessGroup>).ToArray();
                var groupJson = JsonSerializer.Serialize(groups, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });
                Logger.LogActivity($"Process Groups data: {groupJson}", "DEBUG");
                return await uow.ProcessGroupRepository.BulkyUpdateAsync(groups, propertySelectors);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to save Process Groups : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<PagedResult<ProcessGroup>> PageAllAsync(int page, int size, bool includeDeleted, params Expression<Func<ProcessGroup, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Process Groups", "INFO");

            try
            {
                return await uow.ProcessGroupRepository.PageAllAsync(page, size, includeDeleted, null, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Process Groups: {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<PagedResult<ProcessGroup>> PageAllAsync(CancellationToken token, int page, int size, bool includeDeleted, params Expression<Func<ProcessGroup, object>>[] includes)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Process Groups", "INFO");

            try
            {
                return await uow.ProcessGroupRepository.PageAllAsync(token, page, size, includeDeleted, includes);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Process Groups : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<PagedResult<ProcessGroup>> PageAllAsync(int page, int size, bool includeDeleted, Expression<Func<ProcessGroup, bool>> predicate = null)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Process Groups", "INFO");

            try
            {
                return await uow.ProcessGroupRepository.PageAllAsync(page, size, includeDeleted, predicate);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Process Groups: {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        public async Task<PagedResult<ProcessGroup>> PageAllAsync(CancellationToken token, int page, int size, Expression<Func<ProcessGroup, bool>> predicate = null, bool includeDeleted = false)
        {
            using var uow = UowFactory.Create();
            Logger.LogActivity($"Retrieve paged Process Groups", "INFO");

            try
            {
                return await uow.ProcessGroupRepository.PageAllAsync(token, page, size, predicate, includeDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Failed to retrieve Process Groups : {ex.Message}", "ERROR");
                await LogErrorAsync(uow, ex);
                throw;
            }
        }

        #region private methods

        private void LogError(IUnitOfWork uow, Exception ex)
        {

            //..log inner exceptions here too
            var innerEx = ex.InnerException;
            while (innerEx != null)
            {
                Logger.LogActivity($"Service Inner Exception: {innerEx.Message}", "ERROR");
                innerEx = innerEx.InnerException;
            }

            var company = uow.CompanyRepository.GetAll(false).FirstOrDefault();
            long companyId = company != null ? company.Id : 1;
            SystemError errorObj = new()
            {
                ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                ErrorSource = "PROCESS-GROUPS-SERVICE",
                StackTrace = ex.StackTrace,
                Severity = "CRITICAL",
                ReportedOn = DateTime.Now,
                CompanyId = companyId
            };
            uow.SystemErrorRespository.Insert(errorObj);
            uow.SaveChanges();
        }

        private async Task LogErrorAsync(IUnitOfWork uow, Exception ex)
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
            SystemError errorObj = new()
            {
                ErrorMessage = innerEx != null ? innerEx.Message : ex.Message,
                ErrorSource = "PROCESS-GROUPS-SERVICE",
                StackTrace = ex.StackTrace,
                Severity = "CRITICAL",
                ReportedOn = DateTime.Now,
                CompanyId = companyId
            };

            //..save error object to the database
            uow.SystemErrorRespository.Insert(errorObj);
            await uow.SaveChangesAsync();
        }

        private static (List<long> toAdd, List<long> toRemove) GetChanges(IEnumerable<long> existingIds, IEnumerable<long> newIds)
        {
            var existingSet = new HashSet<long>(existingIds);
            var newSet = new HashSet<long>(newIds);

            var toAdd = newSet.Except(existingSet).ToList();
            var toRemove = existingSet.Except(newSet).ToList();

            return (toAdd, toRemove);
        }

        #endregion

    }
}

using AutoMapper;
using DocumentFormat.OpenXml.Office2016.Excel;
using Grc.ui.App.Factories;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Utils;
using System.Linq.Dynamic.Core;

namespace Grc.ui.App.Services {
    public class RegulatonAuthorityService : GrcBaseService, IRegulatonAuthorityService
    {
        private IQueryable<RegulatoryAuthorityResponse> query = new List<RegulatoryAuthorityResponse> {
                new() { Id = 1, AuthorityName = "First Type",  AuthorityAlias="T-01",CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 2, AuthorityName = "Second Type", AuthorityAlias="T-02",CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 3, AuthorityName = "Third Type",  AuthorityAlias="T-03",CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 4, AuthorityName = "Fourth Type", AuthorityAlias="T-04",CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 5, AuthorityName = "Fifth Type",  AuthorityAlias="T-05",CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 6, AuthorityName = "Sixth Type",  AuthorityAlias="T-06",CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 7, AuthorityName = "Seventh Type",AuthorityAlias="T-07",CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 8, AuthorityName = "Eighth Type", AuthorityAlias="T-08",CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 9, AuthorityName = "Nineth Type", AuthorityAlias="T-09",CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 10, AuthorityName = "Tenth Type",       AuthorityAlias="T-10",CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 11, AuthorityName = "Eleventh Type",    AuthorityAlias="T-11",CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 12, AuthorityName = "Twelveth Type",    AuthorityAlias="T-12",CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 13, AuthorityName = "Thirteenth Type",  AuthorityAlias="T-13",CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 14, AuthorityName = "Fourteenth Type",  AuthorityAlias="T-14",CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 15, AuthorityName = "Fifteenth Type",   AuthorityAlias="T-15",CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 16, AuthorityName = "Seventeenth Type", AuthorityAlias="T-16",CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 17, AuthorityName = "Mobile Type",      AuthorityAlias="T-17",CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 18, AuthorityName = "Investment Type",  AuthorityAlias="T-18",CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 19, AuthorityName = "Business Type",    AuthorityAlias="T-19",CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 20, AuthorityName = "Trade Type",       AuthorityAlias="T-20",CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 21, AuthorityName = "Banking Type",     AuthorityAlias="T-21",CreatedAt = DateTime.Now,IsDeleted = true }
            }.AsQueryable();

        public RegulatonAuthorityService(IApplicationLoggerFactory loggerFactory, 
            IHttpHandler httpHandler, 
            IEnvironmentProvider environment, 
            IEndpointTypeProvider endpointType, 
            IMapper mapper, 
            IWebHelper webHelper, 
            SessionManager sessionManager, 
            IGrcErrorFactory errorFactory, 
            IErrorService errorService) : base(loggerFactory, httpHandler, 
                environment, endpointType, mapper, 
                webHelper, sessionManager, 
                errorFactory, errorService) {
        }

        public async Task<GrcResponse<RegulatoryAuthorityResponse>> GetAuthorityAsync(GrcIdRequst getRequest) {
            return await Task.FromResult(new GrcResponse<RegulatoryAuthorityResponse>(query.FirstOrDefault()));
        }

        public async Task<GrcResponse<PagedResponse<RegulatoryAuthorityResponse>>> GetAllRegulatoryAuthorities(TableListRequest request)
        {
            //..filter data
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var lookUp = request.SearchTerm.ToLower();
                query = query.Where(a =>
                    (a.AuthorityName != null && a.AuthorityName.ToLower().Contains(lookUp)) ||
                    (a.AuthorityAlias != null && a.AuthorityAlias.ToLower().Contains(lookUp))
                );
            }

            //..apply sorting
            if (!string.IsNullOrEmpty(request.SortBy)) {
                var sortExpr = $"{request.SortBy} {(request.SortDirection == "Ascending" ? "asc" : "desc")}";
                query = query.OrderBy(sortExpr);
            }

            var page = new PagedResponse<RegulatoryAuthorityResponse>() {
                TotalCount = 20,
                Page = request.PageIndex,
                Size = request.PageSize,
                Entities = query.ToList(),
                TotalPages = 2
            };
            return await Task.FromResult(new GrcResponse<PagedResponse<RegulatoryAuthorityResponse>>(page));
        }

        public async Task<GrcResponse<RegulatoryAuthorityResponse>> CreateAuthorityAsync(RegulatoryAuthorityRequest request)
        {
            var record = new RegulatoryAuthorityResponse
            {
                AuthorityAlias = request.AuthorityAlias,
                AuthorityName = request.AuthorityName,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsDeleted = false,
                Id = query.Max(r => r.Id) + 1
            };
            return await Task.FromResult(new GrcResponse<RegulatoryAuthorityResponse>(record));
        }

        public async Task<GrcResponse<RegulatoryAuthorityResponse>> UpdateAuthorityAsync(RegulatoryAuthorityRequest request) {

            var record = query.FirstOrDefault(r => r.Id == request.Id);

            if (record == null)
            {
                return await Task.FromResult(new GrcResponse<RegulatoryAuthorityResponse>(record));
            }

            record.AuthorityAlias = request.AuthorityAlias;
            record.AuthorityName = request.AuthorityName;
            record.IsDeleted = request.Status == "Inactive";
            record.UpdatedAt = DateTime.Now.AddDays(-2);

            return await Task.FromResult(new GrcResponse<RegulatoryAuthorityResponse>(record));
        }

        public async Task<GrcResponse<ServiceResponse>> DeleteAuthorityAsync(GrcIdRequst deleteRequest) {
            var record = query.FirstOrDefault(r => r.Id == deleteRequest.RecordId);
            if (record == null) {
                return await Task.FromResult(new GrcResponse<ServiceResponse>(new ServiceResponse {
                    Status = false,
                    StatusCode = 404,
                    Message = "Authority not found"
                }));
            }
            return await Task.FromResult(new GrcResponse<ServiceResponse>(new ServiceResponse
            {
                Status = true,
                StatusCode = 200,
                Message = "Authority deleted successfully"
            }));
        }

    }
}

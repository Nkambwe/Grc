using AutoMapper;
using Grc.ui.App.Factories;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Utils;
using System.Linq.Dynamic.Core;

namespace Grc.ui.App.Services {
    public class ResponsibilityService : GrcBaseService, IResponsibilityService {

        private IQueryable<OwnerResponse> query = new List<OwnerResponse>
        {
            new(){Id=1,Name="James Kasozi", Email="james.kasozi@postbank.co.ug", Position="Head of department"},
            new(){Id=2,Name="Ssemanda Insima Moses", Email="ssemanda.moses@postbank.co.ug", Position="Manager"},
            new(){Id=3,Name="Stephen Mbalu", Email="stephen.mbalu@postbank.co.ug", Position="Managert"}
        }.AsQueryable();

        public ResponsibilityService(IApplicationLoggerFactory loggerFactory, 
            IHttpHandler httpHandler, 
            IEnvironmentProvider environment, 
            IEndpointTypeProvider endpointType, 
            IMapper mapper, IWebHelper webHelper, 
            SessionManager sessionManager, 
            IGrcErrorFactory errorFactory, 
            IErrorService errorService) 
            : base(loggerFactory, httpHandler, environment, endpointType, 
                  mapper, webHelper, sessionManager, errorFactory, errorService) {
        }

        public async Task<GrcResponse<OwnerResponse>> GetOwnerAsync(GrcIdRequest getRequest)
        {
            return await Task.FromResult(new GrcResponse<OwnerResponse>(query.FirstOrDefault()));
        }

        public async Task<GrcResponse<List<OwnerResponse>>> GetAllAsync(GrcRequest getRequest)
        {
            return await Task.FromResult(new GrcResponse<List<OwnerResponse>>(query.ToList()));
        }

        public async Task<GrcResponse<PagedResponse<OwnerResponse>>> GetAllOwners(TableListRequest request) {
            if (!string.IsNullOrWhiteSpace(request.SearchTerm)) {
                var lookUp = request.SearchTerm.ToLower();
                query = query.Where(a =>
                    (a.Name != null && a.Name.ToLower().Contains(lookUp))||
                    (a.Phone != null && a.Phone.ToLower().Contains(lookUp)) ||
                    (a.Email != null && a.Email.ToLower().Contains(lookUp))
                );
            }

            if (!string.IsNullOrEmpty(request.SortBy)) {
                var sortExpr = $"{request.SortBy} {(request.SortDirection == "Ascending" ? "asc" : "desc")}";
                query = query.OrderBy(sortExpr);
            }

            var page = new PagedResponse<OwnerResponse>()
            {
                TotalCount = 20,
                Page = request.PageIndex,
                Size = request.PageSize,
                Entities = query.ToList(),
                TotalPages = 2
            };
            return await Task.FromResult(new GrcResponse<PagedResponse<OwnerResponse>>(page));
        }

        public async Task<GrcResponse<OwnerResponse>> CreateOwnerAsync(OwnerViewModel request) {
            var record = new OwnerResponse {
                Name = request.Name,
                Phone = request.Phone,
                Email = request.Email,
                Position = request.Position,
                Comment = request.Comment,
                DepartmentId = request.DepartmentId,
                Department = "IT",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsDeleted = false,
                Id = query.Max(r => r.Id) + 1
            };
            return await Task.FromResult(new GrcResponse<OwnerResponse>(record));
        }

        public async Task<GrcResponse<OwnerResponse>> UpdateOwnerAsync(OwnerViewModel request) {
            var record = query.FirstOrDefault(r => r.Id == request.Id);

            if (record == null) {
                return await Task.FromResult(new GrcResponse<OwnerResponse>(record));
            }

            record.Name = request.Name;
            record.Phone = request.Phone;
            record.Email = request.Email;
            record.Position = request.Position;
            record.Comment = request.Comment;
            record.DepartmentId = request.DepartmentId;
            record.IsDeleted = request.IsDeleted;
            record.UpdatedAt = DateTime.Now.AddDays(-2);

            return await Task.FromResult(new GrcResponse<OwnerResponse>(record));
        }

        public async Task<GrcResponse<ServiceResponse>> DeleteOwnerAsync(GrcIdRequest deleteRequest)
        {
            var record = query.FirstOrDefault(r => r.Id == deleteRequest.RecordId);
            if (record == null)
            {
                return await Task.FromResult(new GrcResponse<ServiceResponse>(new ServiceResponse
                {
                    Status = false,
                    StatusCode = 404,
                    Message = "Document owner not found"
                }));
            }
            return await Task.FromResult(new GrcResponse<ServiceResponse>(new ServiceResponse
            {
                Status = true,
                StatusCode = 200,
                Message = "Document owner deleted successfully"
            }));
        }

    }
}

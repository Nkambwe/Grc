using AutoMapper;
using Grc.ui.App.Factories;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Utils;
using System.Linq.Dynamic.Core;

namespace Grc.ui.App.Services {
    public class DocumentTypeService : GrcBaseService, IDocumentTypeService
    {
        private IQueryable<DocumentTypeResponse> query = new List<DocumentTypeResponse> {
                new() { Id = 1, TypeName = "First Type", CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 2, TypeName = "Second Type", CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 3, TypeName = "Third Type", CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 4, TypeName = "Fourth Type", CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 5, TypeName = "Fifth Type", CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 6, TypeName = "Sixth Type", CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 7, TypeName = "Seventh Type", CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 8, TypeName = "Eighth Type", CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 9, TypeName = "Nineth Type", CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 10, TypeName = "Tenth Type", CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 11, TypeName = "Eleventh Type", CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 12, TypeName = "Twelveth Type", CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 13, TypeName = "Thirteenth Type", CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 14, TypeName = "Fourteenth Type", CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 15, TypeName = "Fifteenth Type", CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 16, TypeName = "Seventeenth Type", CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 17, TypeName = "Mobile Type", CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 18, TypeName = "Investment Type", CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 19, TypeName = "Business Type", CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 20, TypeName = "Trade Type", CreatedAt = DateTime.Now,IsDeleted = true},
                new() { Id = 21, TypeName = "Banking Type", CreatedAt = DateTime.Now,IsDeleted = true }
            }.AsQueryable();

        public DocumentTypeService(IApplicationLoggerFactory loggerFactory, 
            IHttpHandler httpHandler, IEnvironmentProvider environment, 
            IEndpointTypeProvider endpointType, IMapper mapper, 
            IWebHelper webHelper, SessionManager sessionManager, 
            IGrcErrorFactory errorFactory, IErrorService errorService) 
            : base(loggerFactory, httpHandler, environment, endpointType, mapper, 
                  webHelper, sessionManager, errorFactory, errorService) {
        }

        public async Task<GrcResponse<DocumentTypeResponse>> GetTypeAsync(GrcIdRequst getRequest)
        {
            return await Task.FromResult(new GrcResponse<DocumentTypeResponse>(query.FirstOrDefault()));
        }

        public async Task<GrcResponse<List<DocumentTypeResponse>>> GetAllAsync(GrcRequest getRequest) {
            return await Task.FromResult(new GrcResponse<List<DocumentTypeResponse>>(query.ToList()));
        }

        public async Task<GrcResponse<PagedResponse<DocumentTypeResponse>>> GetAllDocumentTypes(TableListRequest request)
        {
            //..filter data
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var lookUp = request.SearchTerm.ToLower();
                query = query.Where(a =>
                    (a.TypeName != null && a.TypeName.ToLower().Contains(lookUp))
                );
            }

            //..apply sorting
            if (!string.IsNullOrEmpty(request.SortBy))
            {
                var sortExpr = $"{request.SortBy} {(request.SortDirection == "Ascending" ? "asc" : "desc")}";
                query = query.OrderBy(sortExpr);
            }

            var page = new PagedResponse<DocumentTypeResponse>()
            {
                TotalCount = 20,
                Page = request.PageIndex,
                Size = request.PageSize,
                Entities = query.ToList(),
                TotalPages = 2
            };
            return await Task.FromResult(new GrcResponse<PagedResponse<DocumentTypeResponse>>(page));
        }

        public async Task<GrcResponse<DocumentTypeResponse>> CreateTypeAsync(DocumentTypeViewModel request)
        {
            var record = new DocumentTypeResponse
            {
                TypeName = request.TypeName,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsDeleted = false,
                Id = query.Max(r => r.Id) + 1
            };
            return await Task.FromResult(new GrcResponse<DocumentTypeResponse>(record));
        }

        public async Task<GrcResponse<DocumentTypeResponse>> UpdateTypeAsync(DocumentTypeViewModel request)
        {
            var record = query.FirstOrDefault(r => r.Id == request.Id);

            if (record == null)
            {
                return await Task.FromResult(new GrcResponse<DocumentTypeResponse>(record));
            }

            record.TypeName = request.TypeName;
            record.IsDeleted = request.Status == "Inactive";
            record.UpdatedAt = DateTime.Now.AddDays(-2);

            return await Task.FromResult(new GrcResponse<DocumentTypeResponse>(record));
        }

        public async Task<GrcResponse<ServiceResponse>> DeleteTypeAsync(GrcIdRequst deleteRequest)
        {
            var record = query.FirstOrDefault(r => r.Id == deleteRequest.RecordId);
            if (record == null)
            {
                return await Task.FromResult(new GrcResponse<ServiceResponse>(new ServiceResponse
                {
                    Status = false,
                    StatusCode = 404,
                    Message = "Document type not found"
                }));
            }
            return await Task.FromResult(new GrcResponse<ServiceResponse>(new ServiceResponse
            {
                Status = true,
                StatusCode = 200,
                Message = "Document type deleted successfully"
            }));
        }
    }
}

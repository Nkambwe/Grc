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
    public class PolicyService : GrcBaseService, IPolicyService {
        private IQueryable<PolicyRegisterResponse> query = new List<PolicyRegisterResponse>()
                {
                    new ()
                    {
                        Id=1,
                        DocumentName = "Occupational Health and Safety Policy",
                        DocumentType = "Policy",
                        IsAligned = false,
                        IsLocked = false,
                        ReviewStatus = "DUE",
                        LastRevisionDate = DateTime.Now.AddMonths(-8).ToString("dd-MM-yyyy"),
                        NextRevisionDate = DateTime.Now.AddMonths(-3).ToString("dd-MM-yyyy"),
                    },
                    new ()
                    {
                        Id=2,
                        DocumentName = "Administration Policy ",
                        DocumentType = "Policy",
                        IsAligned = true,
                        IsLocked = true,
                        ReviewStatus = "UPTODATE",
                        LastRevisionDate = DateTime.Now.AddMonths(-2).ToString("dd-MM-yyyy"),
                        NextRevisionDate = DateTime.Now.AddMonths(12).ToString("dd-MM-yyyy"),
                    },
                    new()
                    {
                        Id=3,
                        DocumentName = "Agent Banking Policy",
                        DocumentType = "Policy",
                        IsAligned = false,
                        IsLocked = false,
                        ReviewStatus = "DUE",
                        LastRevisionDate = DateTime.Now.AddMonths(-11).ToString("dd-MM-yyyy"),
                        NextRevisionDate = DateTime.Now.AddMonths(-12).ToString("dd-MM-yyyy"),
                    },
                    new()
                    {
                        Id=4,
                        DocumentName = "Projects Management Policy",
                        DocumentType = "Policy",
                        IsAligned = false,
                        IsLocked = false,
                        ReviewStatus = "OVERDUE",
                        LastRevisionDate = DateTime.Now.AddMonths(-9).ToString("dd-MM-yyyy"),
                        NextRevisionDate = DateTime.Now.AddMonths(-12).ToString("dd-MM-yyyy"),
                    },
                    new()
                    {
                        Id=5,
                        DocumentName = "AML/CFT Policy",
                        DocumentType = "Policy",
                        IsAligned = true,
                        IsLocked = true,
                        ReviewStatus = "UPTODATE",
                        LastRevisionDate = DateTime.Now.AddMonths(-12).ToString("dd-MM-yyyy"),
                        NextRevisionDate = DateTime.Now.AddMonths(12).ToString("dd-MM-yyyy"),
                    },
                    new()
                    {
                        Id=6,
                        DocumentName = "Strategic Planning Procedures",
                        DocumentType = "Procedure",
                        IsAligned = true,
                        IsLocked = false,
                        ReviewStatus = "DUE",
                        LastRevisionDate = DateTime.Now.AddMonths(-4).ToString("dd-MM-yyyy"),
                        NextRevisionDate = DateTime.Now.AddMonths(-16).ToString("dd-MM-yyyy"),
                    },
                    new()
                    {
                        Id=7,
                        DocumentName = "Capacity Management Procedure",
                        DocumentType = "Policy",
                        IsAligned = true,
                        IsLocked = true,
                        ReviewStatus = "OVERDUE",
                        LastRevisionDate = DateTime.Now.AddMonths(6).ToString("dd-MM-yyyy"),
                        NextRevisionDate = DateTime.Now.AddMonths(-13).ToString("dd-MM-yyyy")
                    }
                }.AsQueryable();

        public PolicyService(IApplicationLoggerFactory loggerFactory, 
            IHttpHandler httpHandler, 
            IEnvironmentProvider environment, 
            IEndpointTypeProvider endpointType,
            IMapper mapper, 
            IWebHelper webHelper, 
            SessionManager sessionManager, 
            IGrcErrorFactory errorFactory, 
            IErrorService errorService) 
            : base(loggerFactory, httpHandler, environment, endpointType,
                  mapper, webHelper, sessionManager, errorFactory, errorService) {
        }

        public async Task<GrcResponse<PolicyRegisterResponse>> GetPolicyAsync(GrcIdRequst getRequest)
        {
            return await Task.FromResult(new GrcResponse<PolicyRegisterResponse>(query.FirstOrDefault()));
        }

        public async Task<GrcResponse<PagedResponse<PolicyRegisterResponse>>> GetAllPolicies(TableListRequest request)
        {
            //..filter data
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var lookUp = request.SearchTerm.ToLower();
                query = query.Where(a =>
                    (a.DocumentName != null && a.DocumentName.ToLower().Contains(lookUp)) ||
                    (a.DocumentType != null && a.DocumentType.ToLower().Contains(lookUp))
                );
            }

            //..apply sorting
            if (!string.IsNullOrEmpty(request.SortBy))
            {
                var sortExpr = $"{request.SortBy} {(request.SortDirection == "Ascending" ? "asc" : "desc")}";
                query = query.OrderBy(sortExpr);
            }

            var page = new PagedResponse<PolicyRegisterResponse>()
            {
                TotalCount = 20,
                Page = request.PageIndex,
                Size = request.PageSize,
                Entities = query.ToList(),
                TotalPages = 2
            };
            return await Task.FromResult(new GrcResponse<PagedResponse<PolicyRegisterResponse>>(page));
        }

        public async Task<GrcResponse<PolicyRegisterResponse>> CreatePolicyAsync(PolicyViewModel request)
        {
            var record = new PolicyRegisterResponse
            {
                DocumentName = request.DocumentName,
                DocumentType = "",
                IsAligned = request.IsAligned,
                IsLocked = true,
                DocumentStatus = request.DocumentStatus,
                Approver = request.Approver,
                Comments = request.Comments,
                LastRevisionDate = request.LastRevisionDate.ToString("dd-MM-yyyy"),
                NextRevisionDate = request.NextRevisionDate?.ToString("dd-MM-yyyy"),
                Id = query.Max(r => r.Id) + 1
            };
            return await Task.FromResult(new GrcResponse<PolicyRegisterResponse>(record));
        }

        public async Task<GrcResponse<PolicyRegisterResponse>> UpdatePolicyAsync(PolicyViewModel request)
        {
            var record = query.FirstOrDefault(r => r.Id == request.Id);

            if (record == null)
            {
                return await Task.FromResult(new GrcResponse<PolicyRegisterResponse>(record));
            }

            record.DocumentName = request.DocumentName;
            record.DocumentType = "";
            record.IsAligned = request.IsAligned;
            record.IsDeleted = false;
            record.DocumentStatus = request.DocumentStatus;
            record.Approver = request.Approver;
            record.Comments = request.Comments;
            record.LastRevisionDate = request.LastRevisionDate.ToString("dd-MM-yyyy");
            record.NextRevisionDate = request.NextRevisionDate?.ToString("dd-MM-yyyy");
            return await Task.FromResult(new GrcResponse<PolicyRegisterResponse>(record));
        }

        public async Task<GrcResponse<ServiceResponse>> DeletePolicyAsync(GrcIdRequst deleteRequest)
        {
            var record = query.FirstOrDefault(r => r.Id == deleteRequest.RecordId);
            if (record == null)
            {
                return await Task.FromResult(new GrcResponse<ServiceResponse>(new ServiceResponse
                {
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

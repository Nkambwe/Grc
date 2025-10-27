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
                        DocumentTypeId = 5,
                        DocumentType = "Policy",
                        DocumentStatus = "NOT-APP",
                        IsAligned = true,
                        IsLocked = false,
                        ReviewStatus = "UNKNOWN",
                        OwnerId = 1,
                        ReviewPeriod = 365,
                        Department = "Digitization and Innovation",
                        Owner = "Chief IT Officer",
                        ApprovalDate = DateTime.Now.AddMonths(-9).ToString("dd-MM-yyyy"),
                        LastRevisionDate = DateTime.Now.AddMonths(-8).ToString("dd-MM-yyyy"),
                        NextRevisionDate = DateTime.Now.AddMonths(-3).ToString("dd-MM-yyyy"),
                        Comments = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.",
                    },
                    new ()
                    {
                        Id=2,
                        DocumentName = "Administration Policy ",
                        DocumentTypeId = 5,
                        DocumentType = "Policy",
                        DocumentStatus = "UPTODATE",
                        IsAligned = true,
                        IsLocked = true,
                        ReviewStatus = "UPTODATE",
                        OwnerId = 1,
                        ReviewPeriod = 365,
                        Department = "Board Of Directors",
                        Owner = "BOD",
                        ApprovalDate = DateTime.Now.AddMonths(-9).ToString("dd-MM-yyyy"),
                        LastRevisionDate = DateTime.Now.AddMonths(-2).ToString("dd-MM-yyyy"),
                        NextRevisionDate = DateTime.Now.AddMonths(12).ToString("dd-MM-yyyy"),
                        Comments = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.",
                    },
                    new()
                    {
                        Id=3,
                        DocumentName = "Agent Banking Policy",
                        DocumentTypeId = 5,
                        DocumentType = "Policy",
                        DocumentStatus = "DUE-REV",
                        IsAligned = false,
                        IsLocked = false,
                        ReviewStatus = "DUE",
                        OwnerId = 1,
                        ReviewPeriod = 365,
                        Department = "Operations and Services",
                        Owner = "Head Operations",
                        ApprovalDate = DateTime.Now.AddMonths(-9).ToString("dd-MM-yyyy"),
                        LastRevisionDate = DateTime.Now.AddMonths(-11).ToString("dd-MM-yyyy"),
                        NextRevisionDate = DateTime.Now.AddMonths(-12).ToString("dd-MM-yyyy"),
                        Comments = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.",
                    },
                    new()
                    {
                        Id=4,
                        DocumentName = "Projects Management Policy",
                        DocumentTypeId = 5,
                        DocumentType = "Policy",
                        DocumentStatus = "DUE-REV",
                        IsAligned = false,
                        IsLocked = false,
                        OwnerId = 1,
                        ReviewPeriod = 365,
                        ReviewStatus = "OVERDUE",
                        Department = "Products Department",
                        Owner = "Head Products",
                        ApprovalDate = DateTime.Now.AddMonths(-9).ToString("dd-MM-yyyy"),
                        LastRevisionDate = DateTime.Now.AddMonths(-9).ToString("dd-MM-yyyy"),
                        NextRevisionDate = DateTime.Now.AddMonths(-12).ToString("dd-MM-yyyy"),
                        Comments = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.",
                    },
                    new()
                    {
                        Id=5,
                        DocumentName = "AML/CFT Policy",
                        DocumentTypeId = 5,
                        DocumentType = "Policy",
                        DocumentStatus = "UPTODATE",
                        IsAligned = true,
                        IsLocked = true,
                        ReviewStatus = "UPTODATE",
                        OwnerId = 1,
                        ReviewPeriod = 365,
                        Department = "IT Security",
                        Owner = "Chief Security Officer",
                        ApprovalDate = DateTime.Now.AddMonths(-9).ToString("dd-MM-yyyy"),
                        LastRevisionDate = DateTime.Now.AddMonths(-12).ToString("dd-MM-yyyy"),
                        NextRevisionDate = DateTime.Now.AddMonths(12).ToString("dd-MM-yyyy"),
                        Comments = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.",
                    },
                    new()
                    {
                        Id=6,
                        DocumentName = "Strategic Planning Procedures",
                        DocumentTypeId = 6,
                        DocumentType = "Procedure",
                        DocumentStatus = "DUE-REV",
                        IsAligned = true,
                        IsLocked = false,
                        ReviewStatus = "DUE",
                        OwnerId = 1,
                        ReviewPeriod = 365,
                        Department = "Board OF Directors",
                        Owner = "BOD",
                        ApprovalDate = DateTime.Now.AddMonths(-9).ToString("dd-MM-yyyy"),
                        LastRevisionDate = DateTime.Now.AddMonths(-4).ToString("dd-MM-yyyy"),
                        NextRevisionDate = DateTime.Now.AddMonths(-16).ToString("dd-MM-yyyy"),
                        Comments = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.",
                    },
                    new()
                    {
                        Id=7,
                        DocumentName = "Capacity Management Procedure",
                        DocumentTypeId = 2,
                        DocumentType = "Framework",
                        DocumentStatus = "DEPT-REV",
                        IsAligned = true,
                        IsLocked = true,
                        OwnerId = 1,
                        ReviewPeriod = 365,
                        ReviewStatus = "OVERDUE",
                        Department = "Operations and Services",
                        Owner = "Head Operations",
                        ApprovalDate = DateTime.Now.AddMonths(-9).ToString("dd-MM-yyyy"),
                        LastRevisionDate = DateTime.Now.AddMonths(6).ToString("dd-MM-yyyy"),
                        NextRevisionDate = DateTime.Now.AddMonths(-13).ToString("dd-MM-yyyy"),
                        Comments = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book.",
                    },
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

        public async Task<GrcResponse<PolicyRegisterResponse>> GetPolicyAsync(GrcIdRequest getRequest) {
            return await Task.FromResult(new GrcResponse<PolicyRegisterResponse>(query.FirstOrDefault(q => q.Id == getRequest.RecordId)));
        }

        public async Task<GrcResponse<List<PolicyRegisterResponse>>> GetAllAsync(GrcRequest request)
        {
            return await Task.FromResult(new GrcResponse<List<PolicyRegisterResponse>>(query.ToList()));
        }

        public async Task<GrcResponse<PagedResponse<PolicyRegisterResponse>>> GetAllPolicies(TableListRequest request) {
            //..filter data
            if (!string.IsNullOrWhiteSpace(request.SearchTerm)) {
                var lookUp = request.SearchTerm.ToLower();
                query = query.Where(a =>
                    (a.DocumentName != null && a.DocumentName.ToLower().Contains(lookUp)) ||
                    (a.DocumentStatus != null && a.DocumentStatus.ToLower().Contains(lookUp)) ||
                    (a.ReviewStatus != null && a.ReviewStatus.ToLower().Contains(lookUp))
                );
            }

            //..apply sorting
            if (!string.IsNullOrEmpty(request.SortBy)) {
                var sortExpr = $"{request.SortBy} {(request.SortDirection == "Ascending" ? "asc" : "desc")}";
                query = query.OrderBy(sortExpr);
            }

            //..get paged data
            var page = new PagedResponse<PolicyRegisterResponse>() {
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
                DocumentTypeId = request.DocumentTypeId,
                DocumentStatus = request.DocumentStatus,
                IsAligned = request.IsAligned,
                IsLocked = request.IsLocked,
                ReviewPeriod = request.ReviewPeriod,
                ReviewStatus = request.ReviewStatus,
                OwnerId = request.OwnerId,
                Comments = request.Comments,
                Approver = "",
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
            record.DocumentTypeId = request.DocumentTypeId;
            record.DocumentStatus = request.DocumentStatus;
            record.IsAligned = request.IsAligned;
            record.IsLocked = request.IsLocked;
            record.ReviewPeriod = request.ReviewPeriod;
            record.ReviewStatus = request.ReviewStatus;
            record.OwnerId = request.OwnerId;
            record.Comments = request.Comments;
            record.Approver = "";
            record.LastRevisionDate = request.LastRevisionDate.ToString("dd-MM-yyyy");
            record.NextRevisionDate = request.NextRevisionDate?.ToString("dd-MM-yyyy");
            return await Task.FromResult(new GrcResponse<PolicyRegisterResponse>(record));
        }

        public async Task<GrcResponse<ServiceResponse>> DeletePolicyAsync(GrcIdRequest deleteRequest)
        {
            var record = query.FirstOrDefault(r => r.Id == deleteRequest.RecordId);
            if (record == null)
            {
                return await Task.FromResult(new GrcResponse<ServiceResponse>(new ServiceResponse
                {
                    Status = false,
                    StatusCode = 404,
                    Message = "Policy not found"
                }));
            }
            return await Task.FromResult(new GrcResponse<ServiceResponse>(new ServiceResponse
            {
                Status = true,
                StatusCode = 200,
                Message = "Policy deleted successfully"
            }));
        }

    }
}

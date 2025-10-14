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
    public class RegulatoryActService : GrcBaseService, IRegulatoryActService
    {
        private IQueryable<RegulatoryActResponse> query = new List<RegulatoryActResponse>() {
                new RegulatoryActResponse {
                    Id = 1,
                    RegulatoryName = "Financial Institutions Act, 2004",
                    AuthorityId = 1,
                    RegulatoryAuthority = "Bank of Uganda",
                    ReviewFrequency = "ANNUALY",
                    IsActive = true,
                    LastReviewDate = new DateTime(2023, 6, 15),
                    ReviewResponsibility = "Financial Institutions Supervision Department",
                    Comments = "Primary legislation governing all financial institutions in Uganda"
                },
                new RegulatoryActResponse {
                    Id = 2,
                    RegulatoryName = "Anti-Money Laundering Act, 2013",
                    AuthorityId = 1,
                    RegulatoryAuthority = "Bank of Uganda",
                    ReviewFrequency =  "BI-ANNUALY",
                    IsActive = true,
                    LastReviewDate = new DateTime(2024, 1, 10),
                    ReviewResponsibility = "Financial Intelligence Authority",
                    Comments = "Implements FATF recommendations on AML/CFT"
                },
                new RegulatoryActResponse {
                    Id = 3,
                    RegulatoryName = "The Proceeds of Crime Act, 2023",
                    AuthorityId = 2,
                    RegulatoryAuthority = "Financial Intelligence Authority",
                    ReviewFrequency =  "THREE-YEARS",
                    IsActive = true,
                    LastReviewDate = new DateTime(2023, 11, 20),
                    ReviewResponsibility = "FIA Legal Department",
                    Comments = "Recent legislation strengthening asset recovery mechanisms"
                },
                new RegulatoryActResponse {
                    Id = 4,
                    RegulatoryName = "Microfinance Deposit-Taking Institutions Act, 2003",
                    AuthorityId = 1,
                    RegulatoryAuthority = "Bank of Uganda",
                    ReviewFrequency =  "ANNUALY",
                    IsActive = true,
                    LastReviewDate = new DateTime(2022, 9, 5),
                    ReviewResponsibility = "Microfinance Supervision Department",
                    Comments = "Regulates MDIs and tier 4 financial institutions"
                },
                new RegulatoryActResponse {
                    Id = 5,
                    RegulatoryName = "The Credit Reference Systems Act, 2021",
                    AuthorityId = 1,
                    RegulatoryAuthority = "Bank of Uganda",
                    ReviewFrequency =  "ANNUALY",
                    IsActive = true,
                    LastReviewDate = new DateTime(2023, 3, 12),
                    ReviewResponsibility = "Credit Reference Bureau Supervision",
                    Comments = "Governs credit reporting and data sharing"
                },
                new RegulatoryActResponse {
                    Id = 6,
                    RegulatoryName = "The Capital Markets Authority Act, Cap 84",
                    AuthorityId = 3,
                    RegulatoryAuthority = "Capital Markets Authority",
                    ReviewFrequency =  "ANNUALY",
                    IsActive = true,
                    LastReviewDate = new DateTime(2022, 12, 8),
                    ReviewResponsibility = "CMA Legal and Regulatory Affairs",
                    Comments = "Regulates securities markets and collective investment schemes"
                },
                new RegulatoryActResponse {
                    Id = 7,
                    RegulatoryName = "The Insurance Act, 2017",
                    AuthorityId = 4,
                    RegulatoryAuthority = "Insurance Regulatory Authority of Uganda",
                    ReviewFrequency = "BI-ANNUALY",
                    IsActive = true,
                    LastReviewDate = new DateTime(2023, 8, 22),
                    ReviewResponsibility = "IRA Legal Department",
                    Comments = "Consolidated legislation for insurance industry regulation"
                },
                new RegulatoryActResponse {
                    Id = 8,
                    RegulatoryName = "The Payments Systems Act, 2020",
                    AuthorityId = 1,
                    RegulatoryAuthority = "Bank of Uganda",
                    ReviewFrequency =  "SEMI-ANNUALY",
                    IsActive = true,
                    LastReviewDate = new DateTime(2024, 2, 1),
                    ReviewResponsibility = "National Payments System Department",
                    Comments = "Regulates electronic payments, mobile money, and fintech"
                },
                new RegulatoryActResponse {
                    Id = 9,
                    RegulatoryName = "The Tier 4 Microfinance Institutions and Money Lenders Act, 2016",
                    AuthorityId = 5,
                    RegulatoryAuthority = "Uganda Microfinance Regulatory Authority",
                    ReviewFrequency =  "ANNUALY",
                    IsActive = true,
                    LastReviewDate = new DateTime(2023, 5, 18),
                    ReviewResponsibility = "UMRA Compliance Department",
                    Comments = "Regulates SACCOs and money lending institutions"
                },
                new RegulatoryActResponse {
                    Id = 10,
                    RegulatoryName = "The Foreign Exchange Act, 2004",
                    AuthorityId = 1,
                    RegulatoryAuthority = "Bank of Uganda",
                    ReviewFrequency =  "ANNUALY",
                    IsActive = true,
                    LastReviewDate = new DateTime(2022, 11, 30),
                    ReviewResponsibility = "Financial Markets Department",
                    Comments = "Governs foreign exchange transactions and controls"
                },
                new RegulatoryActResponse {
                    Id = 11,
                    RegulatoryName = "The Public Finance Management Act, 2015",
                    AuthorityId = 6,
                    RegulatoryAuthority = "Ministry of Finance, Planning and Economic Development",
                    ReviewFrequency =  "ANNUALY",
                    IsActive = true,
                    LastReviewDate = new DateTime(2023, 4, 10),
                    ReviewResponsibility = "Accountant General's Department",
                    Comments = "Governs public financial management and accountability"
                },
                new RegulatoryActResponse {
                    Id = 12,
                    RegulatoryName = "The Companies Act, 2012",
                    AuthorityId = 7,
                    RegulatoryAuthority = "Uganda Registration Services Bureau",
                    ReviewFrequency =  "ANNUALY",
                    IsActive = true,
                    LastReviewDate = new DateTime(2022, 7, 15),
                    ReviewResponsibility = "URSB Legal Department",
                    Comments = "Includes provisions on corporate governance for financial institutions"
                },
                new RegulatoryActResponse {
                    Id = 13,
                    RegulatoryName = "The Data Protection and Privacy Act, 2019",
                    AuthorityId = 8,
                    RegulatoryAuthority = "Personal Data Protection Office",
                    ReviewFrequency =  "ANNUALY",
                    IsActive = true,
                    LastReviewDate = new DateTime(2023, 10, 5),
                    ReviewResponsibility = "Data Protection Commission",
                    Comments = "Critical for financial data handling and customer privacy"
                },
                new RegulatoryActResponse {
                    Id = 14,
                    RegulatoryName = "The Tax Procedures Code Act, 2014",
                    AuthorityId = 9,
                    RegulatoryAuthority = "Uganda Revenue Authority",
                    ReviewFrequency =  "ANNUALY",
                    IsActive = true,
                    LastReviewDate = new DateTime(2024, 1, 25),
                    ReviewResponsibility = "URA Legal Department",
                    Comments = "Includes provisions for financial transaction reporting"
                },
                new RegulatoryActResponse {
                    Id = 15,
                    RegulatoryName = "The Counter Terrorism Act, 2015",
                    AuthorityId = 2,
                    RegulatoryAuthority = "Financial Intelligence Authority",
                    ReviewFrequency =  "ANNUALY",
                    IsActive = true,
                    LastReviewDate = new DateTime(2023, 12, 15),
                    ReviewResponsibility = "FIA Counter Financing of Terrorism Unit",
                    Comments = "Implements UN resolutions on countering terrorism financing"
                }
            }.AsQueryable();

        public RegulatoryActService(IApplicationLoggerFactory loggerFactory, 
                                    IHttpHandler httpHandler, 
                                    IEnvironmentProvider environment, 
                                    IEndpointTypeProvider endpointType, 
                                    IMapper mapper, 
                                    IWebHelper webHelper, 
                                    SessionManager sessionManager, 
                                    IGrcErrorFactory errorFactory, 
                                    IErrorService errorService) 
                                    : base(loggerFactory, httpHandler, environment, 
                                          endpointType, mapper, webHelper, sessionManager, 
                                          errorFactory, errorService) {
        }

        public async Task<GrcResponse<RegulatoryActResponse>> GetRegulatoryActAsyncAsync(GrcIdRequst getRequest)
        {
            return await Task.FromResult(new GrcResponse<RegulatoryActResponse>(query.FirstOrDefault(q => q.Id == getRequest.RecordId)));
        }

        public async Task<GrcResponse<List<RegulatoryActResponse>>> GetAllAsync(GrcRequest request)
        {
            return await Task.FromResult(new GrcResponse<List<RegulatoryActResponse>>(query.ToList()));
        }

        public async Task<GrcResponse<PagedResponse<RegulatoryActResponse>>> GetPagedRegulatoryActAsync(TableListRequest request) {
            //..filter data
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var lookUp = request.SearchTerm.ToLower();
                query = query.Where(a =>
                    (a.RegulatoryName != null && a.RegulatoryName.ToLower().Contains(lookUp)) ||
                    (a.RegulatoryAuthority != null && a.RegulatoryAuthority.ToLower().Contains(lookUp)) ||
                    (a.ReviewResponsibility != null && a.ReviewResponsibility.ToLower().Contains(lookUp))
                );
            }

            //..apply sorting
            if (!string.IsNullOrEmpty(request.SortBy))
            {
                var sortExpr = $"{request.SortBy} {(request.SortDirection == "Ascending" ? "asc" : "desc")}";
                query = query.OrderBy(sortExpr);
            }

            //..get paged data
            var page = new PagedResponse<RegulatoryActResponse>()
            {
                TotalCount = 20,
                Page = request.PageIndex,
                Size = request.PageSize,
                Entities = query.ToList(),
                TotalPages = 2
            };
            return await Task.FromResult(new GrcResponse<PagedResponse<RegulatoryActResponse>>(page));
        }

        public async Task<GrcResponse<RegulatoryActResponse>> CreateRegulatoryActAsync(RegulatoryActViewModel request)
        {
            var record = new RegulatoryActResponse
            {
                RegulatoryName = request.RegulatoryName,
                ReviewFrequency = request.ReviewFrequency,
                RegulatoryAuthority = request.RegulatoryAuthority,
                AuthorityId = request.AuthorityId,
                Comments = request.Comments,
                LastReviewDate = request.LastReviewDate,
                Id = query.Max(r => r.Id) + 1
            };
            return await Task.FromResult(new GrcResponse<RegulatoryActResponse>(record));
        }

        public async Task<GrcResponse<RegulatoryActResponse>> UpdateRegulatoryActAsync(RegulatoryActViewModel request) {
            var record = query.FirstOrDefault(r => r.Id == request.Id);

            if (record == null)
            {
                return await Task.FromResult(new GrcResponse<RegulatoryActResponse>(record));
            }

            record.RegulatoryName = request.RegulatoryName;
            record.ReviewFrequency = request.ReviewFrequency;
            record.RegulatoryAuthority = request.RegulatoryAuthority;
            record.AuthorityId = request.AuthorityId;
            record.Comments = request.Comments;
            record.LastReviewDate = request.LastReviewDate;
            return await Task.FromResult(new GrcResponse<RegulatoryActResponse>(record));
        }

        public async Task<GrcResponse<ServiceResponse>> DeleteRegulatoryActAsync(GrcIdRequst deleteRequest)
        {
            var record = query.FirstOrDefault(r => r.Id == deleteRequest.RecordId);
            if (record == null)
            {
                return await Task.FromResult(new GrcResponse<ServiceResponse>(new ServiceResponse
                {
                    Status = false,
                    StatusCode = 404,
                    Message = "Regulatory act not found"
                }));
            }
            return await Task.FromResult(new GrcResponse<ServiceResponse>(new ServiceResponse
            {
                Status = true,
                StatusCode = 200,
                Message = "Regulatory act deleted successfully"
            }));
        }

    }
}

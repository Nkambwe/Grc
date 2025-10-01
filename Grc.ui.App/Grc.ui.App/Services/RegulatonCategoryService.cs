using AutoMapper;
using DocumentFormat.OpenXml.Office2016.Excel;
using Grc.ui.App.Factories;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Utils;

namespace Grc.ui.App.Services
{
    public class RegulatonCategoryService : GrcBaseService, IRegulatonCategoryService
    {
        public RegulatonCategoryService(IApplicationLoggerFactory loggerFactory, 
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

        public async Task<GrcResponse<RegulatoryCategoryResponse>> GetCategoryAsync(GrcIdRequst deleteRequest) {
            var result = new RegulatoryCategoryResponse {
                Id = 31,
                CategoryName = "Sample Regulation",
                CreatedAt = DateTime.Now.AddDays(-10),
                UpdatedAt = DateTime.Now.AddDays(-2),
                IsDeleted = false
            };
            return await Task.FromResult(new GrcResponse<RegulatoryCategoryResponse>(result));
        }

        public async Task<GrcResponse<PagedResponse<RegulatoryCategoryResponse>>> GetAllRegulatoryCategories(TableListRequest request)
        {
            var page = new PagedResponse<RegulatoryCategoryResponse>()
            {
                TotalCount = 20,
                Page = request.PageIndex,
                Size = request.PageSize,
                Entities = new List<RegulatoryCategoryResponse> {
                    new() { Id = 1, CategoryName = "First Regulation", CreatedAt = DateTime.Now,IsDeleted = true},
                    new() { Id = 2, CategoryName = "Second Regulation", CreatedAt = DateTime.Now,IsDeleted = true},
                    new() { Id = 3, CategoryName = "Third Regulation", CreatedAt = DateTime.Now,IsDeleted = true},
                    new() { Id = 4, CategoryName = "Fourth Regulation", CreatedAt = DateTime.Now,IsDeleted = true},
                    new() { Id = 5, CategoryName = "Fifth Regulation", CreatedAt = DateTime.Now,IsDeleted = true},
                    new() { Id = 6, CategoryName = "Sixth Regulation", CreatedAt = DateTime.Now,IsDeleted = true},
                    new() { Id = 7, CategoryName = "Seventh Regulation", CreatedAt = DateTime.Now,IsDeleted = true},
                    new() { Id = 8, CategoryName = "Eighth Regulation", CreatedAt = DateTime.Now,IsDeleted = true},
                    new() { Id = 9, CategoryName = "Nineth Regulation", CreatedAt = DateTime.Now,IsDeleted = true},
                    new() { Id = 10, CategoryName = "Tenth Regulation", CreatedAt = DateTime.Now,IsDeleted =true},
                    new() { Id = 11, CategoryName = "Eleventh Regulation", CreatedAt = DateTime.Now,IsDeleted =true},
                    new() { Id = 12, CategoryName = "Twelveth Regulation", CreatedAt = DateTime.Now,IsDeleted =true},
                    new() { Id = 13, CategoryName = "Thirteenth Regulation", CreatedAt = DateTime.Now,IsDeleted =true},
                    new() { Id = 14, CategoryName = "Fourteenth Regulation", CreatedAt = DateTime.Now,IsDeleted =true},
                    new() { Id = 15, CategoryName = "Fifteenth Regulation", CreatedAt = DateTime.Now,IsDeleted =true},
                    new() { Id = 16, CategoryName = "Seventeenth Regulation", CreatedAt = DateTime.Now,IsDeleted =true},
                    new() { Id = 17, CategoryName = "Mobile Regulation", CreatedAt = DateTime.Now,IsDeleted =true},
                    new() { Id = 18, CategoryName = "Investment Regulation", CreatedAt = DateTime.Now,IsDeleted =true},
                    new() { Id = 19, CategoryName = "Business Regulation", CreatedAt = DateTime.Now,IsDeleted =true},
                    new() { Id = 20, CategoryName = "Trade Regulation", CreatedAt = DateTime.Now,IsDeleted =true},
                    new() { Id = 21, CategoryName = "Banking Regulation", CreatedAt = DateTime.Now,IsDeleted =true }
                },
                TotalPages = 2
            };

            return await Task.FromResult(new GrcResponse<PagedResponse<RegulatoryCategoryResponse>>(page));
        }

        public async Task<GrcResponse<List<RegulatoryCategoryResponse>>> GetRegulatoryCategories(GrcRequest request)
        {
            //Page = request.PageIndex,
            //Size = request.PageSize,
            var data = new List<RegulatoryCategoryResponse> {
                    new() { Id = 1, CategoryName = "First Regulation", CreatedAt = DateTime.Now,IsDeleted = true},
                    new() { Id = 2, CategoryName = "Second Regulation", CreatedAt = DateTime.Now,IsDeleted = true},
                    new() { Id = 3, CategoryName = "Third Regulation", CreatedAt = DateTime.Now,IsDeleted = true},
                    new() { Id = 4, CategoryName = "Fourth Regulation", CreatedAt = DateTime.Now,IsDeleted = true},
                    new() { Id = 5, CategoryName = "Fifth Regulation", CreatedAt = DateTime.Now,IsDeleted = true},
                    new() { Id = 6, CategoryName = "Sixth Regulation", CreatedAt = DateTime.Now,IsDeleted = true},
                    new() { Id = 7, CategoryName = "Seventh Regulation", CreatedAt = DateTime.Now,IsDeleted = true},
                    new() { Id = 8, CategoryName = "Eighth Regulation", CreatedAt = DateTime.Now,IsDeleted = true},
                    new() { Id = 9, CategoryName = "Nineth Regulation", CreatedAt = DateTime.Now,IsDeleted = true},
                    new() { Id = 10, CategoryName = "Tenth Regulation", CreatedAt = DateTime.Now,IsDeleted =true},
                    new() { Id = 11, CategoryName = "Eleventh Regulation", CreatedAt = DateTime.Now,IsDeleted =true},
                    new() { Id = 12, CategoryName = "Twelveth Regulation", CreatedAt = DateTime.Now,IsDeleted =true},
                    new() { Id = 13, CategoryName = "Thirteenth Regulation", CreatedAt = DateTime.Now,IsDeleted =true},
                    new() { Id = 14, CategoryName = "Fourteenth Regulation", CreatedAt = DateTime.Now,IsDeleted =true},
                    new() { Id = 15, CategoryName = "Fifteenth Regulation", CreatedAt = DateTime.Now,IsDeleted =true},
                    new() { Id = 16, CategoryName = "Seventeenth Regulation", CreatedAt = DateTime.Now,IsDeleted =true},
                    new() { Id = 17, CategoryName = "Mobile Regulation", CreatedAt = DateTime.Now,IsDeleted =true},
                    new() { Id = 18, CategoryName = "Investment Regulation", CreatedAt = DateTime.Now,IsDeleted =true},
                    new() { Id = 19, CategoryName = "Business Regulation", CreatedAt = DateTime.Now,IsDeleted =true},
                    new() { Id = 20, CategoryName = "Trade Regulation", CreatedAt = DateTime.Now,IsDeleted =true},
                    new() { Id = 21, CategoryName = "Banking Regulation", CreatedAt = DateTime.Now,IsDeleted =true }
                };
            return await Task.FromResult(new GrcResponse<List<RegulatoryCategoryResponse>>(data));
        }

        public async Task<GrcResponse<RegulatoryCategoryResponse>> CreateCategoryAsync(RegulatoryCategoryRequest request) {
            return await Task.FromResult(new GrcResponse<RegulatoryCategoryResponse>(new RegulatoryCategoryResponse {
                Id = 1,
                CategoryName = request.Category ,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            }));
        }

        public async Task<GrcResponse<RegulatoryCategoryResponse>> UpdateCategoryAsync(RegulatoryCategoryRequest request) {
            return await Task.FromResult(new GrcResponse<RegulatoryCategoryResponse>(new RegulatoryCategoryResponse {
                Id = request.Id,
                CategoryName = request.Category ,
                CreatedAt = DateTime.Now.AddDays(-2),
                UpdatedAt = DateTime.Now,
                IsDeleted = request.Status == "Inactive"
            }));
        }

        public async Task<GrcResponse<ServiceResponse>> DeleteCategoryAsync(GrcIdRequst deleteRequest)
        {
            return await Task.FromResult(new GrcResponse<ServiceResponse>(new ServiceResponse
            {
                Status = true,
                StatusCode = 200,
                Message = "Record deleted successfully"
            }));
        }

    }
}

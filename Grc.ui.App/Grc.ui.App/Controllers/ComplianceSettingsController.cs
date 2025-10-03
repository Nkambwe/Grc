using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Grc.ui.App.Defaults;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Extensions.Http;
using Grc.ui.App.Factories;
using Grc.ui.App.Filters;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Grc.ui.App.Controllers {
    public class ComplianceSettingsController : GrcBaseController {
        private readonly IAuthenticationService _authService;
        private readonly IRegulatonCategoryService _regulatoryCategoryService;
        private readonly IRegulatonTypeService _regulatoryTypeService;
        private readonly IRegulatonAuthorityService _regulatoryAuthorityService;
        private readonly IDocumentTypeService _documentTypeService;

        public ComplianceSettingsController(IApplicationLoggerFactory loggerFactory, 
            IEnvironmentProvider environment, 
            IWebHelper webHelper, 
            ILocalizationService localizationService, 
            IErrorService errorService, 
            IAuthenticationService authService,
            IGrcErrorFactory errorFactory,
            IRegulatonCategoryService regulatoryService,
            IRegulatonTypeService regulatoryTypeService,
            IRegulatonAuthorityService regulatoryAuthorityService,
            IDocumentTypeService documentTypeService,
            SessionManager sessionManager) 
            : base(loggerFactory, environment, webHelper, 
                  localizationService, errorService,
                  errorFactory, sessionManager) {

            Logger.Channel = $"COMP-SETTINGS-{DateTime.Now:yyyyMMddHHmmss}";
             _authService = authService;
            _regulatoryCategoryService = regulatoryService;
            _regulatoryTypeService = regulatoryTypeService;
            _regulatoryAuthorityService = regulatoryAuthorityService;
            _documentTypeService = documentTypeService;
        }

        #region Regulatory Categories
        public async Task<IActionResult> ComplianceRegulatoryCategories() {
            try {
                if (User.Identity?.IsAuthenticated == true) {
                    var ipAddress = WebHelper.GetCurrentIpAddress();
                    var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                    if (userResponse.HasError || userResponse.Data == null) {
                        TempData["UiResponse"] = JsonSerializer.Serialize(
                            UiResponse.Fail("Your session expired. Please login again.")
                        );
                        return Redirect(Url.Action("Dashboard", "Application"));
                    }

                    var currentUser = userResponse.Data;
                    var userDashboard = new UserDashboardModel
                    {
                        Initials = $"{currentUser.FirstName[..1]} {currentUser.LastName[..1]}",
                        LastLogin = DateTime.Now,
                        Workspace = SessionManager.GetWorkspace(),
                        //..add statistics
                        DashboardStatistics = new()
                    };

                    return View(userDashboard);
                } else {
                    return RedirectToAction("Login", "Application");
                }

            } catch (Exception ex)  {
                Logger.LogActivity($"Error loading Compliance Regulatory Categories view: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                TempData["UiResponse"] = JsonSerializer.Serialize(
                           UiResponse.Fail("Error loading Compliance Regulatory types view")
                       );
                return Redirect(Url.Action("Dashboard", "Application"));
            }

        }

        [LogActivityResult("Retrieve Category", "User retrieved regulatory category", ActivityTypeDefaults.COMPLIACE_RETRIVE_CATEGORY, "Category")]
        public async Task<IActionResult> GetRegulatoryCategory(long id)
        {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new
                    {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }

                if (id == 0)
                {
                    var msg = "Category Id is required";
                    Logger.LogActivity(msg);
                    return BadRequest(new
                    {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }
                var currentUser = userResponse.Data;
                GrcIdRequst getRequest = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_GET_CATEGORY.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>()
                };

                GrcResponse<RegulatoryCategoryResponse> result = await _regulatoryCategoryService.GetCategoryAsync(getRequest);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Unerror occurred while deleting category";
                    Logger.LogActivity(errMsg);
                    return Ok(new
                    {
                        message = errMsg,
                        data = new { }
                    });
                }

                RegulatoryCategoryResponse response = result.Data;
                var categoryRecord = new {
                    id = response.Id,
                    startTab = "",
                    category = response.CategoryName,
                    status = response.IsDeleted ? "Inactive" : "Active",
                    addedon = response.CreatedAt.ToString("dd-MM-yyyy"),
                    endTab = ""
                };

                return Ok(new {
                    success = true,
                    message = "Category created successfully",
                    data = categoryRecord
                });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected error deleting category: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return Redirect(Url.Action("ComplianceRegulatoryCategories", "ComplianceSettings"));
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(GrcAntiForgeryTokenAttribute))]
        [LogActivityResult("Add Category", "User added regulatory category", ActivityTypeDefaults.COMPLIACE_CREATE_CATEGORY, "Category")]
        public async Task<IActionResult> CreateRegulatoryCategory([FromBody] RegulatoryCategoryRequest request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new{
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }

                if (request == null) {
                    var msg = "Invalid request data";
                    Logger.LogActivity(msg);
                    return BadRequest(new {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }

                //..set system fields
                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.CREATEREGULATORYCATEGORY.GetDescription();

                GrcResponse<RegulatoryCategoryResponse> result = await _regulatoryCategoryService.CreateCategoryAsync(request);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Failed to create category";
                    Logger.LogActivity(errMsg);
                    return Ok(new {
                        success = false,
                        message = errMsg,
                        data = new { }
                    });
                }
                
                var created = result.Data;
                var categoryRecord = new {
                    id = created.Id,
                    startTab = "",
                    category = created.CategoryName,
                    status = created.IsDeleted ? "Inactive" : "Active",
                    addedon = created.CreatedAt.ToString("dd-MM-yyyy"),
                    endTab = ""
                };

                return Ok(new {
                    success = true,
                    message = "Category created successfully",
                    data = categoryRecord
                });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error creating category: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return Redirect(Url.Action("ComplianceRegulatoryCategories", "ComplianceSettings"));
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(GrcAntiForgeryTokenAttribute))]
        [LogActivityResult("Update Category", "User modified regulatory category", ActivityTypeDefaults.COMPLIANCE_EDITED_CATEGORY, "Category")]
        public async Task<IActionResult> UpdateRegulatoryCategory([FromBody] RegulatoryCategoryRequest request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }

                if (request == null) {
                    var msg = "Invalid request data";
                    Logger.LogActivity(msg);
                    return BadRequest(new {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }

                //..set system fields
                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.COMPLIANCE_EDITED_CATEGORY.GetDescription();

                GrcResponse<RegulatoryCategoryResponse> result = await _regulatoryCategoryService.UpdateCategoryAsync(request);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Failed to update category";
                    Logger.LogActivity(errMsg);
                    return Ok(new {
                        success = false,
                        message = errMsg,
                        data = new { }
                    });
                }

                var updated = result.Data;
                var categoryRecord = new {
                    id = updated.Id,
                    category = updated.CategoryName,
                    status = updated.IsDeleted ? "Inactive" : "Active",
                    addedon = updated.CreatedAt.ToString("dd-MM-yyyy")
                };

                return Ok(new {
                    success = true,
                    message = "Category updated successfully",
                    data = categoryRecord
                });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error updating category: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return Redirect(Url.Action("ComplianceRegulatoryCategories", "ComplianceSettings"));
            }
        }

        [HttpDelete]
        [ServiceFilter(typeof(GrcAntiForgeryTokenAttribute))]
        [LogActivityResult("Delete Category", "User deleted regulatory category", ActivityTypeDefaults.COMPLIANCE_DELETED_CATEGORY, "Category")]
        public async Task<IActionResult> DeleteRegulatoryCategory(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new {
                        message = msg,
                        data = new { }
                    });
                }

                if (id == 0) {
                    var msg = "Category Id is required";
                    Logger.LogActivity(msg);
                    return BadRequest(new {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }
                var currentUser = userResponse.Data;
                GrcIdRequst deleteRequest = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_DELETED_CATEGORY.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>()
                };

                var  result = await _regulatoryCategoryService.DeleteCategoryAsync(deleteRequest);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Unerror occurred while deleting category";
                    Logger.LogActivity(errMsg);
                    return Ok(new {
                        message = errMsg,
                        data = new { }
                    });
                }

                ServiceResponse response = result.Data;
                return Ok(new {
                    message = response.Message,
                    success = response.Status,
                });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error deleting category: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return Redirect(Url.Action("ComplianceRegulatoryCategories", "ComplianceSettings"));
            }
        }
        
        [HttpPost]
        [LogActivityResult("Export Categories", "User exported regulatory categories to excel", ActivityTypeDefaults.COMPLIANCE_EXPORT_CATEGORY, "Category")]
        public IActionResult ExportToExcel([FromBody] List<RegulatoryCategoryResponse> data) {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Regulation Categories");

            //..headers
            worksheet.Cell(1, 1).Value = "Id";
            worksheet.Cell(1, 2).Value = "Category";
            worksheet.Cell(1, 3).Value = "Status";
            worksheet.Cell(1, 4).Value = "Added On";

            int row = 2;
            foreach (var item in data) {
                worksheet.Cell(row, 1).Value = item.Id;
                worksheet.Cell(row, 2).Value = item.CategoryName;
                worksheet.Cell(row, 3).Value = item.IsDeleted ? "Inactive" : "Active";
                worksheet.Cell(row, 4).Value = item.CreatedAt.ToString("dd-MM-yyyy");
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "RegulatoryCategories.xlsx"
            );
        }

        [HttpPost]
        [LogActivityResult("Export Categories", "User exported regulatory categories to excel", ActivityTypeDefaults.COMPLIANCE_EXPORT_CATEGORY, "Category")]
        public async Task<IActionResult> ExportAllCategories() {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null) {
                var msg = "Unable to resolve current user";
                Logger.LogActivity(msg);
                return Ok(new {
                    success = false,
                    message = msg,
                    data = new { }
                });
            }

            var request = new TableListRequest {
                UserId = userResponse.Data.UserId,
                IPAddress = ipAddress,
                PageIndex = 1,
                PageSize = int.MaxValue, 
                Action = Activity.COMPLIANCE_EXPORT_CATEGORY.GetDescription()
            };

            var categoryData = await _regulatoryCategoryService.GetAllRegulatoryCategories(request);
            if (categoryData.HasError || categoryData.Data == null) {
                var errMsg = categoryData.Error?.Message ?? "Failed to retrieve categories";
                Logger.LogActivity(errMsg);
                return Ok(new {
                    success = false,
                    message = errMsg,
                    data = new { }
                });
            }

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Categories");

            //..headers
            ws.Cell(1, 1).Value = "ID";
            ws.Cell(1, 2).Value = "Category";
            ws.Cell(1, 3).Value = "Status";
            ws.Cell(1, 4).Value = "Added On";

            int row = 2;
            foreach (var cat in categoryData.Data.Entities) {
                ws.Cell(row, 1).Value = cat.Id;
                ws.Cell(row, 2).Value = cat.CategoryName;
                ws.Cell(row, 3).Value = cat.IsDeleted ? "Inactive" : "Active";
                ws.Cell(row, 4).Value = cat.CreatedAt.ToString("dd-MM-yyyy");
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "RegulatoryCategories.xlsx"
            );
        }

        public async Task<IActionResult> GetRegulatoryCategories()
        {
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    Logger.LogActivity($"REGULATORY CATEGORY LIST ERROR: Failed to get current user record - {JsonSerializer.Serialize(grcResponse)}");
                }

                var currentUser = grcResponse.Data;
                GrcRequest request = new()
                {
                    UserId = currentUser.UserId,
                    Action = Activity.RETRIEVEREGULATORYCATEGORIES.GetDescription(),
                    IPAddress = ipAddress,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>()
                };

                //..get list of all regulatory categories
                var categoryData = await _regulatoryCategoryService.GetRegulatoryCategories(request);

                List<RegulatoryCategoryResponse> categories;
                if (categoryData.HasError)
                {
                    categories = new();
                    Logger.LogActivity($"REGULATORY CATEGORY DATA ERROR: Failed to retrieve category items - {JsonSerializer.Serialize(categoryData)}");
                }
                else
                {
                    categories = categoryData.Data;
                    Logger.LogActivity($"REGULATORY CATEGORY DATA - {JsonSerializer.Serialize(categories)}");
                }

                //..transform data for Tabulator
                var tabulatorData = categories.Select(cat => new {
                    id = cat.Id,
                    startTab = "",
                    category = cat.CategoryName,
                    status = cat.IsDeleted,
                    addedon = cat.CreatedAt.ToString("dd-MM-yyyy"),
                    endTab = ""
                }).ToList();

                return Json(new { success = true, data = tabulatorData });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving regulatory categories: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "REGULATORY-CATEGORY-CONTROLLER", ex.StackTrace);
                return Json(new { success = false, data = new List<object>() });
            }
        }

        public async Task<IActionResult> AllRegulatoryCategories([FromBody] TableListRequest request)
        {
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    Logger.LogActivity($"REGULATORY CATEGORY DATA ERROR: Failed to get current user record - {JsonSerializer.Serialize(grcResponse)}");
                }

                //..update with user data
                var currentUser = grcResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.RETRIEVEREGULATORYCATEGORIES.GetDescription();

                //..get regulatory category data
                var categoryData = await _regulatoryCategoryService.GetAllRegulatoryCategories(request);
                PagedResponse<RegulatoryCategoryResponse> categoryList = new();

                if (categoryData.HasError)
                {
                    Logger.LogActivity($"REGULATORY CATEGORY DATA ERROR: Failed to retrieve category items - {JsonSerializer.Serialize(categoryData)}");
                }
                else
                {
                    categoryList = categoryData.Data;
                    Logger.LogActivity($"REGULATORY CATEGORY DATA - {JsonSerializer.Serialize(categoryList)}");
                }
                categoryList.Entities ??= new();
                var pagedEntities = categoryList.Entities
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(cat => new {
                        id = cat.Id,
                        startTab = "",
                        category = cat.CategoryName,
                        status = cat.IsDeleted ? "Inactive" : "Active",
                        addedon = cat.CreatedAt.ToString("dd-MM-yyyy"),
                        endTab = ""
                    }).ToList();

                if (!string.IsNullOrEmpty(request.SearchTerm)) {
                    pagedEntities = pagedEntities.Where(c => c.category.ToLower().Contains(request.SearchTerm.ToLower())).ToList();
                }

                var totalPages = (int)Math.Ceiling((double)categoryList.TotalCount / categoryList.Size);
                return Ok(new
                {
                    last_page = totalPages,
                    total_records = categoryList.TotalCount,
                    data = pagedEntities
                });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving regulatory category items: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "REGULATORY-CATEGORY-CONTROLLER", ex.StackTrace);
                return Ok(new
                {
                    last_page = 0,
                    data = new List<object>()
                });
            }
        }

        #endregion

        #region Regulatory Types

        public async Task<IActionResult> ComplianceRegulatoryTypes() {
            try
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    var ipAddress = WebHelper.GetCurrentIpAddress();
                    var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                    if (userResponse.HasError || userResponse.Data == null) {
                        TempData["UiResponse"] = JsonSerializer.Serialize(
                            UiResponse.Fail("Your session expired. Please login again.")
                        );
                        return Redirect(Url.Action("Dashboard", "Application"));
                    }

                    var currentUser = userResponse.Data;
                    var userDashboard = new UserDashboardModel
                    {
                        Initials = $"{currentUser.FirstName[..1]} {currentUser.LastName[..1]}",
                        LastLogin = DateTime.Now,
                        Workspace = SessionManager.GetWorkspace(),
                        //..add statistics
                        DashboardStatistics = new()
                    };

                    return View(userDashboard);
                } else {
                    return RedirectToAction("Login", "Application");
                }


            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error loading Compliance Regulatory types view: {ex.Message}", "ERROR");
                TempData["UiResponse"] = JsonSerializer.Serialize(
                            UiResponse.Fail("Error loading Compliance Regulatory types view")
                        );
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }
            
        }

        [LogActivityResult("Retrieve Type", "User retrieved regulatory type", ActivityTypeDefaults.COMPLIACE_RETRIVE_TYPE, "Regulatory_Type")]
        public async Task<IActionResult> GetRegulatoryType(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }

                if (id == 0) {
                    var msg = "Type Id is required";
                    Logger.LogActivity(msg);
                    return BadRequest(new {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }
                var currentUser = userResponse.Data;
                GrcIdRequst getRequest = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_GET_TYPE.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>()
                };

                var result = await _regulatoryTypeService.GetTypeAsync(getRequest);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Unerror occurred while retrieving type";
                    Logger.LogActivity(errMsg);
                    return Ok(new {
                        succuss=false,
                        message = errMsg,
                        data = new { }
                    });
                }

                RegulatoryTypeResponse response = result.Data;
                var categoryRecord = new {
                    id = response.Id,
                    startTab = "",
                    typeName = response.TypeName,
                    status = response.IsDeleted ? "Inactive" : "Active",
                    addedon = response.CreatedAt.ToString("dd-MM-yyyy"),
                    endTab = ""
                };

                return Ok(new {
                    success = true,
                    message = "Type created successfully",
                    data = categoryRecord
                });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected error retrieving type: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return Redirect(Url.Action("ComplianceRegulatoryTypes", "ComplianceSettings"));
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(GrcAntiForgeryTokenAttribute))]
        [LogActivityResult("Add Type", "User added regulatory type", ActivityTypeDefaults.COMPLIACE_CREATE_TYPE, "Regulatory_Type")]
        public async Task<IActionResult> CreateRegulatoryType([FromBody] RegulatoryViewModel request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }

                if (request == null) {
                    var msg = "Invalid request data";
                    Logger.LogActivity(msg);
                    return BadRequest(new {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }

                //..set system fields
                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.CREATEREGULATORYTYPE.GetDescription();

               var result = await _regulatoryTypeService.CreateTypeAsync(request);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Failed to create type";
                    Logger.LogActivity(errMsg);
                    return Ok(new {
                        success = false,
                        message = errMsg,
                        data = new { }
                    });
                }

                var created = result.Data;
                var categoryRecord = new {
                    id = created.Id,
                    startTab = "",
                    typeName = created.TypeName,
                    status = created.IsDeleted ? "Inactive" : "Active",
                    addedon = created.CreatedAt.ToString("dd-MM-yyyy"),
                    endTab = ""
                };

                return Ok(new {
                    success = true,
                    message = "Type created successfully",
                    data = categoryRecord
                });
            }
            catch (Exception ex) {
                Logger.LogActivity($"Unexpected error creating type: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return Redirect(Url.Action("ComplianceRegulatoryTypes", "ComplianceSettings"));
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(GrcAntiForgeryTokenAttribute))]
        [LogActivityResult("Update type", "User modified regulatory type", ActivityTypeDefaults.COMPLIANCE_EDITED_TYPE, "Regulatory_Type")]
        public async Task<IActionResult> UpdateRegulatoryType([FromBody] RegulatoryViewModel request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new {
                        success=false,
                        message = msg,
                        data = new { }
                    });
                }

                if (request == null) {
                    var msg = "Invalid request data";
                    Logger.LogActivity(msg);
                    return BadRequest(new
                    {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }

                //..set system fields
                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.COMPLIANCE_EDITED_TYPE.GetDescription();

                var result = await _regulatoryTypeService.UpdateTypeAsync(request);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Failed to update type";
                    Logger.LogActivity(errMsg);
                    return Ok(new {
                        success = false,
                        message = errMsg,
                        data = new { }
                    });
                }

                var updated = result.Data;
                var typeRecord = new {
                    id = updated.Id,
                    typeName = updated.TypeName,
                    status = updated.IsDeleted ? "Inactive" : "Active",
                    addedon = updated.CreatedAt.ToString("dd-MM-yyyy")
                };

                return Ok(new
                {
                    success = true,
                    message = "Type updated successfully",
                    data = typeRecord
                });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected error updating type: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return Redirect(Url.Action("ComplianceRegulatoryTypes", "ComplianceSettings"));
            }
        }

        [HttpDelete]
        [ServiceFilter(typeof(GrcAntiForgeryTokenAttribute))]
        [LogActivityResult("Delete type", "User deleted regulatory type", ActivityTypeDefaults.COMPLIANCE_DELETED_TYPE, "Regulatory_Type")]
        public async Task<IActionResult> DeleteRegulatoryType(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new {
                        success=false,
                        message = msg,
                        data = new { }
                    });
                }

                if (id == 0) {
                    var msg = "Type Id is required";
                    Logger.LogActivity(msg);
                    return BadRequest(new {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }
                var currentUser = userResponse.Data;
                GrcIdRequst deleteRequest = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_DELETED_TYPE.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>()
                };

                var result = await _regulatoryTypeService.DeleteTypeAsync(deleteRequest);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Failed to delete regulatory type";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg ?? "" });
                }

                ServiceResponse response = result.Data;
                return Ok(new { success = true, message = response.Message });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected error deleting type: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return Redirect(Url.Action("ComplianceRegulatoryTypes", "ComplianceSettings"));
            }
        }

        [HttpPost]
        [LogActivityResult("Export types", "User exported regulatory types to excel", ActivityTypeDefaults.COMPLIANCE_EXPORT_TYPE, "Regulatory_Type")]
        public IActionResult ExcelExportTypes([FromBody] List<RegulatoryTypeResponse> data) {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Regulation Types");

            //..headers
            worksheet.Cell(1, 2).Value = "Type";
            worksheet.Cell(1, 3).Value = "Status";
            worksheet.Cell(1, 4).Value = "Added On";

            int row = 2;
            foreach (var item in data) {
                worksheet.Cell(row, 2).Value = item.TypeName;
                worksheet.Cell(row, 3).Value = item.IsDeleted ? "Inactive" : "Active";
                worksheet.Cell(row, 4).Value = item.CreatedAt.ToString("dd-MM-yyyy");
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "RegulatoryTypes.xlsx"
            );
        }

        [HttpPost]
        [LogActivityResult("Export types", "User exported regulatory types to excel", ActivityTypeDefaults.COMPLIANCE_EXPORT_TYPE, "Regulatory_Type")]
        public async Task<IActionResult> ExcelExportAllTypes() {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null)
            {
                var msg = "Unable to resolve current user";
                Logger.LogActivity(msg);
                return Ok(new
                {
                    success = false,
                    message = msg,
                    data = new { }
                });
            }

            var request = new TableListRequest
            {
                UserId = userResponse.Data.UserId,
                IPAddress = ipAddress,
                PageIndex = 1,
                PageSize = int.MaxValue,
                Action = Activity.COMPLIANCE_EXPORT_TYPES.GetDescription()
            };

            GrcResponse<PagedResponse<RegulatoryTypeResponse>> typeData = await _regulatoryTypeService.GetAllRegulatoryTypes(request);
            if (typeData.HasError || typeData.Data == null)
            {
                var errMsg = typeData.Error?.Message ?? "Failed to retrieve types";
                Logger.LogActivity(errMsg);
                return Ok(new
                {
                    success = false,
                    message = errMsg,
                    data = new { }
                });
            }

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Types");

            //..headers
            ws.Cell(1, 2).Value = "Type";
            ws.Cell(1, 3).Value = "Status";
            ws.Cell(1, 4).Value = "Added On";

            int row = 2;
            foreach (var cat in typeData.Data.Entities)
            {
                ws.Cell(row, 2).Value = cat.TypeName;
                ws.Cell(row, 3).Value = cat.IsDeleted ? "Inactive" : "Active";
                ws.Cell(row, 4).Value = cat.CreatedAt.ToString("dd-MM-yyyy");
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "RegulatoryTypes.xlsx"
            );
        }

        public async Task<IActionResult> AllRegulatoryTypes([FromBody] TableListRequest request)
        {
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    Logger.LogActivity($"REGULATORY TYPES DATA ERROR: Failed to get current user record - {JsonSerializer.Serialize(grcResponse)}");
                }

                //..update with user data
                var currentUser = grcResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.RETRIEVEREGULATORYTYPES.GetDescription();

                //..get regulatory type data
                var typeData = await _regulatoryTypeService.GetAllRegulatoryTypes(request);
                PagedResponse<RegulatoryTypeResponse> typeList = new();

                if (typeData.HasError)
                {
                    Logger.LogActivity($"REGULATORY TYPE DATA ERROR: Failed to retrieve type items - {JsonSerializer.Serialize(typeData)}");
                }
                else
                {
                    typeList = typeData.Data;
                    Logger.LogActivity($"REGULATORY TYPE DATA - {JsonSerializer.Serialize(typeList)}");
                }
                typeList.Entities ??= new();
                var pagedEntities = typeList.Entities
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(t => new {
                        id = t.Id,
                        startTab = "",
                        typeName = t.TypeName,
                        status = t.IsDeleted ? "Inactive" : "Active",
                        addedon = t.CreatedAt.ToString("dd-MM-yyyy"),
                        endTab = ""
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)typeList.TotalCount / typeList.Size);
                return Ok(new {
                    last_page = totalPages,
                    total_records = typeList.TotalCount,
                    data = pagedEntities
                });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving regulatory type items: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "REGULATORY-TYPE-CONTROLLER", ex.StackTrace);
                return Ok(new
                {
                    last_page = 0,
                    data = new List<object>()
                });
            }
        }

        #endregion

        #region Regulatory Authorities

        public async Task<IActionResult> ComplianceAuthorities()
        {
            try
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    var ipAddress = WebHelper.GetCurrentIpAddress();
                    var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                    if (userResponse.HasError || userResponse.Data == null)
                    {
                        return Redirect(Url.Action("Dashboard", "Application"));
                    }

                    var currentUser = userResponse.Data;
                    var userDashboard = new UserDashboardModel
                    {
                        Initials = $"{currentUser.FirstName[..1]} {currentUser.LastName[..1]}",
                        LastLogin = DateTime.Now,
                        Workspace = SessionManager.GetWorkspace(),
                        //..add statistics
                        DashboardStatistics = new()
                    };

                    return View(userDashboard);
                }
                else
                {
                    return RedirectToAction("Login", "Application");
                }


            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error loading Compliance Regulatory Authorities view: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }
            
        }

        [LogActivityResult("Retrieve Authority", "User retrieved regulatory authority", ActivityTypeDefaults.COMPLIACE_RETRIVE_AUTHORITY, "Authorities")]
        public async Task<IActionResult> GetRegulatoryAuthority(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new
                    {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }

                if (id == 0) {
                    var msg = "Authority Id is required";
                    Logger.LogActivity(msg);
                    return BadRequest(new
                    {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }
                var currentUser = userResponse.Data;
                GrcIdRequst getRequest = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_GET_AUTHORITY.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>()
                };

                var result = await _regulatoryAuthorityService.GetAuthorityAsync(getRequest);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Unerror occurred while retrieving authority";
                    Logger.LogActivity(errMsg);
                    return Ok(new {
                        succuss = false,
                        message = errMsg,
                        data = new { }
                    });
                }

                RegulatoryAuthorityResponse response = result.Data;
                var categoryRecord = new {
                    id = response.Id,
                    startTab = "",
                    authorityName = response.AuthorityName,
                    authorityAlias = response.AuthorityAlias,
                    status = response.IsDeleted ? "Inactive" : "Active",
                    addedon = response.CreatedAt.ToString("dd-MM-yyyy"),
                    endTab = ""
                };

                return Ok(new
                {
                    success = true,
                    message = "",
                    data = categoryRecord
                });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected error deleting type: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return Redirect(Url.Action("ComplianceAuthorities", "ComplianceSettings"));
            }
        }

        public async Task<IActionResult> AllRegulatoryAuthorities([FromBody] TableListRequest request) {
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    Logger.LogActivity($"REGULATORY AUTHORITIES DATA ERROR: Failed to get current user record - {JsonSerializer.Serialize(grcResponse)}");
                }

                //..update with user data
                var currentUser = grcResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.COMPLIANCE_RETRIEVE_AUTHORITY.GetDescription();

                //..get regulatory authorities data
                var authoritiesData = await _regulatoryAuthorityService.GetAllRegulatoryAuthorities(request);
                PagedResponse<RegulatoryAuthorityResponse> authoritiesList = new();

                if (authoritiesData.HasError) {
                    Logger.LogActivity($"REGULATORY AUTHORITY DATA ERROR: Failed to retrieve authority items - {JsonSerializer.Serialize(authoritiesData)}");
                }
                else
                {
                    authoritiesList = authoritiesData.Data;
                    Logger.LogActivity($"REGULATORY AUTHORITY DATA - {JsonSerializer.Serialize(authoritiesList)}");
                }
                authoritiesList.Entities ??= new();

                var pagedEntities = authoritiesList.Entities
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(t => new {
                        id = t.Id,
                        startTab = "",
                        authorityName = t.AuthorityName,
                        authorityAlias = t.AuthorityAlias,
                        status = t.IsDeleted ? "Inactive" : "Active",
                        addedon = t.CreatedAt.ToString("dd-MM-yyyy"),
                        endTab = ""
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)authoritiesList.TotalCount / authoritiesList.Size);
                return Ok(new
                {
                    last_page = totalPages,
                    total_records = authoritiesList.TotalCount,
                    data = pagedEntities
                });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving regulatory authority items: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "REGULATORY-AUTHGORITY-CONTROLLER", ex.StackTrace);
                return Ok(new
                {
                    last_page = 0,
                    data = new List<object>()
                });
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(GrcAntiForgeryTokenAttribute))]
        [LogActivityResult("Add Authority", "User added regulatory authority", ActivityTypeDefaults.COMPLIACE_CREATE_AUTHORITY, "Authorities")]
        public async Task<IActionResult> CreateRegulatoryAuthority([FromBody] RegulatoryAuthorityRequest request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new
                    {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }

                if (request == null)
                {
                    var msg = "Invalid request data";
                    Logger.LogActivity(msg);
                    return BadRequest(new
                    {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }

                //..set system fields
                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.COMPLIANCE_CREATE_AUTHORITY.GetDescription();

                var result = await _regulatoryAuthorityService.CreateAuthorityAsync(request);
                if (result.HasError || result.Data == null)
                {
                    var errMsg = result.Error?.Message ?? "Failed to create authority";
                    Logger.LogActivity(errMsg);
                    return Ok(new
                    {
                        success = false,
                        message = errMsg,
                        data = new { }
                    });
                }

                var created = result.Data;
                var categoryRecord = new
                {
                    id = created.Id,
                    startTab = "",
                    authorityName = created.AuthorityName,
                    authorityAlias = created.AuthorityAlias,
                    status = created.IsDeleted ? "Inactive" : "Active",
                    addedon = created.CreatedAt.ToString("dd-MM-yyyy"),
                    endTab = ""
                };

                return Ok(new {
                    success = true,
                    message = "Authority created successfully",
                    data = categoryRecord
                });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected error creating authority: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return Redirect(Url.Action("ComplianceAuthorities", "ComplianceSettings"));
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(GrcAntiForgeryTokenAttribute))]
        [LogActivityResult("Update Authority", "User modified regulatory authority", ActivityTypeDefaults.COMPLIANCE_EDITED_AUTHORITY, "Authorities")]
        public async Task<IActionResult> UpdateRegulatoryAuthority([FromBody] RegulatoryAuthorityRequest request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new
                    {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }

                if (request == null)
                {
                    var msg = "Invalid request data";
                    Logger.LogActivity(msg);
                    return BadRequest(new
                    {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }

                //..set system fields
                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.COMPLIANCE_EDITED_AUTHORITY.GetDescription();

                var result = await _regulatoryAuthorityService.UpdateAuthorityAsync(request);
                if (result.HasError || result.Data == null)
                {
                    var errMsg = result.Error?.Message ?? "Failed to update authority";
                    Logger.LogActivity(errMsg);
                    return Ok(new
                    {
                        success = false,
                        message = errMsg,
                        data = new { }
                    });
                }

                var updated = result.Data;
                var authRecord = new
                {
                    id = updated.Id,
                    authorityName = updated.AuthorityName,
                    authorityAlias = updated.AuthorityAlias,
                    status = updated.IsDeleted ? "Inactive" : "Active",
                    addedon = updated.CreatedAt.ToString("dd-MM-yyyy")
                };

                return Ok(new
                {
                    success = true,
                    message = "Authority updated successfully",
                    data = authRecord
                });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected error updating authority: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return Redirect(Url.Action("ComplianceAuthorities", "ComplianceSettings"));
            }
        }

        [HttpDelete]
        [ServiceFilter(typeof(GrcAntiForgeryTokenAttribute))]
        [LogActivityResult("Delete Authority", "User deleted regulatory authority", ActivityTypeDefaults.COMPLIANCE_DELETED_AUTHORITY, "Authorities")]
        public async Task<IActionResult> DeleteRegulatoryAuthority(long id)
        {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                {
                    var errMsg = userResponse.Error?.Message ?? "Your session has expired. Please login";
                    Logger.LogActivity(errMsg);
                    return Ok(new
                    {
                        success = false,
                        message = errMsg,
                        data = new { }
                    });
                }

                if (id == 0)
                {
                    var msg = "Authority Id is required";
                    Logger.LogActivity(msg);
                    return BadRequest(new
                    {
                        success = false,
                        message = msg,
                        data = new { }
                    });
                }
                var currentUser = userResponse.Data;
                GrcIdRequst deleteRequest = new()
                {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_DELETED_AUTHORITY.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>()
                };

                var result = await _regulatoryAuthorityService.DeleteAuthorityAsync(deleteRequest);
                if (result.HasError || result.Data == null)
                {
                    var errMsg = result.Error?.Message ?? "Unerror occurred while deleting authority";
                    Logger.LogActivity(errMsg);
                    return Ok(new
                    {
                        success = false,
                        message = errMsg,
                        data = new { }
                    });
                }

                var response = result.Data;
                return Ok(new
                {
                    message = response.Message,
                    success = response.Status,
                });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Unexpected error deleting authority: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return Redirect(Url.Action("ComplianceAuthorities", "ComplianceSettings"));
            }
        }

        [HttpPost]
        [LogActivityResult("Export authorities", "User exported regulatory authorities to excel", ActivityTypeDefaults.COMPLIANCE_EXPORT_AUTHORITY, "Authorities")]
        public IActionResult ExcelExportAuthorities([FromBody] List<RegulatoryAuthorityResponse> data)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Regulation Authorities");

            //..headers
            worksheet.Cell(1, 2).Value = "Authority";
            worksheet.Cell(1, 2).Value = "Alias";
            worksheet.Cell(1, 3).Value = "Status";
            worksheet.Cell(1, 4).Value = "Added On";

            int row = 2;
            foreach (var item in data)
            {
                worksheet.Cell(row, 2).Value = item.AuthorityName;
                worksheet.Cell(row, 2).Value = item.AuthorityAlias;
                worksheet.Cell(row, 3).Value = item.IsDeleted ? "Inactive" : "Active";
                worksheet.Cell(row, 4).Value = item.CreatedAt.ToString("dd-MM-yyyy");
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "RegulatoryAuthorities.xlsx"
            );
        }

        [HttpPost]
        [LogActivityResult("Export authorities", "User exported regulatory authorities to excel", ActivityTypeDefaults.COMPLIANCE_EXPORT_AUTHORITY, "Authorities")]
        public async Task<IActionResult> ExcelExportAllAuthorities() {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null)
            {
                var msg = "Unable to resolve current user";
                Logger.LogActivity(msg);
                return Ok(new
                {
                    success = false,
                    message = msg,
                    data = new { }
                });
            }

            var request = new TableListRequest
            {
                UserId = userResponse.Data.UserId,
                IPAddress = ipAddress,
                PageIndex = 1,
                PageSize = int.MaxValue,
                Action = Activity.COMPLIANCE_EXPORT_AUTHORITIES.GetDescription()
            };

            var typeData = await _regulatoryAuthorityService.GetAllRegulatoryAuthorities(request);
            if (typeData.HasError || typeData.Data == null)
            {
                var errMsg = typeData.Error?.Message ?? "Failed to retrieve authorities";
                Logger.LogActivity(errMsg);
                return Ok(new
                {
                    success = false,
                    message = errMsg,
                    data = new { }
                });
            }

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Authorities");

            //..headers
            ws.Cell(1, 2).Value = "Authority";
            ws.Cell(1, 2).Value = "Alias";
            ws.Cell(1, 3).Value = "Status";
            ws.Cell(1, 4).Value = "Added On";

            int row = 2;
            foreach (var cat in typeData.Data.Entities)
            {
                ws.Cell(row, 2).Value = cat.AuthorityName;
                ws.Cell(row, 2).Value = cat.AuthorityAlias;
                ws.Cell(row, 3).Value = cat.IsDeleted ? "Inactive" : "Active";
                ws.Cell(row, 4).Value = cat.CreatedAt.ToString("dd-MM-yyyy");
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "RegulatoryAuthorities.xlsx"
            );
        }

        #endregion

        #region Document Types

        public async Task<IActionResult> ComplianceDocumentType()
        {
            try
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    var ipAddress = WebHelper.GetCurrentIpAddress();
                    var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                    if (userResponse.HasError || userResponse.Data == null)
                    {
                        TempData["UiResponse"] = JsonSerializer.Serialize(
                            UiResponse.Fail("Your session expired. Please login again.")
                        );
                        return Redirect(Url.Action("Dashboard", "Application"));
                    }

                    var currentUser = userResponse.Data;
                    var userDashboard = new UserDashboardModel
                    {
                        Initials = $"{currentUser.FirstName[..1]} {currentUser.LastName[..1]}",
                        LastLogin = DateTime.Now,
                        Workspace = SessionManager.GetWorkspace(),
                        DashboardStatistics = new()
                    };

                    return View(userDashboard);
                }
                else
                {
                    return RedirectToAction("Login", "Application");
                }
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error loading Compliance Document Types view: {ex.Message}", "ERROR");
                TempData["UiResponse"] = JsonSerializer.Serialize(
                    UiResponse.Fail("Error loading Compliance Document Types view")
                );
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDocumentTypes()
        {
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    Logger.LogActivity($"DOCUMENT TYPE LIST ERROR: Failed to Current user record - {JsonSerializer.Serialize(grcResponse)}");
                    return Ok(new { success = false, message = "Unable to resolve current user" });
                }

                var currentUser = grcResponse.Data;
                GrcRequest request = new()
                {
                    UserId = currentUser.UserId,
                    Action = Activity.COMPLIANCE_RETRIEVE_DOCTYPE.GetDescription(),
                    IPAddress = ipAddress,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>()
                };

                //..get list of all branches
                var doctypeData = await _documentTypeService.GetAllAsync(request);

                List<DocumentTypeResponse> documentTypes;
                if (doctypeData.HasError)
                {
                    documentTypes = new();
                    Logger.LogActivity($"DOCUMENT TYPE DATA ERROR: Failed to retrieve branch items - {JsonSerializer.Serialize(doctypeData)}");
                }
                else
                {
                    documentTypes = doctypeData.Data;
                    Logger.LogActivity($"DOCUMENT TYPES DATA - {JsonSerializer.Serialize(documentTypes)}");
                }

                //..get ajax data
                List<object> select2Data = new();
                if (documentTypes.Any())
                {
                    select2Data = documentTypes.Select(type => new {
                        id = type.Id,
                        text = type.TypeName
                    }).Cast<object>().ToList();
                }

                return Json(new { results = select2Data });
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "DOCUMENT-TYPE", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDocumentType(long id)
        {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                {
                    return Ok(new { success = false, message = "Unable to resolve current user" });
                }

                if (id == 0)
                    return BadRequest(new { success = false, message = "Type Id is required" });

                var currentUser = userResponse.Data;
                var request = new GrcIdRequst
                {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    IPAddress = ipAddress,
                    Action = Activity.COMPLIANCE_GET_DOCTYPE.GetDescription(),
                    IsDeleted = false
                };

                var result = await _documentTypeService.GetTypeAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to retrieve document type" });

                var response = result.Data;
                var record = new {
                    id = response.Id,
                    typeName = response.TypeName,
                    status = response.IsDeleted ? "Inactive" : "Active",
                    addedon = response.CreatedAt.ToString("dd-MM-yyyy")
                };

                return Ok(new { success = true, data = record });
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "DOCUMENT-TYPE", ex.StackTrace);
                return Ok(new { success = false, message = "Unexpected error retrieving document type" });
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(GrcAntiForgeryTokenAttribute))]
        [LogActivityResult("Add document type", "User added document type", ActivityTypeDefaults.COMPLIANCE_CREATE_DOCTYPE, "Document")]
        public async Task<IActionResult> CreateDocumentType([FromBody] DocumentTypeViewModel request)
        {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                if (request == null)
                    return BadRequest(new { success = false, message = "Invalid request" });

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.COMPLIANCE_CREATE_DOCTYPE.GetDescription();

                var result = await _documentTypeService.CreateTypeAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to create document type" });

                var created = result.Data;
                var record = new
                {
                    id = created.Id,
                    typeName = created.TypeName,
                    status = created.IsDeleted ? "Inactive" : "Active",
                    addedon = created.CreatedAt.ToString("dd-MM-yyyy")
                };

                return Ok(new { success = true, message = "Document type created successfully", data = record });
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "DOCUMENT-TYPE", ex.StackTrace);
                return Ok(new { success = false, message = "Unexpected error creating document type" });
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(GrcAntiForgeryTokenAttribute))]
        [LogActivityResult("Modify document type", "User modified document type", ActivityTypeDefaults.COMPLIANCE_EDITED_DOCTYPE, "Document")]
        public async Task<IActionResult> UpdateDocumentType([FromBody] DocumentTypeViewModel request)
        {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                if (request == null)
                    return BadRequest(new { success = false, message = "Invalid request" });

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.COMPLIANCE_EDITED_DOCTYPE.GetDescription();

                var result = await _documentTypeService.UpdateTypeAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update document type" });

                var updated = result.Data;
                var record = new
                {
                    id = updated.Id,
                    typeName = updated.TypeName,
                    status = updated.IsDeleted ? "Inactive" : "Active",
                    addedon = updated.CreatedAt.ToString("dd-MM-yyyy")
                };

                return Ok(new { success = true, message = "Document type updated successfully", data = record });
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "DOCUMENT-TYPE", ex.StackTrace);
                return Ok(new { success = false, message = "Unexpected error updating document type" });
            }
        }

        [HttpDelete]
        [ServiceFilter(typeof(GrcAntiForgeryTokenAttribute))]
        [LogActivityResult("Export document types", "User exported document types to excel", ActivityTypeDefaults.COMPLIANCE_DELETED_DOCTYPE, "Document")]
        public async Task<IActionResult> DeleteDocumentType(long id)
        {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var errMsg = userResponse.Error?.Message ?? "Unable to resolve current user";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg });
                }
                
                if (id == 0) {
                    var errMsg = "Type Id is required";
                    Logger.LogActivity(errMsg);
                    return BadRequest(new { success = false, message = errMsg });
                }

                var currentUser = userResponse.Data;
                var request = new GrcIdRequst
                {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    IPAddress = ipAddress,
                    Action = Activity.COMPLIANCE_DELETED_DOCTYPE.GetDescription(),
                    IsDeleted = true
                };

                var result = await _documentTypeService.DeleteTypeAsync(request);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Failed to delete document type";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg });
                }
                
                return Ok(new { success = true, message = result.Data.Message });
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "DOCUMENT-TYPE", ex.StackTrace);
                return Ok(new { success = false, message = "Unexpected error deleting document type" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AllDocumentTypes([FromBody] TableListRequest request)
        {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { last_page = 0, data = new List<object>() });

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.COMPLIANCE_RETRIEVE_DOCTYPE.GetDescription();

                var typeData = await _documentTypeService.GetAllDocumentTypes(request);
                PagedResponse<DocumentTypeResponse> docList = new();

                if (typeData.HasError)
                {
                    Logger.LogActivity($"DOCUMENT TYPES DATA ERROR: Failed to retrieve authority items - {JsonSerializer.Serialize(docList)}");
                }
                else
                {
                    docList = typeData.Data;
                    Logger.LogActivity($"DOCUMENT TYPES DATA - {JsonSerializer.Serialize(docList)}");
                }
                docList.Entities ??= new();

                var pagedEntities = docList.Entities
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(t => new {
                        id = t.Id,
                        startTab = "",
                        typeName = t.TypeName,
                        status = t.IsDeleted ? "Inactive" : "Active",
                        addedon = t.CreatedAt.ToString("dd-MM-yyyy"),
                        endTab = ""
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)docList.TotalCount / docList.Size);
                return Ok(new
                {
                    last_page = totalPages,
                    total_records = docList.TotalCount,
                    data = pagedEntities
                });
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "DOCUMENT-TYPE", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        [HttpPost]
        [LogActivityResult("Export document types", "User exported document types to excel", ActivityTypeDefaults.COMPLIANCE_EXPORT_AUTHORITY, "Document")]
        public IActionResult ExcelExportDoctypes([FromBody] List<DocumentTypeResponse> data)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Document Types");

            //..headers
            worksheet.Cell(1, 2).Value = "Document Type";
            worksheet.Cell(1, 3).Value = "Status";
            worksheet.Cell(1, 4).Value = "Added On";

            int row = 2;
            foreach (var item in data)
            {
                worksheet.Cell(row, 2).Value = item.TypeName;
                worksheet.Cell(row, 3).Value = item.IsDeleted ? "Inactive" : "Active";
                worksheet.Cell(row, 4).Value = item.CreatedAt.ToString("dd-MM-yyyy");
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "RegulatoryAuthorities.xlsx"
            );
        }

        [HttpPost]
        [LogActivityResult("Export document types", "User exported document types to excel", ActivityTypeDefaults.COMPLIANCE_EXPORT_AUTHORITY, "Document")]
        public async Task<IActionResult> ExcelExportAllDoctypes()
        {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null)
            {
                var msg = "Unable to resolve current user";
                Logger.LogActivity(msg);
                return Ok(new
                {
                    success = false,
                    message = msg,
                    data = new { }
                });
            }

            var request = new TableListRequest
            {
                UserId = userResponse.Data.UserId,
                IPAddress = ipAddress,
                PageIndex = 1,
                PageSize = int.MaxValue,
                Action = Activity.COMPLIANCE_EXPORT_DOCTYPES.GetDescription()
            };

            var typeData = await _documentTypeService.GetAllDocumentTypes(request);
            if (typeData.HasError || typeData.Data == null)
            {
                var errMsg = typeData.Error?.Message ?? "Failed to retrieve document types";
                Logger.LogActivity(errMsg);
                return Ok(new
                {
                    success = false,
                    message = errMsg,
                    data = new { }
                });
            }

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Document_Types");

            //..headers
            ws.Cell(1, 2).Value = "Document Type";
            ws.Cell(1, 3).Value = "Status";
            ws.Cell(1, 4).Value = "Added On";

            int row = 2;
            foreach (var cat in typeData.Data.Entities)
            {
                ws.Cell(row, 2).Value = cat.TypeName;
                ws.Cell(row, 3).Value = cat.IsDeleted ? "Inactive" : "Active";
                ws.Cell(row, 4).Value = cat.CreatedAt.ToString("dd-MM-yyyy");
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Document_Types.xlsx"
            );
        }

        #endregion

        #region Users
        public async Task<IActionResult> ComplianceUsers() {
            try
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    var ipAddress = WebHelper.GetCurrentIpAddress();
                    var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                    if (userResponse.HasError || userResponse.Data == null)
                    {
                        return Redirect(Url.Action("Dashboard", "Application"));
                    }

                    var currentUser = userResponse.Data;
                    var userDashboard = new UserDashboardModel
                    {
                        Initials = $"{currentUser.FirstName[..1]} {currentUser.LastName[..1]}",
                        LastLogin = DateTime.Now,
                        Workspace = SessionManager.GetWorkspace(),
                        //..add statistics
                        DashboardStatistics = new()
                    };

                    return View(userDashboard);
                }
                else
                {
                    return RedirectToAction("Login", "Application");
                }

            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error loading Compliance Regulatory Categories view: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }
            
        }

        #endregion

        #region Delegation
        public async Task<IActionResult> ComplianceDelegation()
        {
            try
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    var ipAddress = WebHelper.GetCurrentIpAddress();
                    var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                    if (userResponse.HasError || userResponse.Data == null)
                    {
                        return Redirect(Url.Action("Dashboard", "Application"));
                    }

                    var currentUser = userResponse.Data;
                    var userDashboard = new UserDashboardModel
                    {
                        Initials = $"{currentUser.FirstName[..1]} {currentUser.LastName[..1]}",
                        LastLogin = DateTime.Now,
                        Workspace = SessionManager.GetWorkspace(),
                        //..add statistics
                        DashboardStatistics = new()
                    };

                    return View(userDashboard);
                }
                else
                {
                    return RedirectToAction("Login", "Application");
                }


            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error loading Compliance Regulatory Categories view: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }
            
        }

        #endregion

        #region Departments

        public async Task<IActionResult> ComplianceDepartments()
        {
            try
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    var ipAddress = WebHelper.GetCurrentIpAddress();
                    var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                    if (userResponse.HasError || userResponse.Data == null)
                    {
                        return Redirect(Url.Action("Dashboard", "Application"));
                    }

                    var currentUser = userResponse.Data;
                    var userDashboard = new UserDashboardModel
                    {
                        Initials = $"{currentUser.FirstName[..1]} {currentUser.LastName[..1]}",
                        LastLogin = DateTime.Now,
                        Workspace = SessionManager.GetWorkspace(),
                        //..add statistics
                        DashboardStatistics = new()
                    };

                    return View(userDashboard);
                }
                else
                {
                    return RedirectToAction("Login", "Application");
                }


            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error loading Compliance Departments view: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }
            
        }

        #endregion

        #region Responsibilities
        public async Task<IActionResult> ComplianceResponsibilities()
        {
            try
            {
                if (User.Identity?.IsAuthenticated == true)
                {
                    var ipAddress = WebHelper.GetCurrentIpAddress();
                    var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                    if (userResponse.HasError || userResponse.Data == null)
                    {
                        return Redirect(Url.Action("Dashboard", "Application"));
                    }

                    var currentUser = userResponse.Data;
                    var userDashboard = new UserDashboardModel
                    {
                        Initials = $"{currentUser.FirstName[..1]} {currentUser.LastName[..1]}",
                        LastLogin = DateTime.Now,
                        Workspace = SessionManager.GetWorkspace(),
                        //..add statistics
                        DashboardStatistics = new()
                    };

                    return View(userDashboard);
                }
                else
                {
                    return RedirectToAction("Login", "Application");
                }


            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error loading Compliance Regulatory Categories view: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "COMPLIANCE-SETTINGS", ex.StackTrace);
                return Redirect(Url.Action("Dashboard", "Application"));
            }
            
        }

        #endregion

    }
}

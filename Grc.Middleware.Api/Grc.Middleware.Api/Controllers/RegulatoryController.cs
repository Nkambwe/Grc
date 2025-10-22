using AutoMapper;
using Grc.Middleware.Api.Security;
using Grc.Middleware.Api.Services;
using Grc.Middleware.Api.Services.Compliance.Regulations;
using Grc.Middleware.Api.Services.Compliance.Support;
using Grc.Middleware.Api.Services.Organization;
using Grc.Middleware.Api.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Grc.Middleware.Api.Controllers {

    [ApiController]
    [Route("grc")]
    public class RegulatoryController : GrcControllerBase {

        private readonly IAuthorityService _authorityService;
        private readonly IFrequencyService _frequencyService;
        private readonly IRegulatoryCategoryService _categoryService;
        private readonly IRegulatoryReturnService _returnsService;
        private readonly IRegulatoryTypeService _regulatoryType;
        private readonly IResponsibilityService _responsibilityService;
        private readonly IReturnTypeService _returnTypeService;
        private readonly IStatutoryArticleService _articleService;
        private readonly IStatutoryRegulationService _regulatoryService;
        public RegulatoryController(IObjectCypher cypher,
            IServiceLoggerFactory loggerFactory, 
            IMapper mapper, 
            ICompanyService companyService,
            IAuthorityService authorityService,
            IFrequencyService frequencyService,
            IRegulatoryCategoryService categoryService,
            IRegulatoryReturnService returnsService,
            IRegulatoryTypeService regulatoryType,
            IResponsibilityService responsibilityService,
            IReturnTypeService returnTypeService,
            IStatutoryArticleService articleService,
            IStatutoryRegulationService regulatoryService,
            IEnvironmentProvider environment, 
            IErrorNotificationService errorService, 
            ISystemErrorService systemErrorService) 
            : base(cypher, loggerFactory, mapper, companyService, environment, errorService, systemErrorService) {
            _authorityService = authorityService;
            _frequencyService = frequencyService;
            _categoryService = categoryService;
            _returnsService = returnsService;
            _returnTypeService = returnTypeService;
            _regulatoryType = regulatoryType;
            _articleService = articleService;
            _regulatoryService = regulatoryService;
            _responsibilityService = responsibilityService;
        }

        #region Authorities
        #endregion

        #region Frequencies
        #endregion

        #region Categories
        #endregion

        #region Returns
        #endregion

        #region Regulatory Types
        #endregion

        #region Responsibilities
        #endregion

        #region Return types
        #endregion

        #region Articles
        #endregion

        #region Statutories
        #endregion

    }
}

using AutoMapper;
using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Enums;
using Grc.Middleware.Api.Http.Responses;
using Grc.Middleware.Api.Security;
using Grc.Middleware.Api.Services;
using Grc.Middleware.Api.Services.Organization;
using Grc.Middleware.Api.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Grc.Middleware.Api.Controllers {

    public class GrcControllerBase : ControllerBase  {
        protected readonly IObjectCypher Cypher;
        protected readonly IServiceLogger Logger;
        protected readonly IMapper Mapper;
        protected readonly ICompanyService CompanyService;
        protected readonly IEnvironmentProvider Environment;
        protected readonly IErrorNotificationService ErrorService;
        protected readonly ISystemErrorService SystemErrorService;
        public GrcControllerBase(IObjectCypher cypher,
            IServiceLoggerFactory loggerFactory, 
                                 IMapper mapper,
                                 ICompanyService companyService,
                                 IEnvironmentProvider environment,
                                 IErrorNotificationService errorService,
                                 ISystemErrorService systemErrorService) { 
            Logger = loggerFactory.CreateLogger();
            Mapper = mapper;
            Cypher = cypher;
            Environment = environment;
            ErrorService = errorService;
            CompanyService = companyService;
            SystemErrorService = systemErrorService;
        }

        protected static string GetElapsedTime(DateTime date) {
            var ts = DateTime.Now - date;

            if (ts.TotalSeconds < 60)
                return FormatPeriod((int)ts.TotalSeconds, "second");
            if (ts.TotalMinutes < 60)
                return FormatPeriod((int)ts.TotalMinutes, "minute");
            if (ts.TotalHours < 24)
                return FormatPeriod((int)ts.TotalHours, "hour");
            if (ts.TotalDays < 30)
                return FormatPeriod((int)ts.TotalDays, "day");
            if (ts.TotalDays < 365)
                return FormatPeriod((int)(ts.TotalDays / 30), "month");

            return FormatPeriod((int)(ts.TotalDays / 365), "year");
        }

        private static string FormatPeriod(int coundt, string period)
            => coundt == 1 ? $"1 {period} ago" : $"{coundt} {period}s ago";


        #region Private methods
        protected async Task<ResponseError> HandleErrorAsync(Exception ex)
        {
            Logger.LogActivity($"{ex.Message}", "ERROR");
            Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

            var conpany = await CompanyService.GetDefaultCompanyAsync();
            long companyId = conpany != null ? conpany.Id : 1;
            SystemError errorObj = new()
            {
                ErrorMessage = ex.Message,
                ErrorSource = "SUPPORT-MIDDLEWARE-COTROLLER",
                StackTrace = ex.StackTrace,
                Severity = "CRITICAL",
                ReportedOn = DateTime.Now,
                CompanyId = companyId,
                CreatedOn = DateTime.Now,
                CreatedBy = "SYSTEM",
            };

            //..save error object to the database
            var result = await SystemErrorService.SaveErrorAsync(errorObj);
            var response = new GeneralResponse();
            if (result)
            {
                response.Status = true;
                response.StatusCode = (int)ResponseCodes.SUCCESS;
                response.Message = "Error captured and saved successfully";
                Logger.LogActivity($"SUPPORT-MIDDLEWARE RESPONSE: {JsonSerializer.Serialize(response)}");
            }
            else
            {
                response.Status = false;
                response.StatusCode = (int)ResponseCodes.FAILED;
                response.Message = "Failed to capture error to database. An error occurrred";
                Logger.LogActivity($"SUPPORT-MIDDLEWARE-COTROLLER RESPONSE: {JsonSerializer.Serialize(response)}");
            }

            return new ResponseError(
                ResponseCodes.BADREQUEST,
                "Oops! Something went wrong",
                $"System Error - {ex.Message}"
            );
        }
        #endregion
    }
}

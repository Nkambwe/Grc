using AutoMapper;
using Grc.Middleware.Api.Security;
using Grc.Middleware.Api.Services;
using Grc.Middleware.Api.Utils;
using Microsoft.AspNetCore.Mvc;

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
        
    }
}

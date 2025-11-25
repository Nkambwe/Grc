using Grc.Middleware.Api.Data.Entities.Logging;
using Grc.Middleware.Api.Data.Entities.System;

namespace Grc.Middleware.Api.Data.Repositories {

    public interface IActivityLogRepository : IRepository<ActivityLog> {
        Task ClearAllActivitiesAsync();
    }

    public interface IMailSettings : IRepository<MailSettings>
    {

    }
    public interface IMailRecord : IRepository<MailRecord>
    {

    }

}

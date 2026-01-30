using Grc.Middleware.Api.Data.Entities.System;

namespace Grc.Middleware.Api.Data.Repositories {

    public interface IMailRecordRepository : IRepository<MailRecord> {

        Task<DateTime?> GetLastNotificationDateAsync(long id, string type="");
        Task<int> GetReturnNotificationCountAsync(long returnId);
    }

}

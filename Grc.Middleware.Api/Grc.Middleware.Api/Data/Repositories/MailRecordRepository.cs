using Grc.Middleware.Api.Data.Entities.System;
using Grc.Middleware.Api.Utils;
using Microsoft.EntityFrameworkCore;

namespace Grc.Middleware.Api.Data.Repositories {
    public class MailRecordRepository : Repository<MailRecord>, IMailRecordRepository
    {
        protected GrcContext Context;
        public MailRecordRepository(IServiceLoggerFactory loggerFactory, GrcContext _context)
            : base(loggerFactory, _context) {

            Context =_context;
        }

        public async Task<DateTime?> GetLastNotificationDateAsync(long id, string type="") {
            try {
                MailRecord record = null;

                record = type switch {
                    "Policy" => await Context.MailRecords
                                                .Where(m => m.DocumentId == id)
                                                .OrderByDescending(m => m.CreatedOn)
                                                .FirstOrDefaultAsync(),
                    "Return" => await Context.MailRecords
                                                .Where(m => m.ReturnId == id)
                                                .OrderByDescending(m => m.CreatedOn)
                                                .FirstOrDefaultAsync(),
                    "Circular" => await Context.MailRecords
                                                .Where(m => m.CircularId == id)
                                                .OrderByDescending(m => m.CreatedOn)
                                                .FirstOrDefaultAsync(),

                    _ => null,
                };
                return record?.CreatedOn;
            } catch (Exception ex) {
                Logger.LogActivity($"Count operation failed: {ex.Message}", "DbAction");
                Logger.LogActivity($"STACKTRACE :: {ex.StackTrace}", "DbStacktrace");
            }

            return null;
        }

        public async Task<int> GetReturnNotificationCountAsync(long returnId) {
            return await Context.MailRecords
                .Where(m => m.ReturnId == returnId && !m.IsDeleted)
                .CountAsync();
        }

    }

}

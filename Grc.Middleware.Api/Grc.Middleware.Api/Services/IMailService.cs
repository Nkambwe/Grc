using Grc.Middleware.Api.Data.Entities.System;

namespace Grc.Middleware.Api.Services
{
    public interface IMailService {
        Task<MailSettings> GetMailSettingsAsync();
        Task<bool> InsertMailAsync(MailRecord mail);
    }
}

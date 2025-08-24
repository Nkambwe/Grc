using Grc.Middleware.Api.Data.Entities.System;

namespace Grc.Middleware.Api.Services {
    public interface IErrorNotificationService {
        Task NotifyNewErrorAsync(SystemError errorModel);
        Task NotifyCountAsync(int total, int count);
    }
}

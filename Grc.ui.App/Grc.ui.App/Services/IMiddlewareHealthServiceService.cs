namespace Grc.ui.App.Services {
    public interface IMiddlewareHealthService {
        Task<(bool status, bool isConnected, bool hasCompanie)> CheckMiddlewareStatusAsync();
    }
}
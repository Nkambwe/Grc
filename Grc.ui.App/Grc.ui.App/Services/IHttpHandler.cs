using Grc.ui.App.Http.Responses;

namespace Grc.ui.App.Services {

    public interface IHttpHandler {
        /// <summary>
        /// Send get request
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        Task<GrcResponse<TResponse>> GetAsync<TResponse>(string endpoint) where TResponse: class;
        /// <summary>
        /// Send Patch request
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="endpoint"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<GrcResponse<TResponse>> PatchAsync<TRequest, TResponse>(string endpoint, TRequest data)where TResponse : class;
        /// <summary>
        /// Update record without response
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="endpoint"></param>
        /// <param name="data"></param>
        /// <remarks>
        /// Use Cases:
        /// await PatchAsync("users/123", new { status = "active" });
        /// Toggle feature flags
        /// await PatchAsync("users/123/preferences", new { darkMode = true });
        /// Update order status
        /// await PatchAsync("approve/456", new { status = "APPROVED" });
        /// pdate app settings
        /// await PatchAsync("settings/notifications", new { emailEnabled = false });
        /// Update user preferences
        /// await PatchAsync("users/123/settings", new { language = "en-US" });
        /// Toggle feature
        /// await PatchAsync("features/dark-mode", new { enabled = true });
        /// Update profile partially (you already have the data locally)
        /// await PatchAsync("users/123/profile", new { firstName = "John" });
        /// Audit/Tracking Updates
        /// Update last accessed timestamp
        /// await PatchAsync("documents/123", new { lastAccessed = DateTime.UtcNow });
        /// Mark as read
        /// await PatchAsync("messages/456", new { isRead = true });
        /// Update activity status
        /// await PatchAsync("users/123", new { isLoggedin = true });
        /// </remarks>
        Task PatchAsync<TRequest>(string endpoint, TRequest data);
        /// <summary>
        /// Send post request
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="endpoint"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<GrcResponse<TResponse>> PostAsync<TRequest, TResponse>(string endpoint, TRequest data) where TResponse: class;
        /// <summary>
        /// Send post request
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="endpoint"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task PostAsync<TRequest>(string endpoint, TRequest data);
        /// <summary>
        /// Send put request
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="endpoint"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<GrcResponse<TResponse>> PutAsync<TRequest, TResponse>(string endpoint, TRequest data) where TResponse : class;
        /// <summary>
        /// Send put request
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <param name="endpoint"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task PutAsync<TRequest>(string endpoint, TRequest data);
        
        /// <summary>
        /// Delete request with response
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        Task<GrcResponse<TResponse>> DeleteAsync<TResponse>(string endpoint) where TResponse: class;

        /// <summary>
        /// Delete all items in a collection with no response
        /// </summary>
        /// <param name="endpoint">Endpoint to reach to</param>
        /// <remarks>
        /// Use cases:
        /// await DeleteAsync("users/123/notifications"); // Delete all notifications for user 123
        /// await DeleteAsync("logs/old"); // Delete old log entries
        /// Clear cache
        /// await DeleteAsync("cache/users");
        /// await DeleteAsync("cache/products");
        /// Logout (delete session)
        /// await DeleteAsync("auth/logout");
        /// await DeleteAsync("sessions/current");
        /// </remarks>
        Task DeleteAllAsync(string endpoint);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="method"></param>
        /// <param name="endpoint"></param>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        Task<GrcResponse<TResponse>> SendAsync<TResponse>(HttpMethod method, string endpoint, object requestBody = null) where TResponse: class;

        /// <summary>
        /// Generic method for custom HTTP methods without response. Eg. Send Notifications
        /// </summary>
        /// <param name="method">HTTP Method</param>
        /// <param name="endpoint">Endpoint to reach to</param>
        /// <param name="requestBody">Request Body</param>
        /// <remarks>
        /// There are several valid scenarios where you'd send HTTP requests without expecting or needing a response body:
        /// Use Cases:
        /// Logging events - you don't need confirmation
        /// await PostAsync("analytics/track-event", new { userId = 123, action = "login" });
        /// Sending notifications - just fire and forget
        /// await PostAsync("notifications/send", new { userId = 123, message = "Welcome" });
        /// DELETE operations often just return 204 No Content
        /// await DeleteAsync("users/123"); // Success = 204, Error = 404/500
        /// When you don't need to parse response JSON
        /// await PostAsync("audit/log", auditData); // Faster - no deserialization
        /// Bulk operations where individual responses aren't needed
        /// foreach(var item in items) {
        ///   await PostAsync("queue/add", item); // Just need to know it succeeded
        /// }
        /// </remarks>
        /// <returns></returns>
        Task SendAsync(HttpMethod method, string endpoint, object requestBody = null);
    }
}

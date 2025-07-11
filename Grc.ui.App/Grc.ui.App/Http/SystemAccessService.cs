using Grc.ui.App.Http.Requests;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;

namespace Grc.ui.App.Http {

    public class SystemAccessService : GrcBaseService, ISystemAccessService {

        public SystemAccessService(IApplicationLoggerFactory loggerFactory, 
                                   IHttpClientFactory httpClientFactory,
                                   IEnvironmentProvider environment, 
                                   IEndpointTypeProvider endpointType)
            : base(loggerFactory, httpClientFactory, environment,endpointType) {
            Logger.Channel = $"SAM-{DateTime.Now:yyyyMMddHHmmss}";
        }

        /// <summary>
        /// Update user status (no response needed)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task UpdateUserStatusAsync(int userId, string status) {
            await PatchAsync($"users/{userId}", new { status });
        }

        /// <summary>
        /// Toggle user preference (no response needed)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        public async Task ToggleNotificationsAsync(int userId, bool enabled) {
            await PatchAsync($"users/{userId}/preferences", new { notifications = enabled });
        }

        /// <summary>
        /// Update last login (fire and forget)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task UpdateLastLoginAsync(int userId) {
            await PatchAsync($"users/{userId}", new { lastLogin = DateTime.UtcNow });
        }

        /// <summary>
        ///  When you DO need the response (use the other overload)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<UserModel> UpdateUserProfileAsync(int userId, UpdateProfileRequest request) {
            return await PatchAsync<UpdateProfileRequest, UserModel>($"users/{userId}", request);
        }
        /// <summary>
        ///  Delete specific user (would use DeleteAsync<User>("users/123"))
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <returns></returns>
        public async Task DeleteUserAsync(int userId) {
            await DeleteAsync($"users/{userId}");
        }

        /// <summary>
        /// Delete all user's notifications (uses endpoint-only version)
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <returns></returns>
        public async Task ClearNotificationsAsync(int userId) {
            await DeleteAsync($"users/{userId}/notifications");
        }

        /// <summary>
        /// Clear user's views (uses endpoint-only version)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task ClearViewsAsync(int userId) {
            await DeleteAsync($"users/{userId}/views");
        }

        /// <summary>
        /// Logout user (uses endpoint-only version)
        /// </summary>
        /// <returns></returns>
        public async Task LogoutAsync() {
            await DeleteAsync(EndpointType.Sam.Logout);
        }
    }

}

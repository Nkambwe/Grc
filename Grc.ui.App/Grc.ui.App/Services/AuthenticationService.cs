using AutoMapper;
using Grc.ui.App.Enums;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using NuGet.Protocol.Plugins;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Grc.ui.App.Services {

    public class AuthenticationService : GrcBaseService, IAuthenticationService {

        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationService(
                    IHttpContextAccessor httpContextAccessor,
                    IApplicationLoggerFactory loggerFactory, 
                    IHttpHandler httpHandler, 
                    IEnvironmentProvider environment, 
                    IEndpointTypeProvider endpointType, 
                    IMapper mapper) 
                    : base(loggerFactory, httpHandler, environment, endpointType, mapper) {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<GrcResponse<UserModel>> AuthenticateAsync(LoginModel model, string ipAddress) {
            //..validate login request
            if(model == null) {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "Request record cannot be empty",
                    "Invalid user request"
                );
        
                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<UserModel>(error);
            }

            try {
                
                var request = Mapper.Map<UserSignInRequest>(model);
                request.IPAddress = ipAddress;

                var endpoint = $"{EndpointProvider.Sam.Users}/auth";
                var response = await HttpHandler.PostAsync<UserSignInRequest, UserModel>(endpoint, request);
                if (!response.HasError)
                    return null;

                return new GrcResponse<UserModel>(response.Data); 
            } catch (Exception ex) {
                Logger.LogActivity($"Authentication failed: {ex.Message}", "ERROR");
                throw new GRCException("Uanble to authenticate user.", ex);
            }
        }

        public async Task<GrcResponse<UserModel>>GetCurrentUserAsync() {
            try {
                var endpoint = $"{EndpointProvider.Sam.Users}/current-user";
                var response = await HttpHandler.GetAsync<UserResponse>(endpoint);
                if (response.HasError) {
                    Logger.LogActivity($"Failed to fetch current user: {response.Error?.Message}", "WARNING");
                    return null;
                }

                return new GrcResponse<UserModel>(Mapper.Map<UserModel>(response.Data)); 
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"GetCurrentUser failed: {httpEx.Message}", "ERROR");
                throw new GRCException("Could not retrieve current user. Network issue.", httpEx);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error in GetCurrentUser: {ex.Message}", "ERROR");
                throw new GRCException("An error occurred while getting the current user.", ex);
            }
        }

        public async Task<bool> IsSignedIn() {
            try {
                var endpoint = $"{EndpointProvider.Sam.Sambase}/auth/status";
                var response = await HttpHandler.GetAsync<AuthStatusResponse>(endpoint);

                if (response.HasError) {
                    Logger.LogActivity($"Auth status check failed: {response.Error?.Message}", "WARNING");
                    return false;
                }

                return response.Data?.IsSignedIn ?? false;
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"IsSignedIn failed (network): {httpEx.Message}", "ERROR");
                return false;
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected IsSignedIn error: {ex.Message}", "ERROR");
                return false;
            }
        }

        public async Task SignInAsync(UserModel user, bool isPersistent = false) {
            if (user == null) 
                throw new ArgumentNullException(nameof(user));

            try {
                 var claims = new List<Claim>{
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Name, user.EmailAddress),
                    new("FullName", $"{user.FirstName} {user.LastName}" ?? ""),
                    new(ClaimTypes.Role, $"{user.RoleId}" ?? "User")
                 };

                var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                var authProperties = new AuthenticationProperties {
                    IsPersistent = isPersistent,
                    AllowRefresh = true,
                    IssuedUtc = DateTimeOffset.UtcNow,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24)
                };

                await _httpContextAccessor.HttpContext.SignInAsync("Cookies",
                    new ClaimsPrincipal(claimsIdentity), 
                    authProperties);

            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"SignIn failed (network): {httpEx.Message}", "ERROR");
                throw new GRCException("Unable to sign in. Please check your connection.", httpEx);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected SignIn error: {ex.Message}", "ERROR");
                throw new GRCException("An unexpected error occurred during sign in.", ex);
            }
        }

        public async Task SignOutAsync() {
            try {
                var endpoint = $"{EndpointProvider.Sam.Sambase}/auth/signout";
                await HttpHandler.PostAsync<object>(endpoint, null);
                Logger.LogActivity("User signed out successfully.");

                 var httpContext = _httpContextAccessor.HttpContext!;
                 await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            } catch (HttpRequestException httpEx) {
                Logger.LogActivity($"SignOut failed (network): {httpEx.Message}", "ERROR");
                throw new GRCException("Failed to sign out. Network issue.", httpEx);
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected SignOut error: {ex.Message}", "ERROR");
                throw new GRCException("An unexpected error occurred during sign out.", ex);
            }
        }

    }

}

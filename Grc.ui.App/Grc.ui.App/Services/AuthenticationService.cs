using AutoMapper;
using Grc.ui.App.Enums;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Text.Json;

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
  
        public async Task<GrcResponse<UserModel>> GetUserByUsernameAsync(string username, string ipAddress) {

            if(string.IsNullOrWhiteSpace(username)) {
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    "Username is required",
                    "Invalid user request"
                );
        
                Logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                return new GrcResponse<UserModel>(error);
            }

            try {
                
                var request = new UserByNameRequest() {
                    UserId = 0,
                    Username = username,
                    IPAddress = ipAddress,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>(),
                };

                var endpoint = $"{EndpointProvider.Sam.Users}/auth";
                return await HttpHandler.PostAsync<UserByNameRequest, UserModel>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Failed to retrieve user record for {username}: {ex.Message}", "ERROR");
                throw new GRCException("Uanble to retrieve user.", ex);
            }
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
                return await HttpHandler.PostAsync<UserSignInRequest, UserModel>(endpoint, request);
            } catch (Exception ex) {
                Logger.LogActivity($"Authentication failed: {ex.Message}", "ERROR");
                throw new GRCException("Uanble to authenticate user.", ex);
            }
        }

        public async Task<GrcResponse<UserModel>> GetCurrentUserAsync(string ipAddress) {

             Logger.LogActivity($"Get Current Loggedin User", "INFO");
            try {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext?.User?.Identity?.IsAuthenticated != true) {
                    var error = new GrcResponseError(
                        GrcStatusCodes.UNAUTHORIZED,
                        "User not authenticated",
                        "No authenticated user found"
                    );
                    return new GrcResponse<UserModel>(error);
                }

                var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var username = httpContext.User.FindFirst(ClaimTypes.Name)?.Value;
                var email = httpContext.User.FindFirst(ClaimTypes.Email)?.Value;
                var displayName = httpContext.User.FindFirst("DisplayName")?.Value;
                var firstName = httpContext.User.FindFirst("FirstName")?.Value;
                var lastName = httpContext.User.FindFirst("LastName")?.Value;
                var roleName = httpContext.User.FindFirst(ClaimTypes.Role)?.Value;
                var roleId = httpContext.User.FindFirst("RoleId")?.Value;

                // If basic claims are available, use them
                if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(username)) {
                    long id = long.Parse(userId);
                    var userModel = new UserModel {
                        UserId = id,
                        UserName = username,
                        EmailAddress = email,
                        DisplayName = displayName,
                        FirstName = firstName,
                        LastName = lastName,
                        RoleName = roleName,
                        IsLogged = true
                    };

                    return new GrcResponse<UserModel>(userModel);
                }

                //..fetch from the DB for missing claims
                var userIdString = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdString) && long.TryParse(userIdString, out long currentId)) {

                    var endpoint = $"{EndpointProvider.Sam.Users}/getById";
            
                    // Add authorization header with current user's token or use a service account
                    var model = new UserByIdRequest() {
                        UserId = currentId,
                        RecordId = currentId,
                        IPAddress = ipAddress,
                        EncryptFields = Array.Empty<string>(),
                        DecryptFields = Array.Empty<string>(),
                    };
                    return await HttpHandler.PostAsync<UserByIdRequest, UserModel>(endpoint,model);
                }

                //..fallback error
                var fallbackError = new GrcResponseError(
                    GrcStatusCodes.NOTFOUND,
                    "User information not found",
                    "Unable to retrieve current user information"
                );
                return new GrcResponse<UserModel>(fallbackError);
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving current user Info: {ex.Message}", "Error");
                var error = new GrcResponseError(
                    GrcStatusCodes.SERVERERROR,
                    "Error retrieving user information",
                    ex.Message
                );

                return new GrcResponse<UserModel>(error);
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
                    new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new(ClaimTypes.Name, user.EmailAddress),
                    new("DisplayName", $"{user.FirstName}" ?? ""),
                    new("FirstName", $"{user.FirstName}" ?? ""),
                    new("LastName", $"{user.LastName}" ?? ""),
                    new(ClaimTypes.Role, $"{user.RoleName}" ?? "User"),
                    new("RoleId", $"{user.RoleId}" ?? ""),
                    new("PhoneNumber", $"{user.PhoneNumber}" ?? ""),
                    new("PFNumber", $"{user.PFNumber}" ?? ""),
                    new("UnitCode", $"{user.UnitCode}" ?? ""),
                    new("DepartmentId", $"{user.DepartmentId}" ?? "")

                 };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
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

        public async Task SignOutAsync(LogoutModel model) {
            try {

                var request = Mapper.Map<LogoutRequest>(model);
                var endpoint = $"{EndpointProvider.Sam.Users}/logout";
                var response = await HttpHandler.PostAsync<LogoutRequest, StatusResponse>(endpoint, request);
                if(response.HasError) { 
                    Logger.LogActivity($"Failed to signout user on server. {response.Error.Message}");
                } else {
                    Logger.LogActivity("User signed out successfully.");
                }
                
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

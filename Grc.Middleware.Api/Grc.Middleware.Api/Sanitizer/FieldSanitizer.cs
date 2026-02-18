using Grc.Middleware.Api.Enums;
using Grc.Middleware.Api.Http.Responses;
using Grc.Middleware.Api.Utils;
using System.Text.Json;

namespace Grc.Middleware.Api.Sanitizer {
    public static class FieldSanitizer {

        public static (bool, GrcResponse<GeneralResponse>) SanitizeField(IServiceLogger logger, string fieldValue, string ipAddress) {
            bool safe = true;
            GrcResponse<GeneralResponse> response = null;
            //..check for malicious patterns before processing
            if (DataSanitizer.ContainsPathTraversal(fieldValue)) {
                safe = false;
                //..log the attack
                logger.LogActivity($"SECURITY ATTEMPT BLOCKED: XSS payload from IP {ipAddress}", "SECURITY");

                //..return error to user
                var error = new ResponseError(ResponseCodes.BADREQUEST, "Invalid input", "Input contains invalid characters");
                logger.LogActivity($"BAD REQUEST: {JsonSerializer.Serialize(error)}");
                response = new GrcResponse<GeneralResponse>(error);
            }

            if (DataSanitizer.ContainsXSS(fieldValue)) {
                safe = false;
                logger.LogActivity($"SECURITY ALERT: XSS attempt detected: {fieldValue} IP {ipAddress}", "WARNING");

                //..reject the request
                var error = new ResponseError(ResponseCodes.BADREQUEST, "Invalid input", "Input contains potentially harmful content");
                response = new GrcResponse<GeneralResponse>(error);
            }

            //..check for SQL injection
            if (DataSanitizer.ContainsSQLInjection(fieldValue)) {
                safe = false;
                logger.LogActivity($"SECURITY ALERT: SQL injection attempt detected from IP {ipAddress}", "WARNING");
                var error = new ResponseError(ResponseCodes.BADREQUEST, "Invalid input", "Input contains invalid characters");
                response = new GrcResponse<GeneralResponse>(error);
            }

            return (safe, response);
        }

    }
}

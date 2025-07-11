using System.ComponentModel;

namespace Grc.Middleware.Api.Enums {

    public enum ResponseCodes {
        [Description("Request was successful")]
        SUCCESS = 200,
        [Description("Resource created successfully")]
        CREATED = 201,
        [Description("Resource not found")]
        NOTFOUND =404,
        [Description("Resource duplication")]
        DUPLICATE = 409,
        [Description("Internal Server error")]
        SERVERERROR = 500,
        [Description("Unauthorized action")]
        UNAUTHORIZED = 401,
        [Description("Client error. Bad request")]
        BADREQUEST = 400,
        [Description("Action not allowed")]
        FORBIDDEN = 403,
        [Description("Password has expired")]
        PASSWORDEXPIRED = 145,
        [Description("Policy Violation")]
        POLICYVIOLATION = 700
    }

}

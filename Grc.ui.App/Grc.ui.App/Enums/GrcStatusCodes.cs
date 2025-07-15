using System.ComponentModel;

namespace Grc.ui.App.Enums {

    public enum GrcStatusCodes {
        [Description("Processing")]
        PROCESSING = 102,
        [Description("Password has expired")]
        PASSWORDEXPIRED = 103,
        [Description("Policy Violation")]
        POLICYVIOLATION = 104,
        [Description("Successful")]
        SUCCESS = 200,
        [Description("Created")]
        CREATED = 201,
        [Description("Recieved")]
        ACCEPTED = 202,
        [Description("Client Error")]
        BADREQUEST = 400,
        [Description("Unauthorized")]
        UNAUTHORIZED = 401,
        [Description("Action not allowed")]
        NOTALLOWED = 403,
        [Description("Resource not found")]
        NOTFOUND = 404,
        [Description("Invalid action")]
        FORBIDEN = 405,
        [Description("Timeout")]
        TIMEOUT = 406,
        [Description("Resource duplication")]
        DUPLICATE = 409,
        [Description("Content to large")]
        TOOLARGE = 413,
        [Description("Resource is retricted")]
        RESTRICTED = 423,
        [Description("Internal Server Error")]
        SERVERERROR = 500,
        [Description("Not supported")]
        NOTIMPLEMENTED = 501,
        [Description("Middleware unreacheable")]
        BADGATEWAY = 502,
        [Description("Service Unavailable")]
        SERVICEUNVAILABLE = 503,
        [Description("Network Authentication Required")]
        AUTHENTICATIONREQUIRED = 511

    }
}

using Grc.Middleware.Api.Enums;

namespace Grc.Middleware.Api.Http.Responses {

    public class ResponseError {

        public int Code { get; internal set;}

        public string Message { get; internal set; }

        public string Description { get; internal set; }
        
        public ResponseError(ResponseCodes code, string message, string description ) { 
            Code = (int)code;
            Message = message;
            Description = description;
        }

    }
}

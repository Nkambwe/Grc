using Grc.Middleware.Api.Enums;

namespace Grc.Middleware.Api.Exceptions {
    public class GrcError {
        public int Code { get; internal set;}
        public string Message { get; internal set; }
        public string Description { get; internal set; }
        
        public GrcError(ResponseCodes code, string message, string description ) { 
            Code = (int)code;
            Message = message;
            Description = description;
        }
    }
}

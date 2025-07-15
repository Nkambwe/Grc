using Grc.ui.App.Enums;

namespace Grc.ui.App.Http.Responses {
    public class GrcResponseError {
        public int Code { get; internal set;}
        public string Message { get; internal set; }
        public string Description { get; internal set; }
        
        public GrcResponseError(GrcStatusCodes code, string message, string description ) { 
            Code = (int)code;
            Message = message;
            Description = description;
        }
    }
}

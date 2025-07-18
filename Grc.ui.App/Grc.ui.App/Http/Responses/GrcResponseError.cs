using Grc.ui.App.Enums;
using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcResponseError {
        public int Code { get; set;}
        public string Message { get; set; }
        public string Description { get; set; }
        
        /// <summary>
        /// Default C'tr
        /// </summary>
        [JsonConstructor]
        public GrcResponseError(int code, string message, string description) {
            Code = code;
            Message = message;
            Description = description;
        }
    

        public GrcResponseError(GrcStatusCodes code, string message, string description ) { 
            Code = (int)code;
            Message = message;
            Description = description;
        }
    }
}

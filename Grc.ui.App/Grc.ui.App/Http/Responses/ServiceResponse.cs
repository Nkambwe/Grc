namespace Grc.ui.App.Http.Responses {

    public class ServiceResponse{
        public bool Status { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }

        /// <summary>
        /// Default C'tr
        /// </summary>
        public ServiceResponse() { }
       
    }   
}

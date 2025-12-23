namespace Grc.Middleware.Api.Http.Responses {

    /// <summary>
    /// GRC Class handling responses
    /// </summary>
    /// <typeparam name="T">Success Data object type</typeparam>
    /// <remarks>
    /// ...successful response
    /// var userResponse = new GrcResponse<User>(new User { Name = "John", Id = 1 });
    /// ...failed response
    /// var userErrorResponse = new GrcResponse<User>(new GrcError(ResponseCodes.UNAUTHORIZED, 
    /// "You're not authorized to access resource",
    /// "Your profile is not configure for resources above 40,000,000 UGX"));
    /// </remarks>
    public class GrcResponse<T> where T : class {
        public bool HasError { get; }
        public ResponseError Error { get; set; }
        public T Data { get; set; }

        /// <summary>
        /// Ctr for success responses
        /// </summary>
        /// <param name="data">Success response data object</param>
        public GrcResponse(T data) {
            HasError = false;
            Data = data;
            Error = null;
        }

        /// <summary>
        /// Ctr for failed responses
        /// </summary>
        /// <param name="error">Error response data object</param>
        public GrcResponse(ResponseError error) {
            HasError = true;
            Error = error;
            Data = null;
        }
    }
}

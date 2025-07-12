namespace Grc.ui.App.Infrastructure {
    public interface IWebHelper {
        /// <summary>
        /// Gets a value that indicates whether the client is being redirected to a new location
        /// </summary>
        bool IsRequestBeingRedirected {get; }

        /// <summary>
        /// Gets or sets a value that indicates whether the client is being redirected to a new location using POST
        /// </summary>
        bool IsPostBeingDone {get;set; }

        /// <summary>
        /// Gets a value indicating whether current connection is secured
        /// </summary>
        /// <returns>True if it's secured, otherwise false</returns>
        bool IsCurrentConnectionSecured();
        
        /// <summary>
        /// Get IP address from HTTP context
        /// </summary>
        /// <returns>String of IP address</returns>
        string GetCurrentIpAddress();
        
        /// <summary>
        /// Gets whether the specified HTTP request URI references the local host.
        /// </summary>
        /// <param name="req">HTTP request</param>
        /// <returns>True, if HTTP request URI references to the local host</returns>
        bool IsLocalRequest(HttpRequest req);

        /// <summary>
        /// Gets whether the request is made with AJAX 
        /// </summary>
        /// <param name="request">HTTP request</param>
        /// <returns>Result</returns>
        bool IsAjaxRequest(HttpRequest request);

        /// <summary>
        /// Get URL referrer if exists
        /// </summary>
        /// <returns>URL referrer</returns>
        string GetUrlReferrer();
    }

}

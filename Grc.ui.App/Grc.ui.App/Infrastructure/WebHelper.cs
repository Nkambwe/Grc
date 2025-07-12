using Grc.ui.App.Defaults;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Net.Http.Headers;
using System.Net;

namespace Grc.ui.App.Infrastructure {

    public class WebHelper : IWebHelper {
        private readonly IHttpContextAccessor _httpContext;

        public WebHelper(IHttpContextAccessor httpContext) {
            _httpContext = httpContext;
        }
        
        /// <summary>
        /// Gets whether the request is made with AJAX 
        /// </summary>
        /// <param name="request">HTTP request</param>
        /// <returns>Result</returns>
        public virtual bool IsAjaxRequest(HttpRequest request) {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Headers == null)
                return false;

            return request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
        
        public virtual string GetUrlReferrer() {
            if (!IsRequestAvailable())
                return string.Empty;

            //URL referrer is null in some case (for example, in IE 8)
            return _httpContext.HttpContext.Request.Headers[HeaderNames.Referer];
        }

        public virtual bool IsCurrentConnectionSecured() {
            if (!IsRequestAvailable())
                return false;

            return _httpContext.HttpContext.Request.IsHttps;
        }


        public virtual bool IsLocalRequest(HttpRequest req) {
            var connection = req.HttpContext.Connection;
            if (IsIpAddressSet(connection.RemoteIpAddress)) {
                //We have a remote address set up
                return IsIpAddressSet(connection.LocalIpAddress)
                    //Is local is same as remote, then we are local
                    ? connection.RemoteIpAddress.Equals(connection.LocalIpAddress)
                    //else we are remote if the remote IP address is not a loopback address
                    : IPAddress.IsLoopback(connection.RemoteIpAddress);
            }

            return true;
        }

        public virtual bool IsRequestBeingRedirected {
            get {
                var response = _httpContext.HttpContext.Response;
                int[] redirectionStatusCodes = { StatusCodes.Status301MovedPermanently, StatusCodes.Status302Found };               
                return redirectionStatusCodes.Contains(response.StatusCode);
            }
        }

        
        public virtual bool IsPostBeingDone {
            get {
                if (_httpContext.HttpContext.Items[HttpDefaults.IsPostBeingDoneRequestItem] == null)
                    return false;
                return Convert.ToBoolean(_httpContext.HttpContext.Items[HttpDefaults.IsPostBeingDoneRequestItem]);
            }

            set => _httpContext.HttpContext.Items[HttpDefaults.IsPostBeingDoneRequestItem] = value;
        }

        
        public virtual string GetCurrentIpAddress() {
            if (!IsRequestAvailable())
                return string.Empty;

            if(_httpContext.HttpContext.Connection?.RemoteIpAddress is not IPAddress remoteIp)
                return "";

            if(remoteIp.Equals(IPAddress.IPv6Loopback))
                return IPAddress.Loopback.ToString();

            return remoteIp.MapToIPv4().ToString();
        }

        #region Protected Methods

        /// <summary>
        /// Check whether current HTTP request is available
        /// </summary>
        /// <returns>True if available; otherwise false</returns>
        protected virtual bool IsRequestAvailable() {
            if (_httpContext?.HttpContext == null)
                return false;

            try {
                if (_httpContext.HttpContext.Request == null)
                    return false;
            } catch (Exception) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Is IP address specified
        /// </summary>
        /// <param name="address">IP address</param>
        /// <returns>Result</returns>
        protected virtual bool IsIpAddressSet(IPAddress address) {
            var rez =  address != null && address.ToString() != IPAddress.IPv6Loopback.ToString();
            return rez;
        }

        #endregion

    }

}

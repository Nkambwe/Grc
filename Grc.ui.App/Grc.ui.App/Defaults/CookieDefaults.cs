namespace Grc.ui.App.Defaults {

    public class CookieDefaults {
        /// <summary>
        /// Gets the cookie name prefix
        /// </summary>
        public static string Prefix => ".Grc";

        /// <summary>
        /// Gets a cookie name of the customer
        /// </summary>
        public static string UserCookie => ".User";

        /// <summary>
        /// Gets a cookie name of the antiforgery
        /// </summary>
        public static string AntiforgeryCookie => ".Antiforgery";

        /// <summary>
        /// Gets a cookie name of the session state
        /// </summary>
        public static string SessionCookie => ".Session";
        /// <summary>
        /// Gets a cookie name of the System language
        /// </summary>
        public static string LanguageCookie => ".Language";
        /// <summary>
        /// Gets a cookie name of the authentication
        /// </summary>
        public static string AuthenticationCookie => ".Authentication";

        /// <summary>
        /// Gets a cookie name of the Entrust authentication
        /// </summary>
        public static string EntrustAuthentication => ".EntrustAuthentication";

        /// <summary>
        /// Gets a cookie name of the Cookie Law Warning
        /// </summary>
        public static string IgnoreCookieLawWarning => ".IgnoreCookieLawWarning";
    }
}

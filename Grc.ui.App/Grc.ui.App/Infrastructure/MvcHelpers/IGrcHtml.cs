using Grc.ui.App.Enums;
using Microsoft.AspNetCore.Html;


namespace Grc.ui.App.Infrastructure.MvcHelpers {

    /// <summary>
    /// Represents a HTML helper interface
    /// </summary>
    public interface IGrcHtml {
        /// <summary>
        /// Add title element to the head
        /// </summary>
        /// <param name="part">Title part</param>
        void AddTitleParts(string part);

        /// <summary>
        /// Append title element to the head
        /// </summary>
        /// <param name="part">Title part</param>
        void AppendTitleParts(string part);

        /// <summary>
        /// Generate all title parts
        /// </summary>
        /// <param name="addDefaultTitle">A value indicating whether to insert a default title</param>
        /// <param name="part">Title part</param>
        /// <returns>Generated HTML string</returns>
        Task<IHtmlContent> GenerateTitleAsync(bool addDefaultTitle = true, string part = "");
        /// <summary>
        /// Generate anti-forgery token for CSRF protection
        /// </summary>
        /// <returns>Anti-forgery token as HTML content</returns>
        IHtmlContent AntiForgeryToken();

        /// <summary>
        /// Get anti-forgery token value for AJAX requests
        /// </summary>
        /// <returns>Anti-forgery token value</returns>
        string GetAntiForgeryTokenValue();
        /// <summary>
        /// Add script element
        /// </summary>
        /// <param name="location">A location of the script element</param>
        /// <param name="src">Script path (minified version)</param>
        /// <param name="debugSrc">Script path (full debug version). If empty, then minified version will be used</param>
        /// <param name="excludeFromBundle">A value indicating whether to exclude this script from bundling</param>
        void AddScriptParts(ResourceLocation location, string src, string debugSrc = "", bool excludeFromBundle = false);

        /// <summary>
        /// Add specific CSS File
        /// </summary>
        /// <param name="src"></param>
        /// <param name="debugSrc"></param>
        /// <param name="excludeFromBundle"></param>
        void AppendCssFileParts(string src, string debugSrc = "", bool excludeFromBundle = false);
        
        /// <summary>
        /// Append script element
        /// </summary>
        /// <param name="location">A location of the script element</param>
        /// <param name="src">Script path (minified version)</param>
        /// <param name="debugSrc">Script path (full debug version). If empty, then minified version will be used</param>
        /// <param name="excludeFromBundle">A value indicating whether to exclude this script from bundling</param>
        void AppendScriptParts(ResourceLocation location, string src, string debugSrc = "", bool excludeFromBundle = false);

        /// <summary>
        /// Generate all script parts
        /// </summary>
        /// <param name="location">A location of the script element</param>
        /// <returns>Generated HTML string</returns>
        IHtmlContent GenerateScripts(ResourceLocation location);

        /// <summary>
        /// Add CSS element
        /// </summary>
        /// <param name="src">Script path (minified version)</param>
        /// <param name="debugSrc">Script path (full debug version). If empty, then minified version will be used</param>
        /// <param name="excludeFromBundle">A value indicating whether to exclude this style sheet from bundling</param>
        void AddCssFileParts(string src, string debugSrc = "", bool excludeFromBundle = false);

        /// <summary>
        /// Add CSS file part
        /// </summary>
        /// <param name="src"></param>
        /// <param name="debugSrc"></param>
        /// <param name="excludeFromBundle"></param>
        void AddPageSpecificCssFileParts(string src, string debugSrc = "", bool excludeFromBundle = false);

        /// <summary>
        /// Generate all CSS parts
        /// </summary>
        /// <returns>Generated HTML string</returns>
        IHtmlContent GenerateCssFiles();

        /// <summary>
        /// Add page class
        /// </summary>
        /// <param name="part">Class part</param>
        void AddPageCssClassParts(string part);

        /// <summary>
        /// Append page class
        /// </summary>
        /// <param name="part">Class part</param>
        void AppendPageCssClassParts(string part);

        /// <summary>
        /// Generate page classes
        /// </summary>
        /// <returns>Generated HTML string</returns>
        string GeneratePageCssClasses();

        /// <summary>
        /// Add custom head parts
        /// </summary>
        /// <param name="part">Custom head part</param>
        void AddHeadCustomParts(string part);

        /// <summary>
        /// Append custom head parts
        /// </summary>
        /// <param name="part">Custom head part</param>
        void AppendHeadCustomParts(string part);

        /// <summary>
        /// Generate all custom head parts
        /// </summary>
        /// <returns>Generated HTML string</returns>
        IHtmlContent GenerateHeadCustom();

        /// <summary>
        /// Returns markup that is not HTML encoded
        /// </summary>
        /// <param name="value">HTML markup</param>
        /// <returns>HTML content</returns>
        IHtmlContent Raw(string value);

        /// <summary>
        /// HTML encode the specified value
        /// </summary>
        /// <param name="value">Value to encode</param>
        /// <returns>Encoded value</returns>
        string Encode(string value);

        /// <summary>
        /// HTML encode the specified object
        /// </summary>
        /// <param name="value">Object to encode</param>
        /// <returns>Encoded value</returns>
        string Encode(object value);

        /// <summary>
        /// Renders a partial view asynchronously
        /// </summary>
        /// <param name="partialViewName">The name of the partial view</param>
        /// <param name="model">The model to pass to the partial view</param>
        /// <returns>HTML content</returns>
        Task<IHtmlContent> PartialAsync(string partialViewName, object model = null);
    }
}

using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Grc.ui.App.Enums;


namespace Grc.ui.App.MvcHelpers {

    /// <summary>
    /// HTML helper class is used to manage various page elements like titles, cripts and css files
    /// </summary>
    /// <remarks>
    /// This class offers a clean, organized way to manage all page resources while keeping views tidy.
    /// The advantage of this class is organized resource management, conditional exclusion of resources, 
    /// automatic rejection of duplicates and separation of concerns
    /// </remarks>
    public class GrcHtml : IGrcHtml {
       
        #region Fields
        
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IHostEnvironment _webHostEnvironment;
        private readonly HtmlEncoder _htmlEncoder;
        private readonly string _siteTitle;
        
        protected readonly List<string> _titleParts = new();
        protected readonly List<string> _headCustomParts = new();
        protected readonly List<string> _pageCssClassParts = new();
        protected readonly Dictionary<ResourceLocation, List<ScriptReferenceMeta>> _scriptParts = new();
        protected readonly List<CssReferenceMeta> _cssParts = new();
        protected readonly Dictionary<ResourceLocation, List<string>> _inlineScriptParts = new();

        protected readonly List<CssReferenceMeta> _regularCssParts = new();
        protected readonly List<CssReferenceMeta> _pageSpecificCssParts = new();

        #endregion

        #region Constructor

        public GrcHtml(
            IActionContextAccessor actionContextAccessor,
            IUrlHelperFactory urlHelperFactory,
            IHostEnvironment webHostEnvironment,
            HtmlEncoder htmlEncoder)
            {
                _actionContextAccessor = actionContextAccessor;
                _urlHelperFactory = urlHelperFactory;
                _webHostEnvironment = webHostEnvironment;
                _htmlEncoder = htmlEncoder;
                _siteTitle = "GRC ";
            }

        #endregion

        #region Title

            /// <summary>
            /// Add title element to the head
            /// </summary>
            /// <param name="part">Title part</param>
            public virtual void AddTitleParts(string part)
            {
                if (string.IsNullOrEmpty(part))
                    return;
            
                _titleParts.Add(part);
            }

            /// <summary>
            /// Append title element to the head
            /// </summary>
            /// <param name="part">Title part</param>
            public virtual void AppendTitleParts(string part)
            {
                if (string.IsNullOrEmpty(part))
                    return;
            
                _titleParts.Insert(0, part);
            }

            /// <summary>
            /// Generate all title parts
            /// </summary>
            /// <param name="addDefaultTitle">A value indicating whether to insert a default title</param>
            /// <param name="part">Title part</param>
            /// <returns>Generated HTML string</returns>
            public virtual Task<IHtmlContent> GenerateTitleAsync(bool addDefaultTitle = true, string part = "")
            {
                if (!string.IsNullOrEmpty(part))
                {
                    AppendTitleParts(part);
                }
            
                var specificTitle = string.Join(" - ", _titleParts.AsEnumerable().Reverse().ToArray());
                string result;
            
                if (!string.IsNullOrEmpty(specificTitle))
                {
                    if (addDefaultTitle)
                    {
                        // Store name + page title
                        var defaultTitle = _siteTitle;
                        result = $"{defaultTitle} - {specificTitle}";
                    }
                    else
                    {
                        // Page title only
                        result = specificTitle;
                    }
                }
                else
                {
                    // Store name only
                    result = _siteTitle;
                }
            
                return Task.FromResult<IHtmlContent>(new HtmlString(_htmlEncoder.Encode(result ?? string.Empty)));
            }

        #endregion

        #region Scripts

            /// <summary>
            /// Add script element
            /// </summary>
            /// <param name="location">A location of the script element</param>
            /// <param name="src">Script path (minified version)</param>
            /// <param name="debugSrc">Script path (full debug version). If empty, then minified version will be used</param>
            /// <param name="excludeFromBundle">A value indicating whether to exclude this script from bundling</param>
            public virtual void AddScriptParts(ResourceLocation location, string src, string debugSrc = "", bool excludeFromBundle = false)
            {
                if (!_scriptParts.ContainsKey(location))
                    _scriptParts.Add(location, new List<ScriptReferenceMeta>());
            
                if (string.IsNullOrEmpty(src))
                    return;
            
                if (!string.IsNullOrEmpty(debugSrc) && _webHostEnvironment.IsDevelopment())
                    src = debugSrc;
            
                if (_actionContextAccessor.ActionContext == null)
                    throw new ArgumentNullException(nameof(_actionContextAccessor.ActionContext));
            
                var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
                var pathBase = _actionContextAccessor.ActionContext.HttpContext.Request.PathBase;
                var isLocal = src.StartsWith("~/") || src.StartsWith("/") || src.StartsWith("../") || src.StartsWith("./");
            
                _scriptParts[location].Add(new ScriptReferenceMeta
                {
                    ExcludeFromBundle = excludeFromBundle,
                    IsLocal = isLocal,
                    Src = isLocal ? urlHelper.Content(src) : src
                });
            }

            /// <summary>
            /// Append script element
            /// </summary>
            /// <param name="location">A location of the script element</param>
            /// <param name="src">Script path (minified version)</param>
            /// <param name="debugSrc">Script path (full debug version). If empty, then minified version will be used</param>
            /// <param name="excludeFromBundle">A value indicating whether to exclude this script from bundling</param>
            public virtual void AppendScriptParts(ResourceLocation location, string src, string debugSrc = "", bool excludeFromBundle = false)
            {
                if (!_scriptParts.ContainsKey(location))
                    _scriptParts.Add(location, new List<ScriptReferenceMeta>());
            
                if (string.IsNullOrEmpty(src))
                    return;
            
                if (!string.IsNullOrEmpty(debugSrc) && _webHostEnvironment.IsDevelopment())
                    src = debugSrc;
            
                if (_actionContextAccessor.ActionContext == null)
                    throw new ArgumentNullException(nameof(_actionContextAccessor.ActionContext));
            
                var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
                var pathBase = _actionContextAccessor.ActionContext.HttpContext.Request.PathBase;
                var isLocal = src.StartsWith("~/") || src.StartsWith("/") || src.StartsWith("../") || src.StartsWith("./");
            
                _scriptParts[location].Insert(0, new ScriptReferenceMeta
                {
                    ExcludeFromBundle = excludeFromBundle,
                    IsLocal = isLocal,
                    Src = isLocal ? urlHelper.Content(src) : src
                });
            }

            /// <summary>
            /// Add inline script element
            /// </summary>
            /// <param name="location">A location of the script element</param>
            /// <param name="script">Script</param>
            public virtual void AddInlineScriptParts(ResourceLocation location, string script)
            {
                if (!_inlineScriptParts.ContainsKey(location))
                    _inlineScriptParts.Add(location, new List<string>());

                if (string.IsNullOrEmpty(script))
                    return;

                _inlineScriptParts[location].Add(script);
            }

            /// <summary>
            /// Generate all script parts
            /// </summary>
            /// <param name="location">A location of the script element</param>
            /// <returns>Generated HTML string</returns>
            public virtual IHtmlContent GenerateScripts(ResourceLocation location)
            {
                if (!_scriptParts.TryGetValue(location, out var value) || value == null || !value.Any())
                    return HtmlString.Empty;
            
                var result = new StringBuilder();
                var scripts = value.Distinct();
            
                foreach (var item in scripts)
                {
                    if (!item.IsLocal)
                    {
                        result.AppendFormat("<script src=\"{0}\"></script>", item.Src);
                        result.Append(Environment.NewLine);
                        continue;
                    }

                    result.AppendFormat("<script src=\"{0}\"></script>", item.Src);
                    result.Append(Environment.NewLine);
                }
            
                return new HtmlString(result.ToString());
            }

            /// <summary>
            /// Generate all inline script parts
            /// </summary>
            /// <param name="location">A location of the script element</param>
            /// <returns>Generated HTML string</returns>
            public virtual IHtmlContent GenerateInlineScripts(ResourceLocation location)
        {
            if (!_inlineScriptParts.TryGetValue(location, out var value) || value == null || !value.Any())
                return HtmlString.Empty;
            
            var result = new StringBuilder();
            
            result.Append("<script>");
            result.Append(Environment.NewLine);
            
            foreach (var item in value)
            {
                result.Append(item);
                result.Append(Environment.NewLine);
            }
            
            result.Append("</script>");
            result.Append(Environment.NewLine);
            
            return new HtmlString(result.ToString());
        }

           
            #endregion

        #region CSS

            /// <summary>
            /// Add CSS element
            /// </summary>
            /// <param name="src">Script path (minified version)</param>
            /// <param name="debugSrc">Script path (full debug version). If empty, then minified version will be used</param>
            /// <param name="excludeFromBundle">A value indicating whether to exclude this style sheet from bundling</param>
            public virtual void AddCssFileParts(string src, string debugSrc = "", bool excludeFromBundle = false) {
                if (string.IsNullOrEmpty(src))
                    return;

                // Process the CSS file as before
                if (!string.IsNullOrEmpty(debugSrc) && _webHostEnvironment.IsDevelopment())
                    src = debugSrc;

                if (_actionContextAccessor.ActionContext == null)
                    throw new ArgumentNullException(nameof(_actionContextAccessor.ActionContext));

                var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
                var isLocal = src.StartsWith("~/") || src.StartsWith("/") || src.StartsWith("../") || src.StartsWith("./");

                // Add to regular CSS parts
                _regularCssParts.Add(new CssReferenceMeta {
                    ExcludeFromBundle = excludeFromBundle,
                    IsLocal = isLocal,
                    Src = isLocal ? urlHelper.Content(src) : src
                });
            }

            public virtual void AddPageSpecificCssFileParts(string src, string debugSrc = "", bool excludeFromBundle = false) {
                if (string.IsNullOrEmpty(src))
                    return;

                // Process the CSS file
                if (!string.IsNullOrEmpty(debugSrc) && _webHostEnvironment.IsDevelopment())
                    src = debugSrc;

                if (_actionContextAccessor.ActionContext == null)
                    throw new ArgumentNullException(nameof(_actionContextAccessor.ActionContext));

                var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
                var isLocal = src.StartsWith("~/") || src.StartsWith("/") || src.StartsWith("../") || src.StartsWith("./");

                // Add to page-specific CSS parts
                _pageSpecificCssParts.Add(new CssReferenceMeta {
                    ExcludeFromBundle = excludeFromBundle,
                    IsLocal = isLocal,
                    Src = isLocal ? urlHelper.Content(src) : src
                });
            }

            /// <summary>
            /// Append CSS element
            /// </summary>
            /// <param name="src">Script path (minified version)</param>
            /// <param name="debugSrc">Script path (full debug version). If empty, then minified version will be used</param>
            /// <param name="excludeFromBundle">A value indicating whether to exclude this style sheet from bundling</param>
            public virtual void AppendCssFileParts(string src, string debugSrc = "", bool excludeFromBundle = false) {
                if (string.IsNullOrEmpty(src))
                    return;

                if (!string.IsNullOrEmpty(debugSrc) && _webHostEnvironment.IsDevelopment())
                    src = debugSrc;

                if (_actionContextAccessor.ActionContext == null)
                    throw new ArgumentNullException(nameof(_actionContextAccessor.ActionContext));

                var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
                var isLocal = src.StartsWith("~/") || src.StartsWith("/") || src.StartsWith("../") || src.StartsWith("./");

                _cssParts.Insert(0, new CssReferenceMeta {
                    ExcludeFromBundle = excludeFromBundle,
                    IsLocal = isLocal,
                    Src = isLocal ? urlHelper.Content(src) : src
                });
            }

            /// <summary>
            /// Generate all CSS parts
            /// </summary>
            /// <returns>Generated HTML string</returns>
           public virtual IHtmlContent GenerateCssFiles() {
                var result = new StringBuilder();
    
                // First generate regular CSS parts
                var distinctRegularParts = _regularCssParts.Distinct();
                foreach (var item in distinctRegularParts)
                {
                    result.AppendFormat("<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\" />", item.Src);
                    result.Append(Environment.NewLine);
                }
    
                // Then generate page-specific CSS parts
                var distinctPageParts = _pageSpecificCssParts.Distinct();
                foreach (var item in distinctPageParts)
                {
                    result.AppendFormat("<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\" />", item.Src);
                    result.Append(Environment.NewLine);
                }
    
                return new HtmlString(result.ToString());
           }

            /// <summary>
            /// Add page class
            /// </summary>
            /// <param name="part">Class part</param>
            public virtual void AddPageCssClassParts(string part) {
                if (string.IsNullOrEmpty(part))
                    return;

                _pageCssClassParts.Add(part);
            }

            /// <summary>
            /// Append page class
            /// </summary>
            /// <param name="part">Class part</param>
            public virtual void AppendPageCssClassParts(string part)
            {
                if (string.IsNullOrEmpty(part))
                    return;

                _pageCssClassParts.Insert(0, part);
            }

            /// <summary>
            /// Generate page classes
            /// </summary>
            /// <returns>Generated HTML string</returns>
            public virtual string GeneratePageCssClasses()
            {
                if (!_pageCssClassParts.Any())
                    return string.Empty;

                return string.Join(" ", _pageCssClassParts);
            }

            /// <summary>
            /// Add custom head parts
            /// </summary>
            /// <param name="part">Custom head part</param>
            public virtual void AddHeadCustomParts(string part)
            {
                if (string.IsNullOrEmpty(part))
                    return;

                _headCustomParts.Add(part);
            }

            /// <summary>
            /// Append custom head parts
            /// </summary>
            /// <param name="part">Custom head part</param>
            public virtual void AppendHeadCustomParts(string part)
            {
                if (string.IsNullOrEmpty(part))
                    return;

                _headCustomParts.Insert(0, part);
            }

            /// <summary>
            /// Generate all custom head parts
            /// </summary>
            /// <returns>Generated HTML string</returns>
            public virtual IHtmlContent GenerateHeadCustom()
            {
                if (!_headCustomParts.Any())
                    return HtmlString.Empty;

                var result = new StringBuilder();
                foreach (var item in _headCustomParts)
                {
                    result.Append(item);
                    result.Append(Environment.NewLine);
                }

                return new HtmlString(result.ToString());
            }

        #endregion

    }
}


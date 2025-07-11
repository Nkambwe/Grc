using Grc.ui.App.Defaults;
using Grc.ui.App.Dtos;
using Grc.ui.App.Helpers;
using Grc.ui.App.Utils;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Xml;

namespace Grc.ui.App.Services {

    public class LocalizationService : ILocalizationService {
        private readonly IApplicationLogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IList<SystemLanguage> _availableLanguages;
        private readonly IGRCFileProvider _fileProvider;

        public LocalizationService(IApplicationLoggerFactory loggerFactory,
                                   IHttpContextAccessor httpContextAccessor, 
                                   IGRCFileProvider fileProvider) {
            _httpContextAccessor = httpContextAccessor;
            _fileProvider = fileProvider;
            _logger = loggerFactory.CreateLogger("app_services");
            _logger.Channel = $"LOCALIZER-{DateTime.Now:yyyyMMddHHmmss}";
        }

        public IList<SystemLanguage> GetAvailableLanguages() {

            try {
                if (_availableLanguages != null)
                    return _availableLanguages;

                _availableLanguages = new List<SystemLanguage>();
                foreach (var filePath in _fileProvider.EnumerateFiles(_fileProvider.MapPath("~/Localization/"), "*.xml")) {
                    var xmlDocument = new XmlDocument();
                    xmlDocument.Load(filePath);

                    //get language code
                    var languageCode = "";
                    //we get the file name format: language.{languagecode}.xml
                    var r = new Regex(Regex.Escape("language.") + "(.*?)" + Regex.Escape(".xml"));
                    var matches = r.Matches(_fileProvider.GetFileName(filePath));
                    foreach (Match match in matches) {
                        languageCode = match.Groups[1].Value;
                    }

                    //and now we use language codes only (not full culture names)
                    languageCode = languageCode[..2];
                    var languageNode = xmlDocument.SelectSingleNode(@"//Language");

                    if (languageNode == null || languageNode.Attributes == null)
                        continue;

                    //get language friendly name
                    var languageName = languageNode.Attributes["Name"].InnerText.Trim();
                    //is default
                    var isDefaultAttribute = languageNode.Attributes["IsDefault"];
                    var isDefault = isDefaultAttribute != null && Convert.ToBoolean(isDefaultAttribute.InnerText.Trim());

                    //is default
                    var isRightToLeftAttribute = languageNode.Attributes["IsRightToLeft"];
                    var isRightToLeft = isRightToLeftAttribute != null && Convert.ToBoolean(isRightToLeftAttribute.InnerText.Trim());

                    //create language
                    var language = new SystemLanguage {
                        Code = languageCode,
                        Name = languageName,
                        IsDefault = isDefault,
                        IsRightToLeft = isRightToLeft,
                    };

                    //load resources
                    var resources = xmlDocument.SelectNodes(@"//Language/LocaleResource");
                    if (resources == null)
                        continue;
                    foreach (XmlNode resNode in resources) {
                        if (resNode.Attributes == null)
                            continue;

                        var resNameAttribute = resNode.Attributes["Name"];
                        var resValueNode = resNode.SelectSingleNode("Value");

                        if (resNameAttribute == null)
                            throw new GRCException("All Language resources must have an attribute Name=\"Value\".");
                        var resourceName = resNameAttribute.Value.Trim();
                        if (string.IsNullOrEmpty(resourceName))
                            throw new GRCException("All Language resource attributes 'Name' must have a value.'");

                        if (resValueNode == null)
                            throw new GRCException("All Language resources must have an element \"Value\".");
                        var resourceValue = resValueNode.InnerText.Trim();

                        language.Resources.Add(new LanguageResource {
                            Name = resourceName,
                            Value = resourceValue
                        });
                    }

                    _availableLanguages.Add(language);
                    _availableLanguages = _availableLanguages.OrderBy(l => l.Name).ToList();

                }

                return _availableLanguages;
            }catch (Exception ex) { 
                _logger.LogActivity($"{ex.Message}", "ERROR");
                _logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");
            }
            
            return _availableLanguages;
        }

        public SystemLanguage GetCurrentLanguage() {
            var httpContext = _httpContextAccessor.HttpContext;

            //..try to get cookie
            var cookieName = $"{CookieDefaults.Prefix}{CookieDefaults.LanguageCookie}";
            httpContext.Request.Cookies.TryGetValue(cookieName, out var cookieLanguageCode);

            //..ensure it's available because it could be deleted
            var availableLanguages = GetAvailableLanguages();
            var language = availableLanguages.FirstOrDefault(l => l.Code.Equals(cookieLanguageCode, StringComparison.InvariantCultureIgnoreCase));
            if (language != null)
                return language;

            //..let's return the default one
            language = availableLanguages.FirstOrDefault(l => l.IsDefault);
             _logger.LogActivity($"DEFAULT LANG :: {JsonSerializer.Serialize(language)}", "INFO");
            if (language != null)
                return language;

            //..return any available language
            language = availableLanguages.FirstOrDefault();
            _logger.LogActivity($"ANY LANG :: {JsonSerializer.Serialize(language)}", "INFO");
            return language;
        }

        public string GetLocalizedLabel(string labelName) {
            var language = GetCurrentLanguage();
            if (language == null)
                return labelName;

            var resourceValue = language.Resources
                .Where(r => r.Name.Equals(labelName, StringComparison.InvariantCultureIgnoreCase))
                .Select(r => r.Value).FirstOrDefault();
            _logger.LogActivity($"LABEL CODE  :: {labelName}", "INFO");
            _logger.LogActivity($"LABEL VALUE :: {resourceValue}", "INFO");
            if (string.IsNullOrEmpty(resourceValue))
                return labelName;

            return resourceValue;
        }

    }

}

using Grc.ui.App.Defaults;

namespace Grc.ui.App.Utils {
    public class LanguageOptions {
         public const string SectionName = "LanguageOptions";
         public string DefaultCulture  { get; set; } = CommonDefaults.DefaultLanguageCulture;
         public string[] SupportedCultures { get; set; } = Array.Empty<string>();
    }

}

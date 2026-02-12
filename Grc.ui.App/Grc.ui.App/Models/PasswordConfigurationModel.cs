using System.Text.Json.Serialization;

namespace Grc.ui.App.Models {

    public class PasswordConfigurationModel {
        public bool ExpirePassword { get; set; }
        public int ExipryDays { get; set; } 
        public int MinimumLength { get; set; }
        public bool AllowMaualReset { get; set; }
        public bool AllowPwsReuse { get; set; }
        public bool IncludeUpper { get; set; }
        public bool IncludeLower { get; set; }
        public bool IncludeSpecial { get; set; }
        public bool IncludeNumerics { get; set; }
        
    }
}

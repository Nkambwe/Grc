
using Grc.ui.App.Defaults;
using System.Text.Json.Serialization;

namespace Grc.ui.App.Models {
    public class PasswordChangeModel {
        public long UserId { get; set; }
        public string Username { get; set; }
        public string Initials { get; set; }
        public string WelcomeMessage { get; set; }
        public string Banner { get; set; } = CommonDefaults.AppVersion;
        public WorkspaceModel Workspace { get; set; }
        public int MinimumLength { get; set; } = 12;
        public bool IncludeUpperChar{ get; set; } = true;
        public bool IncludeLowerChar { get; set; } = true;
        public bool IncludeSpecialChar { get; set; } = true;
        public bool CanReusePasswords { get; set; } = true;
        public bool IncludeNumericChar { get; set; } = true;

    }

}

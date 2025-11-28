using Grc.ui.App.Models;
using System.Text.Json.Serialization;

namespace Grc.ui.App.Dtos {
    public class UnitExtensionCountResponse {

        [JsonPropertyName("banner")]
        public string Banner { get; set; }

        [JsonIgnore]
        public string WelcomeMessage { get; set; } = string.Empty;

        [JsonIgnore]
        public string Initials { get; set; }

        [JsonIgnore]
        public WorkspaceModel Workspace { get; set; }

        [JsonPropertyName("unitProcesses")]
        public Dictionary<string, int> UnitProcesses { get; set; } = new();

        [JsonIgnore]
        public List<QuickActionModel> QuickActions { get; set; } = new();

    }

}

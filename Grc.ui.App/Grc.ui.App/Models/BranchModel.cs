using System.Text.Json.Serialization;

namespace Grc.ui.App.Models {

    public class BranchModel {

        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("companyId")]
        public long CompanyId { get; set; }

        [JsonPropertyName("solId")]
        public string SolId { get; set; }

        [JsonPropertyName("companyName")]
        public string CompanyName { get; set; }

        [JsonPropertyName("companyAlias")]
        public string CompanyAlias { get; set; }

        [JsonPropertyName("branchName")]
        public string BranchName { get; set; }
    }

}

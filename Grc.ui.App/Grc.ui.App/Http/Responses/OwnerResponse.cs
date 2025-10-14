using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses
{
    public class OwnerResponse {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("ownerName")]
        public string Name { get; set; }

        [JsonPropertyName("ownerPhone")]
        public string Phone { get; set; }

        [JsonPropertyName("ownerEmail")]
        public string Email { get; set; }

        [JsonPropertyName("ownerPosition")]
        public string Position { get; set; }

        [JsonPropertyName("ownerComment")]
        public string Comment { get; set; }

        [JsonPropertyName("departmentId")]
        public long DepartmentId { get; set; }

        [JsonPropertyName("department")]
        public string Department { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

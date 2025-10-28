namespace Grc.ui.App.Http.Responses {
    public class GrcPermissionSetResponse {
        public long Id { get; set; }
        public string SetName { get; set; }
        public string SetDescription { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public List<GrcPermissionResponse> Permissions { get; set; } = new();
    }
}

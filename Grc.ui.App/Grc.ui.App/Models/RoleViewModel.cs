namespace Grc.ui.App.Models {
    public class RoleViewModel {
        public long Id { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public long GroupId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsVerified { get; set; }
        public bool IsApproved { get; set; }
        public List<long> Permissions { get; set; }
        public List<long> Users { get; set; }
    }
}

namespace Grc.ui.App.Models {
    public class RoleGroupViewModel
    {
        public long Id { get; set; }
        public string GroupName { get; set; }
        public string GroupDescription { get; set; }
        public long DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string GroupScope { get; set; }
        public string AttachedTo { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsVerified { get; set; }
        public bool IsApproved { get; set; }
    }
}

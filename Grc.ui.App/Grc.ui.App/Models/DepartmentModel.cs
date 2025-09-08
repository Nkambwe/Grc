namespace Grc.ui.App.Models {

    public class DepartmentModel {
        public long Id { get; set; }
        public long BranchId { get; set; }
        public string Branch { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentAlias { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatdOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public List<DepartmentUnitModel> Units { get; set; } = new();
    }

}
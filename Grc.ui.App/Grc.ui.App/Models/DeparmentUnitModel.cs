namespace Grc.ui.App.Models {
    public class DeparmentUnitModel {
        public long Id { get; set; }
        public long DepartmentId { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatdOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
    }
}
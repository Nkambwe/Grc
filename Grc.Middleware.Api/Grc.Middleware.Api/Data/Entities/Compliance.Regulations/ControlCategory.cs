namespace Grc.Middleware.Api.Data.Entities.Compliance.Regulations {
    public class ControlCategory : BaseEntity {
        public string CategoryName { get; set; }
        public bool Exclude { get; set; }
        public string Owner { get; set; }
        public string Notes { get; set; }
        public virtual ICollection<ControlItem> ControlItems { get; set; }
        
    }

}

namespace Grc.Middleware.Api.Data.Entities.Compliance.Regulations {
    public class ControlItem: BaseEntity {
        public string ItemName { get; set; }
        public bool Exclude { get; set; }
        public string Notes { get; set; }
        public long ControlCategoryId { get; set; }
        public virtual ControlCategory ControlCategory { get; set; }
        public virtual ICollection<StatutoryArticleControl> StatutoryArticleControls { get; set; }
    }

}

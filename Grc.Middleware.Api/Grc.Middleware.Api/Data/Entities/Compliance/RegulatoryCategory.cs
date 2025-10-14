namespace Grc.Middleware.Api.Data.Entities.Compliance {
    public class RegulatoryCategory : BaseEntity {
        public string CategoryName { get; set; }
        public virtual ICollection<StatutoryRegulation> Regulations { get; set; }
    }

}

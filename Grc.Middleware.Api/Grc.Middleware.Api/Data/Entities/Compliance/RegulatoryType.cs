namespace Grc.Middleware.Api.Data.Entities.Compliance {
    public class RegulatoryType : BaseEntity
    {
        public string TypeName { get; set; } // Law, Statute, Regulation, Standard
        public virtual ICollection<StatutoryRegulation> Regulations { get; set; }
    }

}

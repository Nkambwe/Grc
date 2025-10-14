namespace Grc.Middleware.Api.Data.Entities.Compliance {
    public class Authority : BaseEntity {
        public string AuthorityName { get; set; }
        public string AuthorityAlias { get; set; }
        public virtual ICollection<StatutoryRegulation> Regulations { get; set; }
    }



}

namespace Grc.Middleware.Api.Data.Entities.Compliance {
    public class Frequency : BaseEntity {
        public string FrequencyName { get; set; }
        public virtual ICollection<RegulatoryReturn> Returns { get; set; }
    }

}

namespace Grc.Middleware.Api.Data.Entities.Org {

    public class Branch : BaseEntity {
        public long CompanyId { get; set; }
        public string BranchName { get; set; }
        public string SolId { get; set; }
       

        public virtual Company Company { get; set; }
        public virtual ICollection<Department> Departments { get; set; }

        public override bool Equals(object obj) {

            if (obj is not Branch)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            var item = (Branch)obj;

            if (item.IsNew() || IsNew())
                return false;

            return item.SolId.Equals(SolId) && item.BranchName.Equals(BranchName);
        }
        public override string ToString() => $"{SolId}-{BranchName}";
        public override int GetHashCode() => ToString().GetHashCode() ^ 31;
    }
}

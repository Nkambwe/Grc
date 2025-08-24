using Grc.Middleware.Api.Data.Entities.Org;

namespace Grc.Middleware.Api.Data.Entities.System {

    public class SystemError: BaseEntity  {
        public string ErrorMessage { get; set; }
        public string ErrorSource { get; set; }
        public string StackTrace { get; set; }
        public string Severity { get; set; }
        public bool IsUserReported { get; set; }
        public bool Assigned {get; set;}
        public string AssignedTo { get; set;}
        public string FixStatus { get; set;}
        public DateTime ReportedOn { get; set;}
        public DateTime? ClosedOn { get; set;}
        public long CompanyId { get; set;}
        public virtual Company Company { get; set;}
        public override bool Equals(object obj) {

            if (obj is not SystemError)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            var item = (SystemError)obj;

            if (item.IsNew() || IsNew())
                return false;

            return item.Id.Equals(Id) && item.ErrorSource.Equals(ErrorSource);
        }

        public override int GetHashCode() => ToString().GetHashCode() ^ 31;
    }
}

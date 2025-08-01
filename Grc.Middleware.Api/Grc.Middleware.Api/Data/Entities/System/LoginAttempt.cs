namespace Grc.Middleware.Api.Data.Entities.System {

    public class LoginAttempt : BaseEntity {
        public long UserId { get; set; }
        public string IpAddress { get; set; }
        public DateTime AttemptTime { get; set; }
        public bool IsSuccessful { get; set; }
        public virtual SystemUser User { get; set; }
        public override bool Equals(object obj) {

            if (obj is not LoginAttempt)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            var item = (LoginAttempt)obj;

            if (item.IsNew() || IsNew())
                return false;

            return item.UserId.Equals(UserId);
        }

        public override string ToString()
            => $"{UserId} ({IpAddress})";

        public override int GetHashCode() => ToString().GetHashCode() ^ 31;
    }

}

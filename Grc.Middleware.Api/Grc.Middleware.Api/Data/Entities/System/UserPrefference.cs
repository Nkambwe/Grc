
namespace Grc.Middleware.Api.Data.Entities.System {
    public class UserPrefference: BaseEntity {
        public long UserId { get; set; }
        public string Theme { get; set; }
        public string Language { get; set; }

        public virtual SystemUser User { get; set; }
        public override bool Equals(object obj) {

            if (obj is not UserPrefference)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            var item = (UserPrefference)obj;

            if (item.IsNew() || IsNew())
                return false;

            return item.UserId.Equals(UserId) && item.Id.Equals(Id);
        }

        public override string ToString()
            => $"{UserId} ({Id})";

        public override int GetHashCode() => ToString().GetHashCode() ^ 31;
    }
}

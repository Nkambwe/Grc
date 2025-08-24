namespace Grc.Middleware.Api.Data.Entities.System {
    public class UserView: BaseEntity {
        public long UserId { get; set; }
        public string Name { get; set; }
        public string View { get; set; }

        public virtual SystemUser User { get; set; }
        public override bool Equals(object obj) {

            if (obj is not UserView)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            var item = (UserView)obj;

            if (item.IsNew() || IsNew())
                return false;

            return item.Name.Equals(Name) && item.Id.Equals(Id);
        }

        public override string ToString()
            => $"{UserId} ({Id})";

        public override int GetHashCode() => ToString().GetHashCode() ^ 31;
    }
}

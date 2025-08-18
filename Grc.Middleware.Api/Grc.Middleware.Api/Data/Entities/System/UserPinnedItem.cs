
namespace Grc.Middleware.Api.Data.Entities.System {
    public class UserPinnedItem : BaseEntity { 
        public long UserId { get; set; }
        public string Label{ get; set;}
        public string IconClass{ get; set;}
        public string Controller{ get; set;}
        public string Action{ get; set;}
        public string Area{ get; set;}
        public string CssClass{ get; set;}   
        
        public virtual SystemUser User { get; set; }
        public override bool Equals(object obj) {

            if (obj is not UserPinnedItem)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            var item = (UserPinnedItem)obj;

            if (item.IsNew() || IsNew())
                return false;

            return item.UserId.Equals(UserId) &&
                item.Label.Equals(Label) &&
                item.Action.Equals(Action);
        }

        public override string ToString()
            => $"{UserId}-{Label}-{Action}";

        public override int GetHashCode() => ToString().GetHashCode() ^ 31;
    }
}

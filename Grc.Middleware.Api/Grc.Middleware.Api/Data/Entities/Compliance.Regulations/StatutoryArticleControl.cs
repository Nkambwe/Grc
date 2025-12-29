namespace Grc.Middleware.Api.Data.Entities.Compliance.Regulations {
    public class StatutoryArticleControl {
        public long StatutoryArticleId { get; set; }
        public virtual StatutoryArticle StatutoryArticle { get; set; }
        public long ControlItemId { get; set; }
        public virtual ControlItem ControlItem { get; set; }
    }

}

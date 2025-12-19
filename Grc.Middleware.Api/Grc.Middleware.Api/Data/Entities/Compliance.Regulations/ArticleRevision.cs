using Grc.Middleware.Api.Data.Entities;
/// <summary>
/// Class representing an article revision
/// </summary>
public class ArticleRevision : BaseEntity {
    public string Section { get; set; }
    public string Summery { get; set; }
    public string Comments { get; set; }
    public string Revision { get; set; }
    public DateTime ReviewedOn { get; set; }
    public long ArticleId { get; set; }
    public virtual StatutoryArticle Article { get; set; }

}

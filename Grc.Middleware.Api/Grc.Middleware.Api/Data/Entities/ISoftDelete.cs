namespace Grc.Middleware.Api.Data.Entities {

    /// <summary>
    /// Soft delete an entity by simply marking it as deleted
    /// </summary>
    public interface ISoftDelete {
        /// <summary>
        /// check if property should be marked as deleted or completed deleted
        /// </summary>
        bool IsDeleted {get;set;}
    }

}

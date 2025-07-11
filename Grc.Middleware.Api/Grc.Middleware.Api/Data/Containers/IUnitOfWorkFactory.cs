namespace Grc.Middleware.Api.Data.Containers {

    /// <summary>
    /// Unit of work Factory
    /// </summary>
    public interface IUnitOfWorkFactory {
        /// <summary>
        /// Creates a new instance of a unit of work
        /// </summary>
        /// <returns>Returns created instance of IUnitOfWork</returns>
        IUnitOfWork Create();
    }

}

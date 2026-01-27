namespace Grc.Middleware.Api.TaskHandler {

    /// <summary>
    /// Interface handles bacnkground mail task
    /// </summary>
    public interface IMailTaskQueue {
        void Enqueue(Func<IServiceProvider, CancellationToken, Task> workItem);
        Task<Func<IServiceProvider, CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }


}

using System.Threading.Channels;

namespace Grc.Middleware.Api.TaskHandler {
    /// <summary>
    /// Background Mail task handler
    /// </summary>
    public class MailTaskQueue : IMailTaskQueue {
        private readonly Channel<Func<IServiceProvider, CancellationToken, Task>> _queue = Channel.CreateUnbounded<Func<IServiceProvider, CancellationToken, Task>>();

        public void Enqueue(Func<IServiceProvider, CancellationToken, Task> workItem) {
            ArgumentNullException.ThrowIfNull(workItem);
            _queue.Writer.TryWrite(workItem);
        }

        public async Task<Func<IServiceProvider, CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken){
            return await _queue.Reader.ReadAsync(cancellationToken);
        }
    }


}

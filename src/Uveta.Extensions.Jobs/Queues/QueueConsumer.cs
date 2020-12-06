using System;
using System.Threading;
using System.Threading.Tasks;
using Uveta.Extensions.Jobs.Abstractions.Models;
using Uveta.Extensions.Jobs.Abstractions.Queues;

namespace Uveta.Extensions.Jobs.Queues
{
    internal class QueueConsumer : IQueueConsumer
    {
        private readonly Action _onDispose;
        private readonly bool _disposed = false;

        public event Func<Job, Delivery, Task>? JobQueued;

    #pragma warning disable CS0067
        public event Func<ErrorArgs, Delivery, Task>? Error;
    #pragma warning restore CS0067

        public QueueConsumer(Action onDispose)
        {
            _onDispose = onDispose;
        }

        public Task StartAsync(int? maximumConcurrentCalls, CancellationToken cancel)
        {
            return Task.CompletedTask;
        }

        public async Task OnNewItemAsync(Job item)
        {
            var delivery = new Delivery(false);
            if (JobQueued is null) return;
            Func<Job, Delivery, Task> handler = JobQueued;
            Delegate[] invocationList = handler.GetInvocationList();
            Task[] handlerTasks = new Task[invocationList.Length];
            for (int i = 0; i < invocationList.Length; i++)
            {
                handlerTasks[i] = ((Func<Job, Delivery, Task>)invocationList[i])(item, delivery);
            }
            await Task.WhenAll(handlerTasks);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _onDispose?.Invoke();
        }
    }
}
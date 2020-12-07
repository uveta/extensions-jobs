using System;
using System.Collections.Generic;
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

        public async Task OnNewJobAsync(Job job, bool waitForHandlers)
        {
            var delivery = new Delivery(false);
            if (JobQueued is null) return;
            if (waitForHandlers)
            {
                Func<Job, Delivery, Task> handler = JobQueued;
                List<Task> handlerTasks = new();
                foreach (var invocation in handler.GetInvocationList())
                {
                    var task = ((Func<Job, Delivery, Task>)invocation)(job, delivery);
                    handlerTasks.Add(task);
                }
                await Task.WhenAll(handlerTasks);
            }
            else
            {
                await JobQueued(job, delivery);
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _onDispose?.Invoke();
        }
    }
}
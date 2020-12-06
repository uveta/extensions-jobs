using System;
using Microsoft.Extensions.DependencyInjection;
using Uveta.Extensions.Jobs.Abstractions.Workers;

namespace Uveta.Extensions.Jobs.Workers.DependencyInjection
{
    public class WorkersBuilder
    {
        public IServiceCollection Services { get; }

        internal WorkersBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public WorkersBuilder AddWorker<TWorker, TInput, TOutput>(Action<WorkerConfiguration<TWorker>> worker)
            where TWorker : class, IWorker<TInput, TOutput>
        {
            Services.AddOptions<WorkerConfiguration<TWorker>>()
                .Configure(worker)
                .Validate(WorkerConfiguration<TWorker>.Validate);
            AddWorkerServices<TWorker, TInput, TOutput>();
            return this;
        }

        private void AddWorkerServices<TWorker, TInput, TOutput>()
            where TWorker : class, IWorker<TInput, TOutput>
        {
            Services.AddScoped<TWorker>();
            Services.AddHostedService<WorkerInvoker<TWorker, TInput, TOutput>>();
        }
    }
}
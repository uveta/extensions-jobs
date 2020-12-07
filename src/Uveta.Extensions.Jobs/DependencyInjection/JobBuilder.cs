using System;
using Microsoft.Extensions.DependencyInjection;
using Uveta.Extensions.Jobs.Abstractions.Queues;
using Uveta.Extensions.Jobs.Abstractions.Repositories;
using Uveta.Extensions.Jobs.Repositories;
using Uveta.Extensions.Jobs.Queues;
using Uveta.Extensions.Serialization;
using Uveta.Extensions.Jobs.Abstractions.Serialization;

namespace Uveta.Extensions.Jobs.DependencyInjection
{
    public class JobBuilder
    {
        public IServiceCollection Services { get; }

        public JobBuilder(IServiceCollection services, Action<InMemoryJobQueueConfiguration>? queue = null)
        {
            Services = services;
            services.AddSingleton<IJobRepository, InMemoryJobRepository>();
            var queueConfigurationBuilder = Services.AddOptions<InMemoryJobQueueConfiguration>();
            if (queue != null) queueConfigurationBuilder.Configure(queue);
            services.AddSingleton<IJobQueue, InMemoryJobQueue>();
            services.AddSingleton(typeof(ISerializer<>), typeof(JsonSerializer<>));
        }

        public JobBuilder AddRepository<T>() where T : class, IJobRepository
        {
            Services.AddSingleton<IJobRepository, T>();
            return this;
        }

        public JobBuilder AddQueue<T>() where T : class, IJobQueue
        {
            Services.AddSingleton<IJobQueue, T>();
            return this;
        }
    }
}
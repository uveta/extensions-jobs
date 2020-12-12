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

        public JobBuilder(IServiceCollection services)
        {
            Services = services;
            AddDefaultRepository();
            AddDefaultQueue();
            Services.AddSingleton(typeof(ISerializer<>), typeof(JsonSerializer<>));
        }

        public JobBuilder AddDefaultRepository()
        {
            Services.AddSingleton<IJobRepository, InMemoryJobRepository>();
            return this;
        }

        public JobBuilder AddDefaultQueue()
        {
            Services.AddOptions<InMemoryJobQueueConfiguration>();
            Services.AddSingleton<IJobQueue, InMemoryJobQueue>();
            return this;
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

        public JobBuilder AddDefaultQueueConfiguration(Action<InMemoryJobQueueConfiguration> configuration)
        {
            var queueConfigurationBuilder = Services.AddOptions<InMemoryJobQueueConfiguration>();
            queueConfigurationBuilder.Configure(configuration);
            return this;
        }
    }
}
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
            services.AddSingleton<IJobRepository, InMemoryJobRepository>();
            services.AddSingleton<IQueue, InMemoryJobQueue>();
            services.AddSingleton(typeof(ISerializer<>), typeof(JsonSerializer<>));
        }

        public JobBuilder AddRepository<T>() where T : class, IJobRepository
        {
            Services.AddSingleton<IJobRepository, T>();
            return this;
        }

        public JobBuilder AddQueue<T>() where T : class, IQueue
        {
            Services.AddSingleton<IQueue, T>();
            return this;
        }
    }
}
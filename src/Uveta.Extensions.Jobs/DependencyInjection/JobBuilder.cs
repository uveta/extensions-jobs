using Microsoft.Extensions.DependencyInjection;
using Uveta.Extensions.Jobs.Abstractions.Queues;
using Uveta.Extensions.Jobs.Abstractions.Repositories;
using Uveta.Extensions.Jobs.Repositories;
using Uveta.Extensions.Jobs.Queues;

namespace Uveta.Extensions.Jobs.DependencyInjection
{
    public class JobBuilder
    {
        public IServiceCollection Services { get; }

        public JobBuilder(IServiceCollection services)
        {
            Services = services;
            services.AddScoped<IJobRepository, InMemoryJobRepository>();
            services.AddScoped<IQueue, InMemoryJobQueue>();
        }

        public JobBuilder AddRepository<T>() where T : IJobRepository
        {
            return this;
        }

        public JobBuilder AddQueue<T>() where T : IQueue
        {
            return this;
        }
    }
}
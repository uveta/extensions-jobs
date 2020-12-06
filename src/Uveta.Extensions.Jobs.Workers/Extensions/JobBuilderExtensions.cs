using System;
using Microsoft.Extensions.DependencyInjection;
using Uveta.Extensions.Jobs.DependencyInjection;
using Uveta.Extensions.Jobs.Workers.DependencyInjection;
using Uveta.Extensions.Jobs.Workers.Services.Scopes;

namespace Uveta.Extensions.Jobs.Workers.Extensions
{
    public static class JobBuilderExtensions
    {
        public static JobBuilder AddWorkers(this JobBuilder jobBuilder, Action<WorkersBuilder> builder)
        {
            jobBuilder.AddDefaultScopeFactory();
            var workers = new WorkersBuilder(jobBuilder.Services);
            builder(workers);
            return jobBuilder;
        }

        private static JobBuilder AddDefaultScopeFactory(this JobBuilder builder)
        {
            builder.Services.AddSingleton<IWorkerScopeFactory, DefaultWorkerScopeFactory>();
            return builder;
        }
    }
}
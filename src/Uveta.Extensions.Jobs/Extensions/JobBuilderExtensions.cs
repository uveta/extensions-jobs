using System;
using Microsoft.Extensions.DependencyInjection;
using Uveta.Extensions.Jobs.DependencyInjection;

namespace Uveta.Extensions.Jobs.Extensions
{
    public static class JobBuilderExtensions
    {
        public static IServiceCollection AddJobs(this IServiceCollection services, Action<JobBuilder> builder)
        {
            var jobs = new JobBuilder(services);
            builder(jobs);
            return services;
        }
    }
}
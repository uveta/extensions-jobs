using System;
using Uveta.Extensions.Jobs.DependencyInjection;
using Uveta.Extensions.Jobs.Endpoints.DependencyInjection;

namespace Uveta.Extensions.Jobs.Endpoints.Extensions
{
    public static class EndpointsExtensions
    {
        public static JobBuilder AddEndpoints(this JobBuilder jobBuilder, Action<EndpointsBuilder> builder)
        {
            var services = jobBuilder.Services;
            var endpoints = new EndpointsBuilder(services);
            builder(endpoints);
            return jobBuilder;
        }
    }
}

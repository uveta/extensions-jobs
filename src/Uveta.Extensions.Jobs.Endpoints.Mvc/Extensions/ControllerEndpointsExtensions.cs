using System;
using Uveta.Extensions.Jobs.DependencyInjection;
using Uveta.Extensions.Jobs.Endpoints.Mvc.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Uveta.Extensions.Jobs.Endpoints.Mvc.Extensions
{
    public static class ControllerEndpointsExtensions
    {
        public static JobBuilder AddControllerEndpoints(
            this JobBuilder jobBuilder,
            Action<ControllerEndpointsBuilder> builder)
        {
            var services = jobBuilder.Services;
            var endpoints = new ControllerEndpointsBuilder(services);
            builder(endpoints);

            services.AddMvcCore(mvc => mvc
                .Conventions.Add(new JobControllerModelConvention(endpoints.Controllers))
            )
            .ConfigureApplicationPartManager(manager => manager
                .ApplicationParts.Add(new JobsApplicationPart(endpoints.Controllers))
            );
            return jobBuilder;
        }
    }
}
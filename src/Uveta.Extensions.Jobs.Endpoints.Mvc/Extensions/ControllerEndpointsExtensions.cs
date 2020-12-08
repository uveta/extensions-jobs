using System;
using Uveta.Extensions.Jobs.DependencyInjection;
using Uveta.Extensions.Jobs.Endpoints.DependencyInjection;
using Uveta.Extensions.Jobs.Endpoints.Extensions;
using Uveta.Extensions.Jobs.Endpoints.Mvc.DependencyInjection;
using Uveta.Extensions.Jobs.Endpoints.Mvc.Services;
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
                .Conventions.Add(new JobControllerModelConvention(endpoints.Configurations))
            )
            .ConfigureApplicationPartManager(manager => manager
                .ApplicationParts.Add(new JobsApplicationPart(endpoints.Configurations))
            );
            return jobBuilder;
        }
    }
}
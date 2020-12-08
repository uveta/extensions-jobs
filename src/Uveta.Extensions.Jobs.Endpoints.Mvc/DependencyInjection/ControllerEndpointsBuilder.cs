using Uveta.Extensions.Jobs.Endpoints.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Uveta.Extensions.Jobs.Endpoints.Mvc.DependencyInjection
{
    public sealed class ControllerEndpointsBuilder : EndpointsBuilder
    {
        internal readonly IList<EndpointConfiguration> Configurations = new List<EndpointConfiguration>();

        internal ControllerEndpointsBuilder(IServiceCollection services) : base(services)
        {
        }

        public ControllerEndpointsBuilder AddController<TEndpoint, TInput, TOutput>(Action<EndpointConfiguration<TEndpoint>> endpoint)
            where TEndpoint : class, IEndpoint<TInput, TOutput>
        {
            AddConfiguration<TEndpoint, TInput, TOutput>(endpoint);
            AddEndpointService<TEndpoint, TInput, TOutput>();
            var configuration = new EndpointConfiguration<TEndpoint>();
            endpoint(configuration);
            Configurations.Add(configuration);
            return this;
        }
    }
}
using System;
using Microsoft.Extensions.DependencyInjection;
using Uveta.Extensions.Jobs.Abstractions.Endpoints;

namespace Uveta.Extensions.Jobs.Endpoints.DependencyInjection
{
    public sealed class EndpointsBuilder
    {
        internal IServiceCollection Services { get; }

        internal EndpointsBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public EndpointsBuilder AddEndpoint<TEndpoint, TInput, TOutput>(
            Action<EndpointConfiguration<TEndpoint>> endpoint)
            where TEndpoint : IEndpoint
        {
            AddConfiguration(endpoint);
            AddEndpointService<TEndpoint, TInput, TOutput>();
            return this;
        }

        private void AddConfiguration<TEndpoint>(
            Action<EndpointConfiguration<TEndpoint>> endpoint)
            where TEndpoint : IEndpoint
        {
            Services
                .AddOptions<EndpointConfiguration<TEndpoint>>()
                .Configure(endpoint)
                .Validate(EndpointConfiguration.Validate);
        }

        private void AddEndpointService<TEndpoint, TInput, TOutput>()
            where TEndpoint : IEndpoint
        {
            Services.AddSingleton<Endpoint<TEndpoint, TInput, TOutput>>();
        }
    }
}

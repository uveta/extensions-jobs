using System;
using Microsoft.Extensions.DependencyInjection;

namespace Uveta.Extensions.Jobs.Endpoints.DependencyInjection
{
    public class EndpointsBuilder
    {
        public IServiceCollection Services { get; }

        internal EndpointsBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public EndpointsBuilder AddEndpoint<TEndpoint, TInput, TOutput>(
            Action<EndpointConfiguration<TEndpoint>> endpoint)
            where TEndpoint : class, IEndpoint<TInput, TOutput>
        {
            AddConfiguration<TEndpoint, TInput, TOutput>(endpoint);
            AddEndpointService<TEndpoint, TInput, TOutput>();
            return this;
        }

        private void AddEndpointService<TEndpoint, TInput, TOutput>()
            where TEndpoint : class, IEndpoint<TInput, TOutput>
        {
            Services.AddSingleton<Endpoint<TEndpoint, TInput, TOutput>>();
        }

        private void AddConfiguration<TEndpoint, TInput, TOutput>(
            Action<EndpointConfiguration<TEndpoint>> endpoint)
            where TEndpoint : class, IEndpoint<TInput, TOutput>
        {
            Services
                .AddOptions<EndpointConfiguration<TEndpoint>>()
                .Configure(endpoint)
                .Validate(EndpointConfiguration.Validate);
        }
    }
}

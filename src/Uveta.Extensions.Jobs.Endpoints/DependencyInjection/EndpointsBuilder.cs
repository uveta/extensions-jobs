using System;
using Microsoft.Extensions.DependencyInjection;

namespace Uveta.Extensions.Jobs.Endpoints.DependencyInjection
{
    public class EndpointsBuilder
    {
        public IServiceCollection Services { get; }

        internal protected EndpointsBuilder(IServiceCollection services)
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

        protected void AddEndpointService<TEndpoint, TInput, TOutput>()
            where TEndpoint : class, IEndpoint<TInput, TOutput>
        {
            Services.AddSingleton<Endpoint<TEndpoint, TInput, TOutput>>();
        }

        protected void AddConfiguration<TEndpoint, TInput, TOutput>(
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

using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Uveta.Extensions.Jobs.Abstractions.Endpoints;

namespace Uveta.Extensions.Jobs.Endpoints.Mvc.DependencyInjection
{
    public sealed class ControllerEndpointsBuilder
    {
        private readonly List<ControllerEndpoint> _controllers = new();
        internal IReadOnlyCollection<ControllerEndpoint> Controllers => _controllers;
        internal IServiceCollection Services { get; }

        internal ControllerEndpointsBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public ControllerEndpointsBuilder AddController<TEndpoint, TInput, TOutput>(
            Action<EndpointConfiguration<TEndpoint>> endpoint)
            where TEndpoint : IEndpoint
        {
            AddConfiguration(endpoint);
            AddEndpointService<TEndpoint, TInput, TOutput>();
            var configuration = new EndpointConfiguration<TEndpoint>();
            endpoint(configuration);
            var controller = new ControllerEndpoint(
                typeof(TInput),
                typeof(TOutput),
                typeof(TEndpoint),
                configuration);
            _controllers.Add(controller);
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
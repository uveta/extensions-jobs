using System;

namespace Uveta.Extensions.Jobs.Endpoints.Mvc.DependencyInjection
{
    public class ControllerEndpoint
    {
        public ControllerEndpoint(
            Type input,
            Type output,
            Type endpoint,
            EndpointConfiguration configuration)
        {
            Input = input;
            Output = output;
            Endpoint = endpoint;
            Configuration = configuration;
        }

        public Type Input { get; }
        public Type Output { get; }
        public Type Endpoint { get; }
        public EndpointConfiguration Configuration { get; }
    }
}
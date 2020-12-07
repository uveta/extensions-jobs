namespace Uveta.Extensions.Jobs.Endpoints
{
    public abstract class EndpointConfiguration
    {
        private string? _name;

        public string Service { get; set; } = null!;
        public string Area { get; set; } = null!;

        public string Name
        {
            get { return _name ?? Area; }
            set { _name = value; }
        }

        public static bool Validate(EndpointConfiguration configuration)
        {
            if (configuration.Service is null) return false;
            if (configuration.Area is null) return false;
            return true;
        }
    }

    public class EndpointConfiguration<TEndpoint> : EndpointConfiguration
        where TEndpoint : IEndpoint
    {
        public EndpointConfiguration<TEndpoint> UseService(string service)
        {
            Service = service;
            return this;
        }

        public EndpointConfiguration<TEndpoint> UseArea(string area)
        {
            Area = area;
            return this;
        }

        public EndpointConfiguration<TEndpoint> UseName(string name)
        {
            Name = name;
            return this;
        }
    }
}

using System.Runtime.Serialization;

namespace Uveta.Extensions.Jobs.Abstractions.Models
{
    public sealed class JobIdentifier
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "service")]
        public string Service { get; set; }

        [DataMember(Name = "area")]
        public string Area { get; set; }

        public JobIdentifier(string id, string service, string area)
        {
            Id = id;
            Service = service;
            Area = area;
        }
    }
}

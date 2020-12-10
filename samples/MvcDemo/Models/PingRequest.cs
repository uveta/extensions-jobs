using System.Runtime.Serialization;

namespace MvcDemo.Models
{
    public class PingRequest
    {
        [DataMember(Name = "message")]
        public string Message { get; set; } = "ping";
    }
}
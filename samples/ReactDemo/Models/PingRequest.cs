using System.Runtime.Serialization;

namespace ReactDemo.Models
{
    public class PingRequest
    {
        [DataMember(Name = "message")]
        public string Message { get; set; } = "ping";
    }
}
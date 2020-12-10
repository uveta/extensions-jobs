using System.Runtime.Serialization;

namespace MvcDemo.Models
{
    public class PingResponse
    {
        [DataMember(Name = "message")]
        public string Message { get; set; } = "pong";
    }
}
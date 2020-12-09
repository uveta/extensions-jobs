using System.Runtime.Serialization;

namespace ReactDemo.Models
{
    public class PingResponse
    {
        [DataMember(Name = "message")]
        public string Message { get; set; } = "pong";
    }
}
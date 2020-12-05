using System.Runtime.Serialization;

namespace Uveta.Extensions.Jobs.Abstractions.Models
{
    public sealed class Job
    {
        /// <summary>
        /// Job metadata used for identification and status tracking
        /// </summary>
        [DataMember(Name = "header")]
        public JobHeader Header { get; set; }

        /// <summary>
        /// Job input parameters
        /// </summary>
        [DataMember(Name = "input")]
        public string? Input { get; set; }

        /// <summary>
        /// Job output value
        /// </summary>
        [DataMember(Name = "output")]
        public string? Output { get; set; }

        public Job(JobHeader header)
        {
            Header = header;
        }
    }
}

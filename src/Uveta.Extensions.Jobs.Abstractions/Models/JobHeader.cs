using System;
using System.Runtime.Serialization;

namespace Uveta.Extensions.Jobs.Abstractions.Models
{
    public sealed class JobHeader
    {
        /// <summary>
        /// Unique job identification consisting of requesting service, area inside service and unique id
        /// </summary>
        [DataMember(Name = "identifier")]
        public JobIdentifier Identifier { get; set; }

        /// <summary>
        /// Current job state
        /// </summary>
        [DataMember(Name = "state")]
        public JobState? State { get; set; }

        /// <summary>
        /// Error details in case of processing issues
        /// </summary>
        [DataMember(Name = "error")]
        public JobError? Error { get; set; }

        /// <summary>
        /// Estimated time of arrival
        /// </summary>
        [DataMember(Name = "eta")]
        public TimeSpan? ETA { get; set; }

        /// <summary>
        /// Job create timestamp
        /// </summary>
        [DataMember(Name = "created")]
        public DateTimeOffset? Created { get; set; }

        /// <summary>
        /// Job end timestamp
        /// </summary>
        [DataMember(Name = "ended")]
        public DateTimeOffset? Ended { get; set; }

        /// <summary>
        /// Job start timestamp
        /// </summary>
        [DataMember(Name = "started")]
        public DateTimeOffset? Started { get; set; }

        public JobHeader(JobIdentifier identifier)
        {
            Identifier = identifier;
        }
    }
}
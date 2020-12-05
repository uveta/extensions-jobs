using System;

namespace Uveta.Extensions.Jobs.Abstractions.Models
{
    public static class JobExtensions
    {
        public static void Finish(this Job job, string output)
        {
            job.Header.State = JobState.Finished;
            job.Header.Ended = DateTimeOffset.UtcNow;
            job.Output = output;
        }

        public static void Start(this Job job, TimeSpan? eta = null)
        {
            job.Header.Started = DateTimeOffset.UtcNow;
            job.Header.State = JobState.Started;
            job.Header.ETA = eta;
        }

        public static void Cancel(this Job job)
        {
            job.Header.State = JobState.Cancelled;
            job.Header.Ended = DateTimeOffset.UtcNow;
        }

        public static void Error(this Job job, Exception e)
        {
            var error = JobError.FromException(e);
            job.Error(error);
        }

        public static void Error(this Job job, JobError error)
        {
            job.Header.State = JobState.Error;
            job.Header.Ended = DateTimeOffset.UtcNow;
            job.Header.Error = error;
        }
    }
}

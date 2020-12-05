using System.Collections.Generic;

namespace Uveta.Extensions.Jobs.Abstractions.Models
{
    public enum JobState
    {
        Created = 0,
        Started = 50,
        Cancelled = 80,
        Finished = 100,
        Error = 150
    }

    public static class JobStateExtensions
    {
        private static readonly ISet<JobState> _finishedStates = new HashSet<JobState>
        {
            JobState.Cancelled, JobState.Finished, JobState.Error
        };

        private static readonly ISet<JobState> _cancelledStates = new HashSet<JobState>
        {
            JobState.Cancelled
        };

        private static readonly ISet<JobState> _finishedSuccessStates = new HashSet<JobState>
        {
            JobState.Finished
        };

        public static bool IsComplete(this JobState? state)
        {
            if (!state.HasValue) return false;
            return _finishedStates.Contains(state.Value);
        }

        public static bool IsSuccess(this JobState? state)
        {
            if (!state.HasValue) return false;
            return _finishedSuccessStates.Contains(state.Value);
        }

        public static bool IsCancelled(this JobState? state)
        {
            if (!state.HasValue) return false;
            return _cancelledStates.Contains(state.Value);
        }
    }
}
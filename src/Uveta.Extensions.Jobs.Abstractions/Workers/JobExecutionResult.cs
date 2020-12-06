using System;
using Uveta.Extensions.Jobs.Abstractions.Models;

namespace Uveta.Extensions.Jobs.Abstractions.Workers
{
    public abstract class JobExecutionResult
    {
        public bool IsFinished { get; private set; } = false;
        public bool IsCancelled { get; private set; } = false;
        public bool IsError => !IsFinished;
        public JobError? Error { get; private set; }

        public static JobExecutionResult<T> Finished<T>(T output) where T : class
        {
            return new JobExecutionResult<T>(output)
            {
                IsFinished = true
            };
        }

        public static JobExecutionResult<T> Cancelled<T>() where T : class
        {
            return new JobExecutionResult<T>
            {
                IsFinished = false,
                IsCancelled = true
            };
        }

        public static JobExecutionResult<T> Failed<T>(JobError error) where T : class
        {
            if (error is null) throw new ArgumentNullException(nameof(error));
            return new JobExecutionResult<T>
            {
                IsFinished = false,
                Error = error
            };
        }
    }

    public sealed class JobExecutionResult<T> : JobExecutionResult
    {
        internal JobExecutionResult()
        {
        }

        internal JobExecutionResult(T output)
        {
            Output = output;
        }

        public T? Output { get; }
    }
}

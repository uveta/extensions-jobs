using System;
using System.Threading;
using System.Threading.Tasks;

namespace Uveta.Extensions.Jobs.Abstractions.Workers
{
    public interface IWorker<TInput, TOutput> where TOutput : class
    {
        TimeSpan? EstimateExecutionTime(TInput input);
        Task<JobExecutionResult<TOutput>> ExecuteAsync(TInput input, CancellationToken cancel);
        Task DeleteOutputAsync(TInput input, TOutput output, CancellationToken cancel);
    }
}

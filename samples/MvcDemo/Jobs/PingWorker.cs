using System;
using System.Threading;
using System.Threading.Tasks;
using MvcDemo.Models;
using Uveta.Extensions.Jobs.Abstractions.Workers;

namespace MvcDemo.Jobs
{
    public class PingWorker : IWorker<PingRequest, PingResponse>
    {
        public TimeSpan? EstimateExecutionTime(PingRequest input)
        {
            return TimeSpan.FromSeconds(30);
        }

        public async Task<JobExecutionResult<PingResponse>> ExecuteAsync(PingRequest input, CancellationToken cancel)
        {
            var output = new PingResponse();
            await Task.Delay(TimeSpan.FromSeconds(10), cancel);
            return JobExecutionResult.Finished(output);
        }

        public Task DeleteOutputAsync(PingRequest input, PingResponse? output, CancellationToken cancel)
        {
            return Task.CompletedTask;
        }
    }
}
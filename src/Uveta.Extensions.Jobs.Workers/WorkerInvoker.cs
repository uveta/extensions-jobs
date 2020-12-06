using System;
using System.Threading;
using System.Threading.Tasks;
using Uveta.Extensions.Jobs.Abstractions.Serialization;
using Uveta.Extensions.Jobs.Abstractions.Models;
using Uveta.Extensions.Jobs.Abstractions.Queues;
using Uveta.Extensions.Jobs.Abstractions.Repositories;
using Uveta.Extensions.Jobs.Abstractions.Workers;
using Uveta.Extensions.Jobs.Workers.Services.Scopes;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Uveta.Extensions.Jobs.Workers
{
    internal class WorkerInvoker<TWorker, TInput, TOutput> : IHostedService
        where TWorker : IWorker<TInput, TOutput>
    {
        private readonly WorkerConfiguration<TWorker> _workerConfiguration;
        private readonly ISerializer<TInput> _inputSerializer;
        private readonly ISerializer<TOutput> _outputSerializer;
        private readonly IQueue _jobQueue;
        private readonly IWorkerScopeFactory _scopeFactory;
        private readonly IJobRepository _jobRepository;
        private readonly IQueueConsumer _consumer;
        private readonly ILogger _logger;

        public WorkerInvoker(
            IJobRepository jobRepository,
            IOptions<WorkerConfiguration<TWorker>> workerConfiguration,
            ISerializer<TInput> inputSerializer,
            ISerializer<TOutput> outputSerializer,
            IQueue jobQueue,
            IWorkerScopeFactory scopeFactory,
            ILogger<WorkerInvoker<TWorker, TInput, TOutput>> logger)
        {
            _logger = logger;
            _jobRepository = jobRepository;
            _workerConfiguration = workerConfiguration.Value;
            _inputSerializer = inputSerializer;
            _outputSerializer = outputSerializer;
            _jobQueue = jobQueue;
            _scopeFactory = scopeFactory;

            _consumer = _jobQueue.GetConsumer(CreateQueueConfiguration());
            _consumer.JobQueued += OnNewJob;
            _consumer.Error += OnError;
        }

        public async Task StartAsync(CancellationToken cancel)
        {
            await _consumer.StartAsync(_workerConfiguration.MaximumConcurrentCalls, cancel);
        }

        public Task StopAsync(CancellationToken cancel)
        {
            _consumer.Dispose();
            return Task.CompletedTask;
        }

        private QueueConfiguration CreateQueueConfiguration()
        {
            return new QueueConfiguration(_workerConfiguration.QueueName);
        }

        private async Task OnNewJob(Job job, Delivery delivery)
        {
            bool updateStatus = true;
            var cancel = new CancellationTokenSource(
                _workerConfiguration.MaximumAllowedExecutionTime)
                .Token;
            try
            {
                if (job.Input is null) throw new ArgumentNullException(nameof(job.Input));
                using var scope = _scopeFactory.CreateScope(job);
                var worker = scope.GetWorker<TWorker>();
                var input = _inputSerializer.Deserialize(job.Input);
                job.Start(worker.EstimateExecutionTime(input));
                await _jobRepository.UpdateAsync(job, cancel);
                var result = await worker.ExecuteAsync(input, cancel);
                if (result.IsCancelled)
                {
                    job.Cancel();
                    delivery.Acknowledge = true;
                    return;
                }
                if (result.IsFinished)
                {
                    if (result.Output is null) throw new InvalidOperationException("Job finished without output");
                    string output = _outputSerializer.Serialize(result.Output);
                    job.Finish(output);
                    delivery.Acknowledge = true;
                    return;
                }
                if (result.IsError)
                {
                    if (delivery.SupportsRepeat)
                    {
                        updateStatus = false;
                    }
                    else
                    {
                        delivery.Acknowledge = true;
                        if (result.Error is not null)
                        {
                            job.Error(result.Error);
                        }
                        else
                        {
                            var genericError = JobError.FromMessage("Job execution failed");
                            job.Error(genericError);
                        }
                    }
                }
            }
            catch (OperationCanceledException e)
            {
                if (delivery.SupportsRepeat)
                {
                    updateStatus = false;
                }
                else
                {
                    delivery.Acknowledge = true;
                    job.Error(e);
                }
            }
            catch (Exception e)
            {
                delivery.Acknowledge = true;
                job.Error(e);
            }
            finally
            {
                if (updateStatus) await _jobRepository.UpdateAsync(job, CancellationToken.None);
            }
        }

        public async Task DeleteOutputAsync(string jobId, CancellationToken cancel)
        {
            if (_workerConfiguration.Service is null || _workerConfiguration.Area is null) return;
            var identifier = new JobIdentifier(
                jobId,
                _workerConfiguration.Service,
                _workerConfiguration.Area);
            var job = await _jobRepository.GetAsync(identifier, cancel);
            if (job is null) return;
            using var scope = _scopeFactory.CreateScope(job);
            var worker = scope.GetWorker<TWorker>();
            if (job.Input is null) return;
            var input = _inputSerializer.Deserialize(job.Input);
            TOutput? output = default;
            if (job.Output != null) output = _outputSerializer.Deserialize(job.Output);
            await worker.DeleteOutputAsync(input, output, cancel);
        }

        private async Task OnError(ErrorArgs args, Delivery delivery)
        {
            _logger.LogWarning(args.Exception, args.Message);
            if (args.Job is not null)
            {
                var job = args.Job;
                JobError jobError;
                if (args.Exception is null) jobError = JobError.FromMessage(args.Message);
                else jobError = JobError.FromException(args.Exception);
                job.Error(jobError);
                await _jobRepository.UpdateAsync(job, CancellationToken.None);
            }
        }
    }
}

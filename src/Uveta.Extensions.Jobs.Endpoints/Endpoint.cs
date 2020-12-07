using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Uveta.Extensions.Jobs.Abstractions.Models;
using Uveta.Extensions.Jobs.Abstractions.Queues;
using Uveta.Extensions.Jobs.Abstractions.Repositories;
using Uveta.Extensions.Jobs.Abstractions.Serialization;

namespace Uveta.Extensions.Jobs.Endpoints
{
    public sealed class Endpoint<TEndpoint, TInput, TOutput> : IEndpoint<TInput, TOutput>
        where TEndpoint : IEndpoint<TInput, TOutput>
    {
        private readonly EndpointConfiguration _configuration;
        private readonly ISerializer<TInput> _inputSerializer;
        private readonly ISerializer<TOutput> _outputSerializer;
        private readonly IJobRepository _repository;
        private readonly IJobQueue _queue;

        public Endpoint(
            IOptions<EndpointConfiguration<TEndpoint>> configuration,
            ISerializer<TInput> inputSerializer,
            ISerializer<TOutput> outputSerializer,
            IJobRepository repository,
            IJobQueue queue)
        {
            _configuration = configuration.Value;
            _inputSerializer = inputSerializer;
            _outputSerializer = outputSerializer;
            _repository = repository;
            _queue = queue;
        }

        public async Task<Job> RequestNewJobAsync(TInput input, CancellationToken cancel)
        {
            var identifier = CreateJobIdentifier(Guid.NewGuid().ToString("N"));
            var header = new JobHeader(identifier)
            {
                State = JobState.Created,
                Created = DateTimeOffset.UtcNow
            };
            var job = new Job(header)
            {
                Input = _inputSerializer.Serialize(input)
            };
            await _repository.CreateAsync(job, cancel);
            using var publisher = _queue.GetPublisher(CreatePublisherConfiguration());
            await publisher.EnqueueAsync(job, cancel);
            return job;
        }

        public async Task<Job?> GetJobAsync(string id, CancellationToken cancel)
        {
            var identifier = CreateJobIdentifier(id);
            return await _repository.GetAsync(identifier, cancel);
        }

        public async Task<TOutput?> GetJobOutputAsync(string id, CancellationToken cancel)
        {
            JobIdentifier identifier = CreateJobIdentifier(id);
            var job = await _repository.GetAsync(identifier, cancel);
            if (job is null || !job.Header.State.IsSuccess()) return default;
            if (job.Output is null) return default;
            return _outputSerializer.Deserialize(job.Output);
        }

        public async Task DeleteJobAsync(string id, CancellationToken cancel)
        {
            JobIdentifier identifier = CreateJobIdentifier(id);
            await _repository.DeleteAsync(identifier, cancel);
        }

        private JobIdentifier CreateJobIdentifier(string id)
        {
            return new JobIdentifier(id, _configuration.Service, _configuration.Area);
        }

        private PublisherConfiguration CreatePublisherConfiguration()
        {
            var queue = new QueueConfiguration(CreateQueueName());
            return new PublisherConfiguration(queue);
        }

        private string CreateQueueName()
        {
            return $"jobs.{_configuration.Service}.{_configuration.Area}";
        }
    }
}

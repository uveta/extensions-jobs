using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Uveta.Extensions.Jobs.Abstractions.Models;
using Uveta.Extensions.Jobs.Abstractions.Repositories;

namespace Uveta.Extensions.Jobs.Repositories
{
    internal class InMemoryJobRepository : IJobRepository
    {
        private static readonly ConcurrentDictionary<string, Job> _jobs = new ConcurrentDictionary<string, Job>();

        public Task CreateAsync(Job job, CancellationToken cancel)
        {
            _jobs.TryAdd(job.Header.Identifier.Id, job);
            return Task.CompletedTask;
        }

        public Task<Job?> GetAsync(JobIdentifier id, CancellationToken cancel)
        {
            Job? job = null;
            if (_jobs.ContainsKey(id.Id)) job = _jobs[id.Id];
            return Task.FromResult(job);
        }

        public Task<bool> UpdateAsync(Job job, CancellationToken cancel)
        {
            _jobs.AddOrUpdate(job.Header.Identifier.Id, job, (_, __) => job);
            return Task.FromResult(true);
        }

        public Task DeleteAsync(JobIdentifier id, CancellationToken cancel)
        {
            _jobs.TryRemove(id.Id, out _);
            return Task.CompletedTask;
        }
    }
}
using System.Threading;
using System.Threading.Tasks;
using Uveta.Extensions.Jobs.Abstractions.Models;

namespace Uveta.Extensions.Jobs.Abstractions.Repositories
{
    public interface IJobRepository
    {
        Task CreateAsync(Job job, CancellationToken cancel);
        Task<Job?> GetAsync(JobIdentifier id, CancellationToken cancel);
        Task<bool> UpdateAsync(Job job, CancellationToken cancel);
        Task DeleteAsync(JobIdentifier id, CancellationToken cancel);
    }
}

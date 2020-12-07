using System.Threading;
using System.Threading.Tasks;
using Uveta.Extensions.Jobs.Abstractions.Models;

namespace Uveta.Extensions.Jobs.Endpoints
{
    public interface IEndpoint
    {
    }

    public interface IEndpoint<TInput, TOutput> : IEndpoint
    {
        Task<Job> RequestNewJobAsync(TInput input, CancellationToken cancel);
        Task<Job?> GetJobAsync(string id, CancellationToken cancel);
        Task<TOutput?> GetJobOutputAsync(string id, CancellationToken cancel);
        Task DeleteJobAsync(string id, CancellationToken cancel);
    }
}
using Uveta.Extensions.Jobs.Abstractions.Models;

namespace Uveta.Extensions.Jobs.Workers.Services.Scopes
{
    public interface IWorkerScopeFactory
    {
        IWorkerScope CreateScope(Job job);
    }
}

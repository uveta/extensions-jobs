using Uveta.Extensions.Jobs.Abstractions.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Uveta.Extensions.Jobs.Workers.Services.Scopes
{
    internal class DefaultWorkerScopeFactory : IWorkerScopeFactory
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DefaultWorkerScopeFactory(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public IWorkerScope CreateScope(Job _)
        {
            var scope = _scopeFactory.CreateScope();
            return new DefaultWorkerScope(scope);
        }
    }
}

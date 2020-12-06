using System;
using Microsoft.Extensions.DependencyInjection;

namespace Uveta.Extensions.Jobs.Workers.Services.Scopes
{
    internal class DefaultWorkerScope : IWorkerScope
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultWorkerScope(IServiceScope scope)
        {
            _serviceProvider = scope.ServiceProvider;
        }

        public TWorker GetWorker<TWorker>() where TWorker : notnull
        {
            return _serviceProvider.GetRequiredService<TWorker>();
        }

        public void Dispose()
        {
        }
    }
}

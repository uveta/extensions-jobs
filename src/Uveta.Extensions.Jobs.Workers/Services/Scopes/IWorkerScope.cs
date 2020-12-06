using System;

namespace Uveta.Extensions.Jobs.Workers.Services.Scopes
{
    public interface IWorkerScope : IDisposable
    {
        TWorker GetWorker<TWorker>() where TWorker : notnull;
    }
}
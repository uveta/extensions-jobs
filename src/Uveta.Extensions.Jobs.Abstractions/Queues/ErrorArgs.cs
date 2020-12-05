using System;
using Uveta.Extensions.Jobs.Abstractions.Models;

namespace Uveta.Extensions.Jobs.Abstractions.Queues
{
    public class ErrorArgs
    {
        public Exception? Exception { get; }
        public string Message { get; }
        public Job? Job { get; }

        public ErrorArgs(Exception e, Job? job = default) : this(e, e.Message, job)
        {
        }

        public ErrorArgs(string message, Job? job = default) : this(null, message, job)
        {
        }

        public ErrorArgs(Exception? e, string message, Job? job = default)
        {
            Exception = e;
            Message = message;
            Job = job;
        }
    }
}

using System;

namespace Uveta.Extensions.Jobs.Workers
{
    public class WorkerConfiguration<T>
    {
        public TimeSpan MaximumAllowedExecutionTime { get; set; } = TimeSpan.FromMinutes(5);
        public int MaximumConcurrentCalls { get; set; } = 10;
        public string? Service { get; set; }
        public string? Area { get; set; }
        public string QueueName => GetQueueName();

        public WorkerConfiguration<T> UseService(string service)
        {
            Service = service;
            return this;
        }

        public WorkerConfiguration<T> UseArea(string area)
        {
            Area = area;
            return this;
        }

        public WorkerConfiguration<T> UseMaximumAllowedExecutionTime(TimeSpan time)
        {
            MaximumAllowedExecutionTime = time;
            return this;
        }

        public WorkerConfiguration<T> UseMaximumConcurrentCalls(int count)
        {
            MaximumConcurrentCalls = count;
            return this;
        }

        public static bool Validate(WorkerConfiguration<T> configuration)
        {
            if (configuration.MaximumAllowedExecutionTime.TotalSeconds < 1) return false;
            if (configuration.MaximumConcurrentCalls <= 0) return false;
            if (configuration.Service is null) return false;
            if (configuration.Area is null) return false;
            return true;
        }

        private string GetQueueName()
        {
            return $"jobs.{Service}.{Area}";
        }
    }
}

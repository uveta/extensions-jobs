using System;
using System.Collections;
using Uveta.Extensions.Jobs.Abstractions.Models;

namespace Uveta.Extensions.Jobs.Abstractions.Exceptions
{
    public class JobFailed : Exception
    {
        public JobFailed()
        {
        }

        public JobFailed(string message) : base(message)
        {
        }

        public JobFailed(string message, Exception innerException)
            : this(message, null, innerException)
        {
        }

        private JobFailed(
            string? message,
            IDictionary? additionalData = null,
            Exception? innerException = null)
            : base(message, innerException)
        {
            if (additionalData is not null) this.AddData(additionalData);
        }

        public static JobFailed FromError(JobError error)
        {
            string message = error.Message;
            var innerException = GenerateInnerException(error);
            if (innerException is null)
                return new JobFailed(message, error.Data);
            else
                return new JobFailed(message, innerException);
        }

        private static Exception? GenerateInnerException(JobError error)
        {
            var exceptionType = Type.GetType(error.Type, false, true);
            if (exceptionType is null) return null;
            try
            {
                var exception = Activator.CreateInstance(exceptionType, error.Message) as Exception;
                exception?.SetStackTrace(error.StackTrace);
                exception?.AddData(error.Data);
                return exception;
            }
            catch
            {
                return null;
            }
        }
    }
}

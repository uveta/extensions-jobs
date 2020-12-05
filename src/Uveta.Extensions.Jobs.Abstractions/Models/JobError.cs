using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Uveta.Extensions.Jobs.Abstractions.Models
{
    public class JobError
    {
        /// <summary>
        /// Error detail message
        /// </summary>
        [DataMember(Name = "message")]
        public string Message { get; set; } = null!;

        /// <summary>
        /// Exception type
        /// </summary>
        [DataMember(Name = "type")]
        public string? Type { get; set; }

        /// <summary>
        /// Exception stack trace
        /// </summary>
        [DataMember(Name = "stackTrace")]
        public string? StackTrace { get; set; }

        /// <summary>
        /// Exception additional data
        /// </summary>
        [DataMember(Name = "data")]
        public IDictionary Data { get; set; } = new Dictionary<object, object>();

        public static JobError FromException(Exception exception)
        {
            var error = FromMessage(exception.Message);
            error.Type = exception.GetType().FullName;
            error.StackTrace = exception.StackTrace;
            foreach (DictionaryEntry data in exception.Data)
                error.Data.Add(data.Key, data.Value);
            return error;
        }

        public static JobError FromMessage(string? message)
        {
            if (message is null) throw new ArgumentNullException(nameof(message));
            return new JobError
            {
                Message = message
            };
        }
    }

    public static class JobErrorExtensions
    {
        public static bool IsException(this JobError error)
        {
            return error.Type != null;
        }
    }
}
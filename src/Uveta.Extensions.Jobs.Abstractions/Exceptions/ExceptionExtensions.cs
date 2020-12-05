using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;

namespace Uveta.Extensions.Jobs.Abstractions.Exceptions
{
    public static class ExceptionExtensions
    {
        private static readonly FieldInfo STACK_TRACE_STRING_FI =
            typeof(Exception).GetField("_stackTraceString", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly Type TRACE_FORMAT_TI =
            Type.GetType("System.Diagnostics.StackTrace").GetNestedType("TraceFormat", BindingFlags.NonPublic);

        private static readonly MethodInfo TRACE_TO_STRING_MI =
            typeof(StackTrace).GetMethod("ToString", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { TRACE_FORMAT_TI }, null);

        public static void SetStackTrace(this Exception target, StackTrace stack)
        {
            var getStackTraceString = TRACE_TO_STRING_MI.Invoke(stack, new object[] { Enum.GetValues(TRACE_FORMAT_TI).GetValue(0) });
            STACK_TRACE_STRING_FI.SetValue(target, getStackTraceString);
        }

        public static void SetStackTrace(this Exception target, string? stack)
        {
            STACK_TRACE_STRING_FI.SetValue(target, stack);
        }

        public static void AddData(this Exception target, IDictionary additionalData)
        {
            foreach (var key in additionalData.Keys)
            {
                target.Data.Add(key, additionalData[key]);
            }
        }
    }
}

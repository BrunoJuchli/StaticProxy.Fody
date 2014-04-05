namespace StaticProxy.Interceptor
{
    using System;
    using System.Reflection;

    internal static class ExceptionExtensions
    {
        public static Exception PreserveStackTrace(this Exception exception)
        {
            var remoteStackTraceString = typeof(Exception).GetField("_remoteStackTraceString", BindingFlags.Instance | BindingFlags.NonPublic);
            remoteStackTraceString.SetValue(exception, exception.StackTrace + Environment.NewLine);

            return exception;
        }
    }
}
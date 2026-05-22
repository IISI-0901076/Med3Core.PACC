using System.Diagnostics;

namespace Med3Core.Log.Common.Logging
{
    /// <summary>Shim: TraceHelper — 對齊 Bootstrapper.Common 的 Log_RunWithTracingAndLogAsync。</summary>
    public static class TraceHelper
    {
        /// <summary>包覆 async 邏輯加上 tracing + log。本地版直接執行委派。</summary>
        public static async Task<T> Log_RunWithTracingAndLogAsync<T>(
            string methodName,
            NLog.Logger logger,
            ActivitySource activitySource,
            Func<Task<T>> action)
        {
            using Activity? activity = activitySource.StartActivity("Svc-" + methodName);
            try
            {
                T result = await action();
                logger.Trace("[{Method}] OK", methodName);
                return result;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "[{Method}] FAIL", methodName);
                throw;
            }
        }
    }

    /// <summary>Shim: NLog 設定載入器。</summary>
    public static class NLogConfigurationLoader
    {
        public static void Log_LoadEmbeddedConfig() { /* no-op in local mode */ }
    }

    /// <summary>Shim: NLogExtension。</summary>
    public static class NLogExtension
    {
        public static async Task<T> Log_RunWithSpendTimeScopeAsync<T>(
            Func<Task<T>> func,
            Microsoft.Extensions.Logging.ILogger logger,
            string message = "",
            string functionName = "")
        {
            return await func();
        }
    }

    /// <summary>Shim: NLogHelper scope。</summary>
    public static class NLogHelper
    {
        public static IDisposable Log_PushCaseScope(string name) => new NoOpDisposable();

        private class NoOpDisposable : IDisposable
        {
            public void Dispose() { }
        }
    }
}

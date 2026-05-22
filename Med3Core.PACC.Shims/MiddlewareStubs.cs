namespace Med3Core.Log.Common.Middleware
{
    /// <summary>Shim: NLog MDC Middleware（pass-through）。</summary>
    public class NLogMdcMiddleware
    {
        private readonly Microsoft.AspNetCore.Http.RequestDelegate _next;
        public NLogMdcMiddleware(Microsoft.AspNetCore.Http.RequestDelegate next) { _next = next; }
        public Task InvokeAsync(Microsoft.AspNetCore.Http.HttpContext context) => _next(context);
    }

    /// <summary>Shim: 驗證錯誤記錄 Middleware（pass-through）。</summary>
    public class ValidationErrorLoggingMiddleware
    {
        private readonly Microsoft.AspNetCore.Http.RequestDelegate _next;
        public ValidationErrorLoggingMiddleware(Microsoft.AspNetCore.Http.RequestDelegate next) { _next = next; }
        public Task InvokeAsync(Microsoft.AspNetCore.Http.HttpContext context) => _next(context);
    }
}

using Med3Core.Err.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Med3Core.Err.Common.Middleware
{
    /// <summary>Shim: 全域例外 Middleware — 捕捉 AppException 回傳 ApiResponse 格式。</summary>
    public class Err_GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<Err_GlobalExceptionMiddleware> _logger;

        public Err_GlobalExceptionMiddleware(RequestDelegate next, ILogger<Err_GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AppException ex)
            {
                _logger.LogWarning(ex, "BusinessException: {Code} {Msg}", ex.ErrorCode, ex.Message);
                context.Response.StatusCode = 200;
                context.Response.ContentType = "application/json";
                var body = new { rtnCode = ex.ErrorCode, rtnMsg = ex.Message, data = (object?)null };
                await context.Response.WriteAsync(JsonSerializer.Serialize(body));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                var body = new { rtnCode = "500", rtnMsg = "Internal Server Error", data = (object?)null };
                await context.Response.WriteAsync(JsonSerializer.Serialize(body));
            }
        }
    }
}

namespace Med3Core.FNDCC.Infrastructure.Common
{
    /// <summary>Shim: 環境變數名稱常數。</summary>
    public static class EnvironmentVariableNames
    {
        public const string KafkaBroker = "kafkaBroker";
        public const string KafkaTopicAplog = "kafkaTopic_aplog";
        public const string KafkaTopicAuditlog = "kafkaTopic_auditlog";
    }
}

namespace Med3Core.FNDCC.Infrastructure.Common.Middlewares
{
    /// <summary>Shim: UTF8 表單驗證 Middleware（直接 pass-through）。</summary>
    public class Utf8FormFileValidationMiddleware
    {
        private readonly Microsoft.AspNetCore.Http.RequestDelegate _next;
        public Utf8FormFileValidationMiddleware(Microsoft.AspNetCore.Http.RequestDelegate next) { _next = next; }
        public Task InvokeAsync(Microsoft.AspNetCore.Http.HttpContext context) => _next(context);
    }
}

namespace Med3Core.FNDCC.Infrastructure.Common.Models
{
    /// <summary>Placeholder type.</summary>
    public static class InfraModelsPlaceholder { }
}

namespace Med3Core.FNDCC.Shared.Common.Extension
{
    /// <summary>Placeholder for extension methods.</summary>
    public static class SharedExtensionPlaceholder { }
}

namespace Med3Core.FNDCC.Shared.Common.Helpers
{
    /// <summary>Placeholder for helpers.</summary>
    public static class SharedHelpersPlaceholder { }
}

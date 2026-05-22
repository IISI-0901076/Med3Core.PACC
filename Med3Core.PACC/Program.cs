using Bootstrapper;
using Med3Core.Err.Common.Middleware;
using Med3Core.FNDCC.Infrastructure.Common;
using Med3Core.FNDCC.Infrastructure.Common.Middlewares;
using Med3Core.Log.Common.Logging;
using Med3Core.Log.Common.Middleware;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Web;
using System.Reflection;

namespace Med3Core.PACC
{
    ///<summary>啟動區</summary>
    public class Program
    {
        ///<summary>主程式</summary>
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            IConfigurationRoot configuration = builder.Configuration.AddEnvironmentVariables().Build();

            builder.Services.AddEndpointsApiExplorer();

            builder.AddExternalServices(configuration);

            #region Database Provider DI 切換
            string provider = configuration["Database:Provider"] ?? "Postgres";
            switch (provider.ToLowerInvariant())
            {
                case "postgres":
                    builder.Services.AddSingleton<Infrastructure.Tracing.IPostgresTracingHelper, Infrastructure.Tracing.PostgresTracingHelper>();
                    builder.Services.AddScoped<Repositories.Interfaces.IPaccRepository, Repositories.Postgres.PaccPostgresRepository>();
                    break;
                case "oracle":
                    throw new NotImplementedException("Oracle provider for PACC is not yet implemented.");
                case "mongo":
                    throw new NotImplementedException("Mongo provider for PACC is not yet implemented.");
                default:
                    throw new InvalidOperationException($"Unknown Database:Provider '{provider}'.");
            }
            builder.Services.AddScoped<Application.Interfaces.IPaccService, Application.Services.PaccService>();
            #endregion

            #region Kafka 與 Log 相關設定
            string kafkaBroker = Environment.GetEnvironmentVariable(EnvironmentVariableNames.KafkaBroker) ?? "192.168.242.59:9092";
            GlobalDiagnosticsContext.Set(EnvironmentVariableNames.KafkaBroker, kafkaBroker);
            string kafkaTopic_aplog = Environment.GetEnvironmentVariable(EnvironmentVariableNames.KafkaTopicAplog) ?? "aplog";
            GlobalDiagnosticsContext.Set(EnvironmentVariableNames.KafkaTopicAplog, kafkaTopic_aplog);
            string kafkaTopic_auditlog = Environment.GetEnvironmentVariable(EnvironmentVariableNames.KafkaTopicAuditlog) ?? "auditlog";
            GlobalDiagnosticsContext.Set(EnvironmentVariableNames.KafkaTopicAuditlog, kafkaTopic_auditlog);

            NLogConfigurationLoader.Log_LoadEmbeddedConfig();

            // 驗證並修改配置
            LoggingConfiguration? config = LogManager.Configuration;
            if (config != null)
            {
                foreach (Target? target in config.AllTargets)
                {
                    if (target is KafkaTarget kafkaTarget)
                    {
                        kafkaTarget.BootstrapServers = kafkaBroker;
                        if (kafkaTarget.Name.EndsWith("_kafka") && kafkaTarget.Name.Contains("audit"))
                        {
                            kafkaTarget.Topic = kafkaTopic_auditlog;
                        }
                        else
                        {
                            kafkaTarget.Topic = kafkaTopic_aplog;
                        }
                    }
                }
                LogManager.ReconfigExistingLoggers();
            }

            //將NLog註冊
            builder.Logging.ClearProviders();
            builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
            builder.Logging.AddConsole();
            builder.Host.UseNLog();
            #endregion

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PACC API" });
                // 讀取編譯時自動產生的 xml 註解
                string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Ex: 'Bearer {token}'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

            });
            builder.Services.AddRouting(options =>
            {
                options.LowercaseUrls = false;  // 關閉全域小寫,改由 Controller 控制
            });

            WebApplication app = builder.Build();

            //  Middleware
            app.UseMiddleware<ValidationErrorLoggingMiddleware>();
            app.UseMiddleware<Err_GlobalExceptionMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger(c =>
                {
                    c.RouteTemplate = "PACC/swagger/{documentName}/swagger.json";
                });
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/PACC/swagger/v1/swagger.json", "PACC API V1");
                    c.RoutePrefix = "PACC/swagger";
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseMiddleware<NLogMdcMiddleware>();
            app.UseAuthorization();

            app.UseMiddleware<Utf8FormFileValidationMiddleware>();

            app.MapControllers();

            // 健康檢查端點（Docker HEALTHCHECK 用）
            app.MapGet("/health", () => Results.Ok(new { status = "ok" })).AllowAnonymous();

            app.Run();
        }
    }
}

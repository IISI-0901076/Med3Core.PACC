using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bootstrapper
{
    /// <summary>Shim: AddExternalServices 擴充方法（替代 Bootstrapper.Common 的 DI 註冊）。</summary>
    public static class BootstrapperExtensions
    {
        /// <summary>本地 shim — 只註冊 Controllers + CORS。正式版由 NuGet 套件提供完整實作。</summary>
        public static WebApplicationBuilder AddExternalServices(this WebApplicationBuilder builder, IConfiguration configuration)
        {
            builder.Services.AddControllers();
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

            // Shim: 不註冊 Keycloak、不註冊 IOracleTracingHelper
            // 認證改為 dev bypass（見 DevAuthHandler）
            builder.Services.AddAuthentication("UserAuthenticationHandler")
                .AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, DevAuthHandler>(
                    "UserAuthenticationHandler", _ => { });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("default", policy => policy.RequireAssertion(_ => true));
            });

            builder.Services.AddSingleton<Med3Core.FNDCC.Infrastructure.Common.Authentication.Providers.IUserInfoFactory,
                Med3Core.FNDCC.Infrastructure.Common.Authentication.Providers.DevUserInfoFactory>();

            return builder;
        }
    }
}

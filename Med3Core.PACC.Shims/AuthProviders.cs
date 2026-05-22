using System.Security.Claims;

namespace Med3Core.FNDCC.Infrastructure.Common.Authentication.Providers
{
    /// <summary>Shim: 當前使用者資訊。</summary>
    public class CurrentUserInfo
    {
        public string UserId { get; set; } = "dev-user";
        public string UserName { get; set; } = "Dev User";
    }

    /// <summary>Shim: 使用者資訊工廠介面。</summary>
    public interface IUserInfoFactory
    {
        CurrentUserInfo? GetCurrentUserInfo(ClaimsPrincipal user);
    }

    /// <summary>Shim: 開發用工廠。</summary>
    public class DevUserInfoFactory : IUserInfoFactory
    {
        public CurrentUserInfo? GetCurrentUserInfo(ClaimsPrincipal user)
            => new CurrentUserInfo
            {
                UserId = user?.FindFirst("sub")?.Value ?? "dev-user",
                UserName = user?.Identity?.Name ?? "Dev User"
            };
    }
}

namespace Med3Core.FNDCC.Infrastructure.Common.Authentication.Providers.Intra
{
    /// <summary>Shim: Intra 使用者。</summary>
    public class IntraUserInfo : Med3Core.FNDCC.Infrastructure.Common.Authentication.Providers.CurrentUserInfo { }
}

namespace Med3Core.FNDCC.Infrastructure.Common.Authentication.Providers.VPN
{
    /// <summary>Shim: VPN 使用者。</summary>
    public class VpnUserInfo : Med3Core.FNDCC.Infrastructure.Common.Authentication.Providers.CurrentUserInfo { }
}

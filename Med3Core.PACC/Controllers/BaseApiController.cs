using Med3Core.PACC.Application.Models;
using Med3Core.Err.Common.Exceptions;
using Med3Core.FNDCC.Infrastructure.Common.Authentication.Providers;
using Med3Core.FNDCC.Infrastructure.Common.Authentication.Providers.Intra;
using Med3Core.FNDCC.Infrastructure.Common.Authentication.Providers.VPN;
using Med3Core.FNDCC.Infrastructure.Common.Models;
using Med3Core.FNDCC.Shared.Common.Extension;
using Med3Core.FNDCC.Shared.Common.Helpers;
using Med3Core.Log.Common.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Med3Core.PACC.Controllers
{
    ///<summary>API Controller 基底</summary>
    [ApiController]
    [Route("pacc1000/[controller]")]
    [Authorize(AuthenticationSchemes = "UserAuthenticationHandler", Policy = "default")]
    public abstract class BaseApiController<TController> : ControllerBase where TController : BaseApiController<TController>
    {
        #region 欄位
        /// <summary>活動追蹤</summary>
        protected readonly ActivitySource _activitySource = new ActivitySource("OpenTelemetrySource");

        /// <summary>日誌紀錄器</summary>
        private ILogger<TController>? _logger;

        /// <summary>當前使用者資訊</summary>
        private CurrentUserInfo? _userInfo;
        #endregion

        #region 屬性

        /// <summary>日誌紀錄器（用於子類別直接記錄日誌）</summary>
        protected ILogger<TController> Logger => _logger ??= HttpContext.RequestServices.GetRequiredService<ILogger<TController>>();

        /// <summary>當前使用者資訊</summary>
        /// <remarks>
        /// 請根據所屬環境調整成對應類型的 UserInfo
        /// <para>VPN => <see cref="VpnUserInfo"/></para>
        /// <para>Internet => 目前沒有</para>
        /// <para>Intra => <see cref="IntraUserInfo"/></para>
        /// </remarks>
        protected CurrentUserInfo? CurrentUser => _userInfo ??= HttpContext.RequestServices.GetRequiredService<IUserInfoFactory>().GetCurrentUserInfo(User);
        #endregion

        ///<summary>統一回傳處理</summary>
        ///<typeparam name="T">回傳類型</typeparam>
        ///<param name="result">回傳值</param>
        ///<param name="message">回傳訊息</param>
        protected IActionResult ApiResult<T>(T result, string? message = null)
        {
            if (result == null)
            {
                //  TODO: 注意！ 目前 AppException 沒辦法直接取得 Message，所以自己先定義錯誤訊息。後續需要自行調整
                return BadRequest(ApiResponse.Fail(AppExceptionHelper.NewBusinessError("001"), message!));
            }

            if (result is IActionResult actionResult) return actionResult;

            //  回傳一定要使用 ApiResponse 格式內容給前端接收。
            return result is IApiResponse ? Ok(result) : Ok(ApiResponse<T>.Ok(message!, result));
        }

        /// <summary>
        /// 通用處理
        /// </summary>
        /// <param name="func"><typeparamref name="TController"/> 執行方法，有回傳值非同步方法</param>
        /// <param name="errorMessage">發生錯誤時顯示的訊息</param>
        /// <param name="functionName"><typeparamref name="TController"/> 執行方法名稱</param>
        protected virtual async Task<IActionResult> RunAction<T>(Func<Task<T>> func, string errorMessage = "發生例外錯誤", [CallerMemberName] string functionName = "")
        {
            string activityName = string.IsNullOrEmpty(functionName)
                   ? string.Concat("API-", MethodBase.GetCurrentMethod()?.Name)
                   : string.Concat("API-", functionName);

            using Activity? activity = _activitySource.StartActivity("API-" + activityName);
            using IDisposable scope = NLogHelper.Log_PushCaseScope(functionName);

            try
            {
                T result = await NLogExtension.Log_RunWithSpendTimeScopeAsync(func, Logger, message: errorMessage, functionName: functionName);
                return ApiResult(result);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "{ErrorMessage} in {FunctionName}, ex: {Message}", errorMessage, functionName, ex.ToString());
                return ApiResult(ApiResponse.Fail(AppExceptionHelper.NewBusinessError("001"), "出現未知錯誤，請洽詢管理員。"));
            }
        }

        /// <summary>
        /// 通用處理
        /// </summary>
        /// <param name="action"><typeparamref name="TController"/> 執行方法，無回傳值非同步方法</param>
        /// <param name="functionName"><typeparamref name="TController"/> 執行方法名稱</param>
        /// <remarks><paramref name="action"/> 委派固定回傳 <see cref="IActionResult"/></remarks>
        protected Task<IActionResult> RunAction(Func<Task<IActionResult>> action, [CallerMemberName] string? functionName = null)
            => RunAction<IActionResult>(action, functionName!);

        /// <summary>
        /// 通用處理
        /// </summary>
        /// <param name="action"><typeparamref name="TController"/> 執行方法，無回傳值同步方法</param>
        /// <param name="functionName"><typeparamref name="TController"/> 執行方法名稱</param>
        /// <remarks><paramref name="action"/> 委派無回傳值</remarks>
        protected Task<IActionResult> RunAction(Func<Task> action, [CallerMemberName] string? functionName = null) => RunAction(async () =>
        {
            await action();
            return ApiResponse.Ok();
        }, functionName!);
    }
}

using Med3Core.Err.Common.Exceptions;

namespace Med3Core.PACC.Domain.Common
{
    /// <summary>業務邏輯例外 — 對應 ApiResponse 的 rtnCode / rtnMsg。</summary>
    public class BusinessException : AppException
    {
        /// <summary>建構業務例外。</summary>
        /// <param name="errorCode">錯誤代碼（如 E001、E006）。</param>
        /// <param name="message">錯誤訊息。</param>
        public BusinessException(string errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}

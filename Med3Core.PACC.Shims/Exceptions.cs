namespace Med3Core.Err.Common.Exceptions
{
    /// <summary>Shim: 應用程式例外基底。</summary>
    public class AppException : Exception
    {
        public string ErrorCode { get; set; }

        public AppException(string errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }

        public AppException(string message) : base(message)
        {
            ErrorCode = "999";
        }

        public AppException(string errorCode, string message, Exception inner) : base(message, inner)
        {
            ErrorCode = errorCode;
        }
    }

    /// <summary>Shim: AppException 輔助。</summary>
    public static class AppExceptionHelper
    {
        public static AppException NewBusinessError(string code, string message = "")
            => new AppException(code, message);
    }
}

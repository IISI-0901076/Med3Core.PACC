using Med3Core.Err.Common.Exceptions;
using System.Text.Json.Serialization;

namespace Med3Core.PACC.Application.Models
{
    ///<summary><see cref="ApiResponse"/> 的介面</summary>
    public interface IApiResponse { }

    ///<summary>回傳訊息</summary>
    ///<remarks>
    ///<para>理論上只有成功會使用 <see cref="ApiResponse"/></para>
    ///</remarks>
    public class ApiResponse<T> : IApiResponse
    {
        ///<summary>建構</summary>
        public ApiResponse() { }

        ///<summary>建構</summary>
        public ApiResponse(T? data)
        {
            Data = data;
        }

        /// <summary>錯誤代碼</summary>
        [JsonPropertyName("rtnCode")]
        public string RtnCode { get; set; } = "0";

        /// <summary>回傳訊息</summary>
        [JsonPropertyName("rtnMsg")]
        public string RtnMsg { get; set; } = string.Empty;

        /// <summary>回傳資料</summary>
        [JsonPropertyName("data")]
        public T? Data { get; set; }

        ///<summary>成功</summary>
        ///<param name="data">資料</param>
        public static ApiResponse<T> Ok(T? data = default) => new ApiResponse<T>() { Data = data };

        ///<summary>成功</summary>
        ///<param name="message">訊息</param>
        ///<param name="data">資料</param>
        public static ApiResponse<T> Ok(string message, T? data = default) => new ApiResponse<T>() { RtnMsg = message, Data = data };

        ///<summary>失敗</summary>
        ///<param name="message">訊息</param>
        ///<param name="errorCode">錯誤代號</param>
        ///<param name="data">資料</param>
        public static ApiResponse<T> Fail(string errorCode, string message, T? data = default)
            => new ApiResponse<T>() { RtnMsg = message, RtnCode = errorCode, Data = data };

        ///<summary>失敗</summary>
        ///<param name="appException">邏輯例外</param>
        ///<param name="data">資料</param>
        public static ApiResponse<T> Fail(AppException appException, T? data = default)
            => new ApiResponse<T>() { RtnMsg = appException.Message, RtnCode = appException.ErrorCode, Data = data };

        ///<summary>失敗</summary>
        ///<param name="appException">邏輯例外</param>
        ///<param name="message">錯誤訊息</param>
        ///<param name="data">資料</param>
        ///<remarks>有時候 <paramref name="appException"/> 所取得的 Message 並不是我們想要的，所以使用 <paramref name="message"/> 來覆蓋訊息</remarks>
        public static ApiResponse<T> Fail(AppException appException, string message, T? data = default)
            => new ApiResponse<T>() { RtnMsg = message, RtnCode = appException.ErrorCode, Data = data };
    }

    ///<summary>回傳訊息</summary>
    public class ApiResponse : ApiResponse<object>, IApiResponse
    {
        ///<summary>建構</summary>
        public ApiResponse() { }

        ///<summary>建構</summary>
        public ApiResponse(object? data) : base(data) { }
    }
}

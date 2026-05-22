namespace Med3Core.PACC.Application.Models.Contracts.Requests
{
    /// <summary>記錄錯誤請求（規格 §2.6）。</summary>
    public class LogErrorDto
    {
        /// <summary>案件序號。</summary>
        public string CaseSeqNo { get; set; } = string.Empty;

        /// <summary>錯誤代碼（如 E400、E448、E438、E422）。</summary>
        public string ErrCode { get; set; } = string.Empty;

        /// <summary>錯誤訊息。</summary>
        public string ProcErrMsg { get; set; } = string.Empty;

        /// <summary>錯誤發生階段 PGM=程式處理 / FILE_SEND=檔案傳送。</summary>
        public string ErrorPhase { get; set; } = string.Empty;
    }
}

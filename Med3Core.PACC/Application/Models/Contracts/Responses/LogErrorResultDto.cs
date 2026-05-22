namespace Med3Core.PACC.Application.Models.Contracts.Responses
{
    /// <summary>記錄錯誤結果（規格 §2.6）。</summary>
    public class LogErrorResultDto
    {
        /// <summary>案件序號。</summary>
        public string CaseSeqNo { get; set; } = string.Empty;

        /// <summary>錯誤代碼。</summary>
        public string ErrCode { get; set; } = string.Empty;

        /// <summary>錯誤階段。</summary>
        public string ErrorPhase { get; set; } = string.Empty;
    }
}

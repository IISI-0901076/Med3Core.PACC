namespace Med3Core.PACC.Application.Models.Contracts.Requests
{
    /// <summary>更新案件狀態請求（規格 §2.3）。</summary>
    public class UpdateCaseStatusDto
    {
        /// <summary>案件序號（必填）。</summary>
        public string CaseSeqNo { get; set; } = string.Empty;

        /// <summary>案件狀態 0→1→2→F。</summary>
        public string? CaseStatus { get; set; }

        /// <summary>程式處理狀態 0→9→1/2。</summary>
        public string? PgmProcStatus { get; set; }

        /// <summary>檔案傳送狀態 0→9→1/2/3。</summary>
        public string? FileSendStatus { get; set; }

        /// <summary>處理位置（S3 bucket/prefix）。</summary>
        public string? ProcPos { get; set; }
    }
}

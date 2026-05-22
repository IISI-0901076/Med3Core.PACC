namespace Med3Core.PACC.Application.Models.Contracts.Responses
{
    /// <summary>案件狀態回應（規格 §2.1 / §2.2 / §2.3 共用）。</summary>
    public class CaseStatusResultDto
    {
        /// <summary>案件序號。</summary>
        public string CaseSeqNo { get; set; } = string.Empty;

        /// <summary>業務代碼。</summary>
        public string? BusCode { get; set; }

        /// <summary>醫事機構代號。</summary>
        public string? HospId { get; set; }

        /// <summary>分區代碼。</summary>
        public string? BranchCode { get; set; }

        /// <summary>案件狀態。</summary>
        public string? CaseStatus { get; set; }

        /// <summary>程式處理狀態。</summary>
        public string? PgmProcStatus { get; set; }

        /// <summary>檔案傳送狀態。</summary>
        public string? FileSendStatus { get; set; }

        /// <summary>收件序號。</summary>
        public string? RecvSeqNo { get; set; }

        /// <summary>讀取位置。</summary>
        public string? ReadPos { get; set; }

        /// <summary>建立時間。</summary>
        public DateTime? AddTime { get; set; }

        /// <summary>程式處理開始時間。</summary>
        public DateTime? PgmTimeS { get; set; }

        /// <summary>程式處理結束時間。</summary>
        public DateTime? PgmTimeE { get; set; }

        /// <summary>檔案傳送開始時間。</summary>
        public DateTime? FileTimeS { get; set; }

        /// <summary>檔案傳送結束時間。</summary>
        public DateTime? FileTimeE { get; set; }

        /// <summary>更新時間（UpdateCaseStatus 回應用）。</summary>
        public DateTime? UpdatedAt { get; set; }
    }
}

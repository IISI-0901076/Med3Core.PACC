namespace Med3Core.PACC.Application.Models.Contracts.Requests
{
    /// <summary>查詢案件狀態請求（規格 §2.2）。</summary>
    public class QueryStatusDto
    {
        /// <summary>精確查詢（指定案件）。</summary>
        public string? CaseSeqNo { get; set; }

        /// <summary>模組篩選。</summary>
        public string? BusCode { get; set; }

        /// <summary>醫事機構代號篩選。</summary>
        public string? HospId { get; set; }

        /// <summary>案件狀態篩選 0/1/2/F。</summary>
        public string? CaseStatus { get; set; }

        /// <summary>程式處理狀態篩選 0/9/1/2。</summary>
        public string? PgmProcStatus { get; set; }

        /// <summary>檔案傳送狀態篩選 0/9/1/2/3（支援逗號分隔多值如 "0,3"）。</summary>
        public string? FileSendStatus { get; set; }

        /// <summary>天數回溯（預設 30）。</summary>
        public int? DaysBack { get; set; }

        /// <summary>頁碼（從 1 開始，預設 1）。</summary>
        public int? PageNo { get; set; }

        /// <summary>每頁筆數（預設 100，上限 1000）。</summary>
        public int? PageSize { get; set; }
    }
}

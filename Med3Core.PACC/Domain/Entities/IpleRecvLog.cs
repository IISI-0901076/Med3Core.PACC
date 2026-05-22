namespace Med3Core.PACC.Domain.Entities
{
    /// <summary>收件記錄表 IPLE_RECV_LOG。</summary>
    public class IpleRecvLog
    {
        /// <summary>收件序號（PK）。</summary>
        public string RecvSeqNo { get; set; } = string.Empty;

        /// <summary>業務代碼。</summary>
        public string BusCode { get; set; } = string.Empty;

        /// <summary>醫事機構代號。</summary>
        public string HospId { get; set; } = string.Empty;

        /// <summary>分區代碼。</summary>
        public string? BranchCode { get; set; }

        /// <summary>檔案類型 XML|FHR|DCF|DICOM。</summary>
        public string FileType { get; set; } = string.Empty;

        /// <summary>收件方式 2=VPN, 3=API。</summary>
        public string RecvMode { get; set; } = string.Empty;

        /// <summary>原始檔名。</summary>
        public string OrigFileName { get; set; } = string.Empty;

        /// <summary>檔案大小。</summary>
        public decimal? FileSize { get; set; }

        /// <summary>S3 Object Key。</summary>
        public string? FileObjectKey { get; set; }

        /// <summary>SAM ID。</summary>
        public string? SamId { get; set; }

        /// <summary>Job ID。</summary>
        public string? JobId { get; set; }

        /// <summary>PC Code。</summary>
        public string? PcCode { get; set; }

        /// <summary>讀取位置。</summary>
        public string? ReadPos { get; set; }

        /// <summary>上傳使用者。</summary>
        public string? UploadUserId { get; set; }

        /// <summary>處理結果 1=成功, 2=失敗。</summary>
        public string? ProcResult { get; set; }

        /// <summary>錯誤訊息。</summary>
        public string? ProcErrMsg { get; set; }

        /// <summary>處理開始時間。</summary>
        public DateTime? ProcSTime { get; set; }

        /// <summary>處理結束時間。</summary>
        public DateTime? ProcETime { get; set; }

        /// <summary>XML 案件序號。</summary>
        public string? XmlCaseseqno { get; set; }

        /// <summary>XML 處理狀態。</summary>
        public string? XmlProcStatus { get; set; }

        /// <summary>建立時間。</summary>
        public DateTime? AddTime { get; set; }

        /// <summary>處理時間。</summary>
        public DateTime? ProcTime { get; set; }
    }
}

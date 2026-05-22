namespace Med3Core.PACC.Domain.Entities
{
    /// <summary>案件處理記錄表 IPLT_FILE_PROC_LOG。</summary>
    public class IpltFileProcLog
    {
        /// <summary>案件序號（PK）。</summary>
        public string CaseSeqNo { get; set; } = string.Empty;

        /// <summary>業務代碼 IAV|IPL|QBJ|QB1~6|REA|RWM|RCP。</summary>
        public string BusCode { get; set; } = string.Empty;

        /// <summary>醫事機構代號。</summary>
        public string HospId { get; set; } = string.Empty;

        /// <summary>分區代碼。</summary>
        public string? BranchCode { get; set; }

        /// <summary>案件狀態 0/1/2/F。</summary>
        public string CaseStatus { get; set; } = "0";

        /// <summary>程式處理狀態 0/9/1/2。</summary>
        public string PgmProcStatus { get; set; } = "0";

        /// <summary>檔案傳送狀態 0/9/1/2/3。</summary>
        public string FileSendStatus { get; set; } = "0";

        /// <summary>對應收件序號。</summary>
        public string? RecvSeqNo { get; set; }

        /// <summary>S3 bucket 區分 1=nhi-ie1, 2=nhi-ie3。</summary>
        public string? ReadPos { get; set; }

        /// <summary>處理位置（S3 Object Key）。</summary>
        public string? ProcPos { get; set; }

        /// <summary>錯誤代碼。</summary>
        public string? ErrCode { get; set; }

        /// <summary>錯誤訊息。</summary>
        public string? ProcErrMsg { get; set; }

        /// <summary>HL7 標記。</summary>
        public string? Hl7 { get; set; }

        /// <summary>上傳格式。</summary>
        public string? UploadFormat { get; set; }

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
    }
}

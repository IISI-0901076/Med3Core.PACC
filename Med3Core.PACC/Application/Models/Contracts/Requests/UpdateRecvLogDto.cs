namespace Med3Core.PACC.Application.Models.Contracts.Requests
{
    /// <summary>更新收件記錄請求（規格 §2.8）。</summary>
    public class UpdateRecvLogDto
    {
        /// <summary>收件序號（必填，WHERE 條件）。</summary>
        public string RecvSeqNo { get; set; } = string.Empty;

        /// <summary>更新讀取位置。</summary>
        public string? ReadPos { get; set; }

        /// <summary>上傳使用者。</summary>
        public string? UploadUserId { get; set; }

        /// <summary>檔案大小。</summary>
        public decimal? FileSize { get; set; }

        /// <summary>處理結果 1=成功, 2=失敗。</summary>
        public string? ProcResult { get; set; }

        /// <summary>錯誤訊息。</summary>
        public string? ProcErrMsg { get; set; }

        /// <summary>處理開始時間（yyyy-MM-dd HH:mm:ss）。</summary>
        public string? ProcSTime { get; set; }

        /// <summary>處理結束時間（yyyy-MM-dd HH:mm:ss）。</summary>
        public string? ProcETime { get; set; }

        /// <summary>XML 案件序號。</summary>
        public string? XmlCaseseqno { get; set; }

        /// <summary>XML 處理狀態。</summary>
        public string? XmlProcStatus { get; set; }

        /// <summary>S3 Object Key。</summary>
        public string? FileObjectKey { get; set; }

        /// <summary>原始檔名。</summary>
        public string? OrigFileName { get; set; }

        /// <summary>SAM ID。</summary>
        public string? SamId { get; set; }

        /// <summary>Job ID。</summary>
        public string? JobId { get; set; }

        /// <summary>PC Code。</summary>
        public string? PcCode { get; set; }
    }
}

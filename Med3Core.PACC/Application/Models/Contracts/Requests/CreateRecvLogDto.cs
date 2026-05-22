namespace Med3Core.PACC.Application.Models.Contracts.Requests
{
    /// <summary>新增收件記錄請求（規格 §2.7）。</summary>
    public class CreateRecvLogDto
    {
        /// <summary>收件序號 (PK)。</summary>
        public string RecvSeqNo { get; set; } = string.Empty;

        /// <summary>業務代碼 IPL|QBJ|IAV。</summary>
        public string BusCode { get; set; } = string.Empty;

        /// <summary>醫事機構代號。</summary>
        public string HospId { get; set; } = string.Empty;

        /// <summary>分區代碼。</summary>
        public string? BranchCode { get; set; }

        /// <summary>檔案類型 XML|FHR|DCF|DICOM。</summary>
        public string FileType { get; set; } = string.Empty;

        /// <summary>收件方式 2=VPN, 3=API 直傳。</summary>
        public string RecvMode { get; set; } = string.Empty;

        /// <summary>原始檔名。</summary>
        public string OrigFileName { get; set; } = string.Empty;

        /// <summary>檔案大小。</summary>
        public decimal FileSize { get; set; }

        /// <summary>S3 Object Key。</summary>
        public string FileObjectKey { get; set; } = string.Empty;

        /// <summary>SAM ID。</summary>
        public string? SamId { get; set; }

        /// <summary>Job ID。</summary>
        public string? JobId { get; set; }

        /// <summary>PC Code。</summary>
        public string? PcCode { get; set; }

        /// <summary>對應 S3 bucket (1=nhi-ie1, 2=nhi-ie3)。</summary>
        public string ReadPos { get; set; } = string.Empty;
    }
}

namespace Med3Core.PACC.Application.Models.Contracts.Requests
{
    /// <summary>新增附件記錄請求（規格 §2.10）。</summary>
    public class CreateAttFileDto
    {
        /// <summary>案件序號。</summary>
        public string CaseSeqNo { get; set; } = string.Empty;

        /// <summary>附件序號。</summary>
        public decimal ItemNo { get; set; }

        /// <summary>檔名。</summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>檔案類型 DICOM|XML|FHR|DCF|MPG。</summary>
        public string FileType { get; set; } = string.Empty;

        /// <summary>S3 Object Key。</summary>
        public string FileObjectKey { get; set; } = string.Empty;

        /// <summary>備註。</summary>
        public string? FileMemo { get; set; }

        /// <summary>原始檔名。</summary>
        public string? OrigFileName { get; set; }

        /// <summary>附件分類 DCF|MPG|...。</summary>
        public string? FileAttach { get; set; }

        /// <summary>文件類型。</summary>
        public string? DocumentType { get; set; }
    }
}

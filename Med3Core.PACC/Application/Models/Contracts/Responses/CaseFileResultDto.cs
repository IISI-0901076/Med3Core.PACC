namespace Med3Core.PACC.Application.Models.Contracts.Responses
{
    /// <summary>附件清單單筆回應（規格 §2.5）。</summary>
    public class CaseFileResultDto
    {
        /// <summary>案件序號。</summary>
        public string CaseSeqNo { get; set; } = string.Empty;

        /// <summary>附件序號。</summary>
        public decimal ItemNo { get; set; }

        /// <summary>檔名。</summary>
        public string? FileName { get; set; }

        /// <summary>檔案類型。</summary>
        public string? FileType { get; set; }

        /// <summary>S3 Object Key。</summary>
        public string? FileObjectKey { get; set; }

        /// <summary>備註。</summary>
        public string? FileMemo { get; set; }

        /// <summary>HL7 標記。</summary>
        public string? Hl7 { get; set; }

        /// <summary>原始檔名。</summary>
        public string? OrigFileName { get; set; }

        /// <summary>文件類型。</summary>
        public string? DocumentType { get; set; }
    }
}

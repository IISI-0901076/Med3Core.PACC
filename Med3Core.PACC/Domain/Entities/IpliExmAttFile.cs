namespace Med3Core.PACC.Domain.Entities
{
    /// <summary>附件本地工作副本表 IPLI_EXM_ATT_FILE（與遠端 NHI_IDC.IPLE_RECV_ATT_FILE 同欄位）。</summary>
    public class IpliExmAttFile
    {
        /// <summary>案件序號（PK 第 1 欄）。</summary>
        public string CaseSeqNo { get; set; } = string.Empty;

        /// <summary>附件序號（PK 第 2 欄）。</summary>
        public decimal ItemNo { get; set; }

        /// <summary>檔名。</summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>檔案類型 DICOM|XML|FHR|DCF|MPG。</summary>
        public string FileType { get; set; } = string.Empty;

        /// <summary>S3 Object Key。</summary>
        public string? FileObjectKey { get; set; }

        /// <summary>備註。</summary>
        public string? FileMemo { get; set; }

        /// <summary>HL7 標記。</summary>
        public string? Hl7 { get; set; }

        /// <summary>原始檔名。</summary>
        public string? OrigFileName { get; set; }

        /// <summary>附件分類 DCF|MPG|...。</summary>
        public string? FileAttach { get; set; }

        /// <summary>文件類型。</summary>
        public string? DocumentType { get; set; }
    }
}

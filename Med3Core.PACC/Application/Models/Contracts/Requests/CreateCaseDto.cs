namespace Med3Core.PACC.Application.Models.Contracts.Requests
{
    /// <summary>新增案件請求（規格 §2.1）。</summary>
    public class CreateCaseDto
    {
        /// <summary>案件序號 (PK)。</summary>
        public string CaseSeqNo { get; set; } = string.Empty;

        /// <summary>業務代碼 IAV|IPL|QBJ|QB1~6|REA|RWM|RCP。</summary>
        public string BusCode { get; set; } = string.Empty;

        /// <summary>醫事機構代號。</summary>
        public string HospId { get; set; } = string.Empty;

        /// <summary>分區代碼。</summary>
        public string? BranchCode { get; set; }

        /// <summary>初始狀態 0=未處理, 1=已處理。</summary>
        public string CaseStatus { get; set; } = "0";

        /// <summary>程式處理狀態。</summary>
        public string? PgmProcStatus { get; set; }

        /// <summary>對應收件序號。</summary>
        public string? RecvSeqNo { get; set; }

        /// <summary>S3 bucket 區分 1=nhi-ie1, 2=nhi-ie3。</summary>
        public string? ReadPos { get; set; }

        /// <summary>HL7 標記。</summary>
        public string? Hl7 { get; set; }

        /// <summary>上傳格式。</summary>
        public string? UploadFormat { get; set; }
    }
}

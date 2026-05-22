namespace Med3Core.PACC.Application.Models.Contracts.Responses
{
    /// <summary>新增附件記錄結果（規格 §2.10）。</summary>
    public class CreateAttFileResultDto
    {
        /// <summary>案件序號。</summary>
        public string CaseSeqNo { get; set; } = string.Empty;

        /// <summary>附件序號。</summary>
        public decimal ItemNo { get; set; }

        /// <summary>S3 Object Key。</summary>
        public string FileObjectKey { get; set; } = string.Empty;
    }
}

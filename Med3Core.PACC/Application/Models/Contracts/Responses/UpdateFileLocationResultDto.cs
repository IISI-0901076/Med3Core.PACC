namespace Med3Core.PACC.Application.Models.Contracts.Responses
{
    /// <summary>更新檔案位置結果（規格 §2.9）。</summary>
    public class UpdateFileLocationResultDto
    {
        /// <summary>案件序號。</summary>
        public string CaseSeqNo { get; set; } = string.Empty;

        /// <summary>S3 Object Key。</summary>
        public string FileObjectKey { get; set; } = string.Empty;
    }
}

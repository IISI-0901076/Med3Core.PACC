namespace Med3Core.PACC.Application.Models.Contracts.Requests
{
    /// <summary>更新檔案 S3 位置請求（規格 §2.9）。</summary>
    public class UpdateFileLocationDto
    {
        /// <summary>案件序號。</summary>
        public string CaseSeqNo { get; set; } = string.Empty;

        /// <summary>S3 Object Key（新位置）。</summary>
        public string FileObjectKey { get; set; } = string.Empty;
    }
}

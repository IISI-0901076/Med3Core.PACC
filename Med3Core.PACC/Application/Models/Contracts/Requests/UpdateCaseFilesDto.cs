namespace Med3Core.PACC.Application.Models.Contracts.Requests
{
    /// <summary>以 fileObjectKey 同步附件請求（規格 §2.4）。</summary>
    public class UpdateCaseFilesDto
    {
        /// <summary>案件序號。</summary>
        public string CaseSeqNo { get; set; } = string.Empty;

        /// <summary>S3 Object Key（格式: {busCode}/{hospId}/{caseSeqNo}/{fileName}）。</summary>
        public string FileObjectKey { get; set; } = string.Empty;
    }
}

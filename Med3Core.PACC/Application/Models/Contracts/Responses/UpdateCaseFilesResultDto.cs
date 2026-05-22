namespace Med3Core.PACC.Application.Models.Contracts.Responses
{
    /// <summary>同步附件結果（規格 §2.4）。</summary>
    public class UpdateCaseFilesResultDto
    {
        /// <summary>案件序號。</summary>
        public string CaseSeqNo { get; set; } = string.Empty;

        /// <summary>S3 Object Key。</summary>
        public string FileObjectKey { get; set; } = string.Empty;

        /// <summary>是否成功同步至本地工作副本表。</summary>
        public bool SyncedToLocal { get; set; }
    }
}

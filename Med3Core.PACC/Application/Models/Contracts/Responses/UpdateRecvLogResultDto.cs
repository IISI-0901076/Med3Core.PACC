namespace Med3Core.PACC.Application.Models.Contracts.Responses
{
    /// <summary>更新收件記錄結果（規格 §2.8）。</summary>
    public class UpdateRecvLogResultDto
    {
        /// <summary>收件序號。</summary>
        public string RecvSeqNo { get; set; } = string.Empty;

        /// <summary>處理結果。</summary>
        public string? ProcResult { get; set; }
    }
}

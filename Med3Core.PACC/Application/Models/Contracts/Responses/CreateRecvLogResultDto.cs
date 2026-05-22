namespace Med3Core.PACC.Application.Models.Contracts.Responses
{
    /// <summary>新增收件記錄結果（規格 §2.7）。</summary>
    public class CreateRecvLogResultDto
    {
        /// <summary>收件序號。</summary>
        public string RecvSeqNo { get; set; } = string.Empty;

        /// <summary>建立時間。</summary>
        public DateTime? AddTime { get; set; }
    }
}

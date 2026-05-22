namespace Med3Core.PACC.Application.Models.Contracts.Requests
{
    /// <summary>查詢附件清單請求（規格 §2.5）。</summary>
    public class QueryCaseFilesRequestDto
    {
        /// <summary>案件序號（必填）。</summary>
        public string CaseSeqNo { get; set; } = string.Empty;

        /// <summary>篩選附件類型 DCF|MPG|FHR|XML|DICOM。</summary>
        public string? FileAttach { get; set; }
    }
}

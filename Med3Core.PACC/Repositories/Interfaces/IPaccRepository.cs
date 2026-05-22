using Med3Core.PACC.Application.Models.Contracts.Requests;
using Med3Core.PACC.Domain.Entities;

namespace Med3Core.PACC.Repositories.Interfaces
{
    /// <summary>PACC Repository 介面（規格 §5）。Postgres/Oracle/Mongo 各自實作。</summary>
    public interface IPaccRepository
    {
        // ---- IPLT_FILE_PROC_LOG ----
        /// <summary>新增案件處理記錄。</summary>
        Task<int> InsertFileProcLogAsync(IpltFileProcLog entity);

        /// <summary>分頁查詢案件處理記錄。</summary>
        Task<List<IpltFileProcLog>> QueryFileProcLogAsync(QueryStatusDto filter);

        /// <summary>查詢符合條件的總筆數。</summary>
        Task<int> QueryFileProcLogCountAsync(QueryStatusDto filter);

        /// <summary>依案件序號取得單筆。</summary>
        Task<IpltFileProcLog?> GetFileProcLogBySeqNoAsync(string caseSeqNo);

        /// <summary>更新案件狀態（樂觀鎖：WHERE 帶舊 CASE_STATUS）。</summary>
        Task<int> UpdateFileProcLogStatusAsync(string caseSeqNo, string? caseStatus,
            string? pgmProcStatus, string? fileSendStatus, string? procPos,
            string oldCaseStatus);

        /// <summary>記錄錯誤並依 ErrorPhase 設定對應狀態為 2。</summary>
        Task<int> UpdateFileProcLogErrorAsync(string caseSeqNo, string errCode,
            string procErrMsg, string errorPhase);

        /// <summary>更新檔案位置（PROC_POS）。</summary>
        Task<int> UpdateFileProcLogLocationAsync(string caseSeqNo, string fileObjectKey);

        // ---- IPLE_RECV_LOG ----
        /// <summary>新增收件記錄。</summary>
        Task<int> InsertRecvLogAsync(IpleRecvLog entity);

        /// <summary>更新收件記錄。</summary>
        Task<int> UpdateRecvLogAsync(UpdateRecvLogDto dto);

        /// <summary>依收件序號取得單筆。</summary>
        Task<IpleRecvLog?> GetRecvLogBySeqNoAsync(string recvSeqNo);

        // ---- IPLE_RECV_ATT_FILE & IPLI_EXM_ATT_FILE ----
        /// <summary>查詢附件清單。</summary>
        Task<List<IpliExmAttFile>> QueryAttFilesBySeqNoAsync(string caseSeqNo, string? fileAttach);

        /// <summary>由遠端 nhi_idc.IPLE_RECV_ATT_FILE 同步至本地 IPLI_EXM_ATT_FILE。</summary>
        Task<int> SyncAttFilesFromRemoteAsync(string caseSeqNo);

        /// <summary>新增附件（UPSERT，雙寫遠端與本地）。</summary>
        Task<int> InsertAttFileAsync(IpliExmAttFile entity);
    }
}

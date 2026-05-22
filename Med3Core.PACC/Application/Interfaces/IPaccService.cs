using Med3Core.PACC.Application.Models.Contracts.Requests;
using Med3Core.PACC.Application.Models.Contracts.Responses;

namespace Med3Core.PACC.Application.Interfaces
{
    /// <summary>PACC Service 介面（規格 §4）。</summary>
    public interface IPaccService
    {
        /// <summary>新增案件。</summary>
        Task<CaseStatusResultDto> CreateCaseAsync(CreateCaseDto dto);

        /// <summary>依條件查詢案件狀態。</summary>
        Task<PagedResult<CaseStatusResultDto>> QueryStatusAsync(QueryStatusDto dto);

        /// <summary>更新案件狀態（含狀態機驗證）。</summary>
        Task<CaseStatusResultDto> UpdateCaseStatusAsync(UpdateCaseStatusDto dto);

        /// <summary>以 fileObjectKey 同步附件狀態。</summary>
        Task<UpdateCaseFilesResultDto> UpdateCaseFilesAsync(UpdateCaseFilesDto dto);

        /// <summary>查詢附件清單。</summary>
        Task<List<CaseFileResultDto>> QueryCaseFilesAsync(QueryCaseFilesRequestDto dto);

        /// <summary>記錄錯誤。</summary>
        Task<LogErrorResultDto> LogErrorAsync(LogErrorDto dto);

        /// <summary>新增收件記錄。</summary>
        Task<CreateRecvLogResultDto> CreateRecvLogAsync(CreateRecvLogDto dto);

        /// <summary>更新收件記錄。</summary>
        Task<UpdateRecvLogResultDto> UpdateRecvLogAsync(UpdateRecvLogDto dto);

        /// <summary>更新檔案 S3 位置。</summary>
        Task<UpdateFileLocationResultDto> UpdateFileLocationAsync(UpdateFileLocationDto dto);

        /// <summary>新增附件記錄。</summary>
        Task<CreateAttFileResultDto> CreateAttFileAsync(CreateAttFileDto dto);
    }
}

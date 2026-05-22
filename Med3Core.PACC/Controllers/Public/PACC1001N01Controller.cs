using Med3Core.PACC.Application.Interfaces;
using Med3Core.PACC.Application.Models.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Med3Core.PACC.Controllers.Public
{
    /// <summary>PACC 案件預核處理 API Controller（10 端點）。</summary>
    public class PACC1001N01Controller : BaseApiController<PACC1001N01Controller>
    {
        private readonly IPaccService _service;

        /// <summary>建構子。</summary>
        public PACC1001N01Controller(IPaccService service)
        {
            _service = service;
        }

        /// <summary>新增案件。</summary>
        [HttpPost(nameof(CreateCase))]
        public Task<IActionResult> CreateCase([FromBody] CreateCaseDto dto) =>
            RunAction(async () => await _service.CreateCaseAsync(dto));

        /// <summary>查詢案件狀態。</summary>
        [HttpPost(nameof(QueryStatus))]
        public Task<IActionResult> QueryStatus([FromBody] QueryStatusDto dto) =>
            RunAction(async () => await _service.QueryStatusAsync(dto));

        /// <summary>更新案件狀態。</summary>
        [HttpPost(nameof(UpdateCaseStatus))]
        public Task<IActionResult> UpdateCaseStatus([FromBody] UpdateCaseStatusDto dto) =>
            RunAction(async () => await _service.UpdateCaseStatusAsync(dto));

        /// <summary>以 S3 Object Key 同步附件。</summary>
        [HttpPost(nameof(UpdateCaseFiles))]
        public Task<IActionResult> UpdateCaseFiles([FromBody] UpdateCaseFilesDto dto) =>
            RunAction(async () => await _service.UpdateCaseFilesAsync(dto));

        /// <summary>查詢附件清單。</summary>
        [HttpPost(nameof(QueryCaseFiles))]
        public Task<IActionResult> QueryCaseFiles([FromBody] QueryCaseFilesRequestDto dto) =>
            RunAction(async () => await _service.QueryCaseFilesAsync(dto));

        /// <summary>記錄錯誤。</summary>
        [HttpPost(nameof(LogError))]
        public Task<IActionResult> LogError([FromBody] LogErrorDto dto) =>
            RunAction(async () => await _service.LogErrorAsync(dto));

        /// <summary>新增收件記錄。</summary>
        [HttpPost(nameof(CreateRecvLog))]
        public Task<IActionResult> CreateRecvLog([FromBody] CreateRecvLogDto dto) =>
            RunAction(async () => await _service.CreateRecvLogAsync(dto));

        /// <summary>更新收件記錄。</summary>
        [HttpPost(nameof(UpdateRecvLog))]
        public Task<IActionResult> UpdateRecvLog([FromBody] UpdateRecvLogDto dto) =>
            RunAction(async () => await _service.UpdateRecvLogAsync(dto));

        /// <summary>更新檔案 S3 位置。</summary>
        [HttpPost(nameof(UpdateFileLocation))]
        public Task<IActionResult> UpdateFileLocation([FromBody] UpdateFileLocationDto dto) =>
            RunAction(async () => await _service.UpdateFileLocationAsync(dto));

        /// <summary>新增附件記錄（UPSERT 雙寫）。</summary>
        [HttpPost(nameof(CreateAttFile))]
        public Task<IActionResult> CreateAttFile([FromBody] CreateAttFileDto dto) =>
            RunAction(async () => await _service.CreateAttFileAsync(dto));
    }
}

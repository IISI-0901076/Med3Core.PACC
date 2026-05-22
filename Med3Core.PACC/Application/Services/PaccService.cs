using Med3Core.Log.Common.Logging;
using Med3Core.PACC.Application.Interfaces;
using Med3Core.PACC.Application.Models.Contracts.Requests;
using Med3Core.PACC.Application.Models.Contracts.Responses;
using Med3Core.PACC.Application.StateMachine;
using Med3Core.PACC.Domain.Common;
using Med3Core.PACC.Domain.Entities;
using Med3Core.PACC.Repositories.Interfaces;
using NLog;
using System.Diagnostics;
using System.Reflection;

namespace Med3Core.PACC.Application.Services
{
    /// <summary>PACC Service 實作。10 個方法各自被 TraceHelper 包覆。</summary>
    public class PaccService : IPaccService
    {
        private readonly Logger _nlogger = LogManager.GetCurrentClassLogger();
        private readonly ActivitySource _activitySource = new ActivitySource("OpenTelemetrySource");
        private readonly IPaccRepository _repo;

        /// <summary>建構子。</summary>
        /// <param name="repo">PACC Repository。</param>
        public PaccService(IPaccRepository repo)
        {
            _repo = repo;
        }

        /// <inheritdoc/>
        public Task<CaseStatusResultDto> CreateCaseAsync(CreateCaseDto dto) =>
            TraceHelper.Log_RunWithTracingAndLogAsync(MethodBase.GetCurrentMethod()!.Name, _nlogger, _activitySource, async () =>
            {
                IpltFileProcLog? exists = await _repo.GetFileProcLogBySeqNoAsync(dto.CaseSeqNo);
                if (exists is not null)
                {
                    throw new BusinessException("E006", $"重複建案：CASE_SEQ_NO '{dto.CaseSeqNo}' 已存在");
                }

                IpltFileProcLog entity = new()
                {
                    CaseSeqNo = dto.CaseSeqNo,
                    BusCode = dto.BusCode,
                    HospId = dto.HospId,
                    BranchCode = dto.BranchCode,
                    CaseStatus = dto.CaseStatus,
                    PgmProcStatus = dto.PgmProcStatus ?? "0",
                    FileSendStatus = "0",
                    RecvSeqNo = dto.RecvSeqNo,
                    ReadPos = dto.ReadPos,
                    Hl7 = dto.Hl7,
                    UploadFormat = dto.UploadFormat,
                };

                await _repo.InsertFileProcLogAsync(entity);
                IpltFileProcLog inserted = (await _repo.GetFileProcLogBySeqNoAsync(dto.CaseSeqNo))!;

                return MapToResult(inserted);
            });

        /// <inheritdoc/>
        public Task<PagedResult<CaseStatusResultDto>> QueryStatusAsync(QueryStatusDto dto) =>
            TraceHelper.Log_RunWithTracingAndLogAsync(MethodBase.GetCurrentMethod()!.Name, _nlogger, _activitySource, async () =>
            {
                int total = await _repo.QueryFileProcLogCountAsync(dto);
                List<IpltFileProcLog> items = await _repo.QueryFileProcLogAsync(dto);

                return new PagedResult<CaseStatusResultDto>
                {
                    TotalCount = total,
                    PageNo = dto.PageNo ?? 1,
                    PageSize = dto.PageSize ?? 100,
                    Items = items.Select(MapToResult).ToList(),
                };
            });

        /// <inheritdoc/>
        public Task<CaseStatusResultDto> UpdateCaseStatusAsync(UpdateCaseStatusDto dto) =>
            TraceHelper.Log_RunWithTracingAndLogAsync(MethodBase.GetCurrentMethod()!.Name, _nlogger, _activitySource, async () =>
            {
                IpltFileProcLog current = await _repo.GetFileProcLogBySeqNoAsync(dto.CaseSeqNo)
                    ?? throw new BusinessException("E001", "案件序號不存在");

                if (dto.CaseStatus is not null && !CaseStateMachine.IsValidCaseStatusTransition(current.CaseStatus, dto.CaseStatus))
                {
                    throw new BusinessException("E003", $"CASE_STATUS 由 {current.CaseStatus} 轉 {dto.CaseStatus} 不合法");
                }
                if (dto.PgmProcStatus is not null && !CaseStateMachine.IsValidPgmProcStatusTransition(current.PgmProcStatus, dto.PgmProcStatus))
                {
                    throw new BusinessException("E003", $"PGM_PROC_STATUS 由 {current.PgmProcStatus} 轉 {dto.PgmProcStatus} 不合法");
                }
                if (dto.FileSendStatus is not null && !CaseStateMachine.IsValidFileSendStatusTransition(current.FileSendStatus, dto.FileSendStatus))
                {
                    throw new BusinessException("E003", $"FILE_SEND_STATUS 由 {current.FileSendStatus} 轉 {dto.FileSendStatus} 不合法");
                }

                int affected = await _repo.UpdateFileProcLogStatusAsync(
                    dto.CaseSeqNo, dto.CaseStatus, dto.PgmProcStatus, dto.FileSendStatus, dto.ProcPos, current.CaseStatus);

                if (affected == 0)
                {
                    throw new BusinessException("E003", "並行更新衝突（樂觀鎖），請重試");
                }

                IpltFileProcLog updated = (await _repo.GetFileProcLogBySeqNoAsync(dto.CaseSeqNo))!;
                CaseStatusResultDto result = MapToResult(updated);
                result.UpdatedAt = DateTime.UtcNow;
                return result;
            });

        /// <inheritdoc/>
        public Task<UpdateCaseFilesResultDto> UpdateCaseFilesAsync(UpdateCaseFilesDto dto) =>
            TraceHelper.Log_RunWithTracingAndLogAsync(MethodBase.GetCurrentMethod()!.Name, _nlogger, _activitySource, async () =>
            {
                _ = await _repo.GetFileProcLogBySeqNoAsync(dto.CaseSeqNo)
                    ?? throw new BusinessException("E001", "案件序號不存在");

                int synced = await _repo.SyncAttFilesFromRemoteAsync(dto.CaseSeqNo);

                return new UpdateCaseFilesResultDto
                {
                    CaseSeqNo = dto.CaseSeqNo,
                    FileObjectKey = dto.FileObjectKey,
                    SyncedToLocal = synced >= 0,
                };
            });

        /// <inheritdoc/>
        public Task<List<CaseFileResultDto>> QueryCaseFilesAsync(QueryCaseFilesRequestDto dto) =>
            TraceHelper.Log_RunWithTracingAndLogAsync(MethodBase.GetCurrentMethod()!.Name, _nlogger, _activitySource, async () =>
            {
                List<IpliExmAttFile> files = await _repo.QueryAttFilesBySeqNoAsync(dto.CaseSeqNo, dto.FileAttach);
                return files.Select(f => new CaseFileResultDto
                {
                    CaseSeqNo = f.CaseSeqNo,
                    ItemNo = f.ItemNo,
                    FileName = f.FileName,
                    FileType = f.FileType,
                    FileObjectKey = f.FileObjectKey,
                    FileMemo = f.FileMemo,
                    Hl7 = f.Hl7,
                    OrigFileName = f.OrigFileName,
                    DocumentType = f.DocumentType,
                }).ToList();
            });

        /// <inheritdoc/>
        public Task<LogErrorResultDto> LogErrorAsync(LogErrorDto dto) =>
            TraceHelper.Log_RunWithTracingAndLogAsync(MethodBase.GetCurrentMethod()!.Name, _nlogger, _activitySource, async () =>
            {
                IpltFileProcLog? current = await _repo.GetFileProcLogBySeqNoAsync(dto.CaseSeqNo)
                    ?? throw new BusinessException("E001", "案件序號不存在");

                await _repo.UpdateFileProcLogErrorAsync(dto.CaseSeqNo, dto.ErrCode, dto.ProcErrMsg, dto.ErrorPhase);

                return new LogErrorResultDto
                {
                    CaseSeqNo = dto.CaseSeqNo,
                    ErrCode = dto.ErrCode,
                    ErrorPhase = dto.ErrorPhase,
                };
            });

        /// <inheritdoc/>
        public Task<CreateRecvLogResultDto> CreateRecvLogAsync(CreateRecvLogDto dto) =>
            TraceHelper.Log_RunWithTracingAndLogAsync(MethodBase.GetCurrentMethod()!.Name, _nlogger, _activitySource, async () =>
            {
                IpleRecvLog? exists = await _repo.GetRecvLogBySeqNoAsync(dto.RecvSeqNo);
                if (exists is not null)
                {
                    throw new BusinessException("E007", $"重複建立收件記錄：RECV_SEQ_NO '{dto.RecvSeqNo}' 已存在");
                }

                IpleRecvLog entity = new()
                {
                    RecvSeqNo = dto.RecvSeqNo,
                    BusCode = dto.BusCode,
                    HospId = dto.HospId,
                    BranchCode = dto.BranchCode,
                    FileType = dto.FileType,
                    RecvMode = dto.RecvMode,
                    OrigFileName = dto.OrigFileName,
                    FileSize = dto.FileSize,
                    FileObjectKey = dto.FileObjectKey,
                    SamId = dto.SamId,
                    JobId = dto.JobId,
                    PcCode = dto.PcCode,
                    ReadPos = dto.ReadPos,
                };

                await _repo.InsertRecvLogAsync(entity);
                IpleRecvLog inserted = (await _repo.GetRecvLogBySeqNoAsync(dto.RecvSeqNo))!;

                return new CreateRecvLogResultDto
                {
                    RecvSeqNo = inserted.RecvSeqNo,
                    AddTime = inserted.AddTime,
                };
            });

        /// <inheritdoc/>
        public Task<UpdateRecvLogResultDto> UpdateRecvLogAsync(UpdateRecvLogDto dto) =>
            TraceHelper.Log_RunWithTracingAndLogAsync(MethodBase.GetCurrentMethod()!.Name, _nlogger, _activitySource, async () =>
            {
                _ = await _repo.GetRecvLogBySeqNoAsync(dto.RecvSeqNo)
                    ?? throw new BusinessException("E002", "收件序號不存在");

                await _repo.UpdateRecvLogAsync(dto);

                return new UpdateRecvLogResultDto
                {
                    RecvSeqNo = dto.RecvSeqNo,
                    ProcResult = dto.ProcResult,
                };
            });

        /// <inheritdoc/>
        public Task<UpdateFileLocationResultDto> UpdateFileLocationAsync(UpdateFileLocationDto dto) =>
            TraceHelper.Log_RunWithTracingAndLogAsync(MethodBase.GetCurrentMethod()!.Name, _nlogger, _activitySource, async () =>
            {
                _ = await _repo.GetFileProcLogBySeqNoAsync(dto.CaseSeqNo)
                    ?? throw new BusinessException("E001", "案件序號不存在");

                await _repo.UpdateFileProcLogLocationAsync(dto.CaseSeqNo, dto.FileObjectKey);

                return new UpdateFileLocationResultDto
                {
                    CaseSeqNo = dto.CaseSeqNo,
                    FileObjectKey = dto.FileObjectKey,
                };
            });

        /// <inheritdoc/>
        public Task<CreateAttFileResultDto> CreateAttFileAsync(CreateAttFileDto dto) =>
            TraceHelper.Log_RunWithTracingAndLogAsync(MethodBase.GetCurrentMethod()!.Name, _nlogger, _activitySource, async () =>
            {
                IpliExmAttFile entity = new()
                {
                    CaseSeqNo = dto.CaseSeqNo,
                    ItemNo = dto.ItemNo,
                    FileName = dto.FileName,
                    FileType = dto.FileType,
                    FileObjectKey = dto.FileObjectKey,
                    FileMemo = dto.FileMemo,
                    OrigFileName = dto.OrigFileName,
                    FileAttach = dto.FileAttach,
                    DocumentType = dto.DocumentType,
                };

                await _repo.InsertAttFileAsync(entity);

                return new CreateAttFileResultDto
                {
                    CaseSeqNo = dto.CaseSeqNo,
                    ItemNo = dto.ItemNo,
                    FileObjectKey = dto.FileObjectKey,
                };
            });

        private static CaseStatusResultDto MapToResult(IpltFileProcLog e) => new()
        {
            CaseSeqNo = e.CaseSeqNo,
            BusCode = e.BusCode,
            HospId = e.HospId,
            BranchCode = e.BranchCode,
            CaseStatus = e.CaseStatus,
            PgmProcStatus = e.PgmProcStatus,
            FileSendStatus = e.FileSendStatus,
            RecvSeqNo = e.RecvSeqNo,
            ReadPos = e.ReadPos,
            AddTime = e.AddTime,
            PgmTimeS = e.PgmTimeS,
            PgmTimeE = e.PgmTimeE,
            FileTimeS = e.FileTimeS,
            FileTimeE = e.FileTimeE,
        };
    }
}

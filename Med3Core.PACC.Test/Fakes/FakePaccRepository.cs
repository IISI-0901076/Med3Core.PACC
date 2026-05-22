using Med3Core.PACC.Application.Models.Contracts.Requests;
using Med3Core.PACC.Domain.Entities;
using Med3Core.PACC.Repositories.Interfaces;

namespace Med3Core.PACC.Test.Fakes
{
    /// <summary>記憶體假 Repository，用於 Service 單元測試。</summary>
    public class FakePaccRepository : IPaccRepository
    {
        public Dictionary<string, IpltFileProcLog> FileProcLogs { get; } = new();
        public Dictionary<string, IpleRecvLog> RecvLogs { get; } = new();
        public Dictionary<(string, decimal), IpliExmAttFile> AttFiles { get; } = new();
        public Dictionary<(string, decimal), IpliExmAttFile> RemoteAttFiles { get; } = new();

        public Task<int> InsertFileProcLogAsync(IpltFileProcLog entity)
        {
            entity.AddTime = DateTime.UtcNow;
            FileProcLogs[entity.CaseSeqNo] = entity;
            return Task.FromResult(1);
        }

        public Task<IpltFileProcLog?> GetFileProcLogBySeqNoAsync(string caseSeqNo)
        {
            FileProcLogs.TryGetValue(caseSeqNo, out var result);
            return Task.FromResult(result);
        }

        public Task<List<IpltFileProcLog>> QueryFileProcLogAsync(QueryStatusDto filter)
        {
            var query = FileProcLogs.Values.AsEnumerable();
            if (!string.IsNullOrEmpty(filter.CaseSeqNo)) query = query.Where(x => x.CaseSeqNo == filter.CaseSeqNo);
            if (!string.IsNullOrEmpty(filter.BusCode)) query = query.Where(x => x.BusCode == filter.BusCode);
            if (!string.IsNullOrEmpty(filter.HospId)) query = query.Where(x => x.HospId == filter.HospId);
            if (!string.IsNullOrEmpty(filter.CaseStatus)) query = query.Where(x => x.CaseStatus == filter.CaseStatus);

            int skip = ((filter.PageNo ?? 1) - 1) * (filter.PageSize ?? 100);
            return Task.FromResult(query.Skip(skip).Take(filter.PageSize ?? 100).ToList());
        }

        public Task<int> QueryFileProcLogCountAsync(QueryStatusDto filter)
        {
            var query = FileProcLogs.Values.AsEnumerable();
            if (!string.IsNullOrEmpty(filter.CaseSeqNo)) query = query.Where(x => x.CaseSeqNo == filter.CaseSeqNo);
            if (!string.IsNullOrEmpty(filter.BusCode)) query = query.Where(x => x.BusCode == filter.BusCode);
            return Task.FromResult(query.Count());
        }

        public Task<int> UpdateFileProcLogStatusAsync(string caseSeqNo, string? caseStatus,
            string? pgmProcStatus, string? fileSendStatus, string? procPos, string oldCaseStatus)
        {
            if (!FileProcLogs.TryGetValue(caseSeqNo, out var log)) return Task.FromResult(0);
            if (log.CaseStatus != oldCaseStatus) return Task.FromResult(0); // 樂觀鎖

            if (caseStatus is not null) log.CaseStatus = caseStatus;
            if (pgmProcStatus is not null)
            {
                log.PgmProcStatus = pgmProcStatus;
                if (pgmProcStatus == "9") log.PgmTimeS = DateTime.UtcNow;
                else if (pgmProcStatus is "1" or "2") log.PgmTimeE = DateTime.UtcNow;
            }
            if (fileSendStatus is not null)
            {
                log.FileSendStatus = fileSendStatus;
                if (fileSendStatus == "9") log.FileTimeS = DateTime.UtcNow;
                else if (fileSendStatus is "1" or "2") log.FileTimeE = DateTime.UtcNow;
            }
            if (procPos is not null) log.ProcPos = procPos;
            return Task.FromResult(1);
        }

        public Task<int> UpdateFileProcLogErrorAsync(string caseSeqNo, string errCode,
            string procErrMsg, string errorPhase)
        {
            if (!FileProcLogs.TryGetValue(caseSeqNo, out var log)) return Task.FromResult(0);
            log.ErrCode = errCode;
            log.ProcErrMsg = procErrMsg;
            if (errorPhase == "PGM") { log.PgmProcStatus = "2"; log.PgmTimeE = DateTime.UtcNow; }
            else { log.FileSendStatus = "2"; log.FileTimeE = DateTime.UtcNow; }
            return Task.FromResult(1);
        }

        public Task<int> UpdateFileProcLogLocationAsync(string caseSeqNo, string fileObjectKey)
        {
            if (!FileProcLogs.TryGetValue(caseSeqNo, out var log)) return Task.FromResult(0);
            log.ProcPos = fileObjectKey;
            return Task.FromResult(1);
        }

        public Task<int> InsertRecvLogAsync(IpleRecvLog entity)
        {
            entity.AddTime = DateTime.UtcNow;
            RecvLogs[entity.RecvSeqNo] = entity;
            return Task.FromResult(1);
        }

        public Task<IpleRecvLog?> GetRecvLogBySeqNoAsync(string recvSeqNo)
        {
            RecvLogs.TryGetValue(recvSeqNo, out var result);
            return Task.FromResult(result);
        }

        public Task<int> UpdateRecvLogAsync(UpdateRecvLogDto dto)
        {
            if (!RecvLogs.TryGetValue(dto.RecvSeqNo, out var log)) return Task.FromResult(0);
            if (dto.ReadPos is not null) log.ReadPos = dto.ReadPos;
            if (dto.ProcResult is not null) { log.ProcResult = dto.ProcResult; log.ProcTime = DateTime.UtcNow; }
            if (dto.FileObjectKey is not null) log.FileObjectKey = dto.FileObjectKey;
            if (dto.OrigFileName is not null) log.OrigFileName = dto.OrigFileName;
            if (dto.FileSize is not null) log.FileSize = dto.FileSize;
            if (dto.SamId is not null) log.SamId = dto.SamId;
            if (dto.JobId is not null) log.JobId = dto.JobId;
            if (dto.PcCode is not null) log.PcCode = dto.PcCode;
            return Task.FromResult(1);
        }

        public Task<List<IpliExmAttFile>> QueryAttFilesBySeqNoAsync(string caseSeqNo, string? fileAttach)
        {
            var result = AttFiles.Values
                .Where(f => f.CaseSeqNo == caseSeqNo)
                .Where(f => fileAttach == null || f.FileAttach == fileAttach)
                .OrderBy(f => f.ItemNo)
                .ToList();
            return Task.FromResult(result);
        }

        public Task<int> SyncAttFilesFromRemoteAsync(string caseSeqNo)
        {
            // 模擬：刪本地、從遠端複製
            var toRemove = AttFiles.Keys.Where(k => k.Item1 == caseSeqNo).ToList();
            foreach (var key in toRemove) AttFiles.Remove(key);

            var remote = RemoteAttFiles.Where(kv => kv.Key.Item1 == caseSeqNo).ToList();
            foreach (var kv in remote) AttFiles[kv.Key] = kv.Value;
            return Task.FromResult(remote.Count);
        }

        public Task<int> InsertAttFileAsync(IpliExmAttFile entity)
        {
            var key = (entity.CaseSeqNo, entity.ItemNo);
            AttFiles[key] = entity;
            RemoteAttFiles[key] = entity;
            return Task.FromResult(1);
        }
    }
}

using Dapper;
using Med3Core.PACC.Application.Models.Contracts.Requests;
using Med3Core.PACC.Domain.Entities;
using Med3Core.PACC.Infrastructure.Tracing;
using Med3Core.PACC.Repositories.Interfaces;
using Npgsql;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Med3Core.PACC.Repositories.Postgres
{
    /// <summary>PACC Repository — PostgreSQL 實作。</summary>
    public class PaccPostgresRepository : IPaccRepository
    {
        private readonly IPostgresTracingHelper _db;
        private readonly ActivitySource _activitySource = new("OpenTelemetrySource");

        /// <summary>建構子。</summary>
        public PaccPostgresRepository(IPostgresTracingHelper db)
        {
            _db = db;
        }

        #region IPLT_FILE_PROC_LOG

        /// <inheritdoc/>
        public async Task<int> InsertFileProcLogAsync(IpltFileProcLog entity)
        {
            using var _ = _activitySource.StartActivity("PostgresDB-" + MethodBase.GetCurrentMethod()!.Name);
            const string sql = @"
INSERT INTO iplt_file_proc_log
    (case_seq_no, bus_code, hosp_id, branch_code, case_status, pgm_proc_status,
     file_send_status, recv_seq_no, read_pos, hl7, upload_format, add_time)
VALUES
    (@CaseSeqNo, @BusCode, @HospId, @BranchCode, @CaseStatus, @PgmProcStatus,
     @FileSendStatus, @RecvSeqNo, @ReadPos, @Hl7, @UploadFormat, CURRENT_TIMESTAMP)";

            var parameters = new[]
            {
                new NpgsqlParameter("CaseSeqNo", entity.CaseSeqNo),
                new NpgsqlParameter("BusCode", entity.BusCode),
                new NpgsqlParameter("HospId", (object?)entity.HospId ?? DBNull.Value),
                new NpgsqlParameter("BranchCode", (object?)entity.BranchCode ?? DBNull.Value),
                new NpgsqlParameter("CaseStatus", entity.CaseStatus),
                new NpgsqlParameter("PgmProcStatus", entity.PgmProcStatus),
                new NpgsqlParameter("FileSendStatus", entity.FileSendStatus),
                new NpgsqlParameter("RecvSeqNo", (object?)entity.RecvSeqNo ?? DBNull.Value),
                new NpgsqlParameter("ReadPos", (object?)entity.ReadPos ?? DBNull.Value),
                new NpgsqlParameter("Hl7", (object?)entity.Hl7 ?? DBNull.Value),
                new NpgsqlParameter("UploadFormat", (object?)entity.UploadFormat ?? DBNull.Value),
            };

            return await _db.ExecuteNonQueryAsync(sql, parameters, NLog.LogManager.GetCurrentClassLogger());
        }

        /// <inheritdoc/>
        public async Task<IpltFileProcLog?> GetFileProcLogBySeqNoAsync(string caseSeqNo)
        {
            using var _ = _activitySource.StartActivity("PostgresDB-" + MethodBase.GetCurrentMethod()!.Name);
            const string sql = @"
SELECT case_seq_no, bus_code, hosp_id, branch_code, case_status, pgm_proc_status,
       file_send_status, recv_seq_no, read_pos, hl7, upload_format, add_time,
       pgm_time_s, pgm_time_e, file_time_s, file_time_e, proc_pos,
       err_code, proc_err_msg
FROM iplt_file_proc_log
WHERE case_seq_no = @CaseSeqNo";

            var parameters = new[] { new NpgsqlParameter("CaseSeqNo", caseSeqNo) };
            var rows = await _db.ExecuteReaderAsync(sql, parameters, NLog.LogManager.GetCurrentClassLogger());
            return MapDynamicToFileProcLog(rows.FirstOrDefault());
        }

        /// <inheritdoc/>
        public async Task<List<IpltFileProcLog>> QueryFileProcLogAsync(QueryStatusDto filter)
        {
            using var _ = _activitySource.StartActivity("PostgresDB-" + MethodBase.GetCurrentMethod()!.Name);
            var (where, parms) = BuildQueryFilter(filter);

            int pageNo = filter.PageNo ?? 1;
            int pageSize = filter.PageSize ?? 100;
            int offset = (pageNo - 1) * pageSize;

            string sql = $@"
SELECT case_seq_no, bus_code, hosp_id, branch_code, case_status, pgm_proc_status,
       file_send_status, recv_seq_no, read_pos, hl7, upload_format, add_time,
       pgm_time_s, pgm_time_e, file_time_s, file_time_e, proc_pos,
       err_code, proc_err_msg
FROM iplt_file_proc_log
{where}
ORDER BY add_time DESC
OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY";

            var rows = await _db.ExecuteReaderAsync(sql, parms, NLog.LogManager.GetCurrentClassLogger());
            return rows.Select(r => (IpltFileProcLog)MapDynamicToFileProcLog(r)!).ToList();
        }
        public async Task<int> QueryFileProcLogCountAsync(QueryStatusDto filter)
        {
            using var _ = _activitySource.StartActivity("PostgresDB-" + MethodBase.GetCurrentMethod()!.Name);
            var (where, parms) = BuildQueryFilter(filter);

            string sql = $"SELECT COUNT(*) FROM iplt_file_proc_log {where}";
            var rows = await _db.ExecuteReaderAsync(sql, parms, NLog.LogManager.GetCurrentClassLogger());
            var first = rows.FirstOrDefault();
            if (first is null) return 0;
            return Convert.ToInt32(((IDictionary<string, object>)first)["count"]);
        }

        /// <inheritdoc/>
        public async Task<int> UpdateFileProcLogStatusAsync(string caseSeqNo, string? caseStatus,
            string? pgmProcStatus, string? fileSendStatus, string? procPos, string oldCaseStatus)
        {
            using var _ = _activitySource.StartActivity("PostgresDB-" + MethodBase.GetCurrentMethod()!.Name);
            var setClauses = new List<string>();
            var parms = new List<NpgsqlParameter>();

            if (caseStatus is not null) { setClauses.Add("case_status = @CaseStatus"); parms.Add(new("CaseStatus", caseStatus)); }
            if (pgmProcStatus is not null)
            {
                setClauses.Add("pgm_proc_status = @PgmProcStatus"); parms.Add(new("PgmProcStatus", pgmProcStatus));
                if (pgmProcStatus == "9") { setClauses.Add("pgm_time_s = CURRENT_TIMESTAMP"); }
                else if (pgmProcStatus == "1" || pgmProcStatus == "2") { setClauses.Add("pgm_time_e = CURRENT_TIMESTAMP"); }
            }
            if (fileSendStatus is not null)
            {
                setClauses.Add("file_send_status = @FileSendStatus"); parms.Add(new("FileSendStatus", fileSendStatus));
                if (fileSendStatus == "9") { setClauses.Add("file_time_s = CURRENT_TIMESTAMP"); }
                else if (fileSendStatus == "1" || fileSendStatus == "2") { setClauses.Add("file_time_e = CURRENT_TIMESTAMP"); }
            }
            if (procPos is not null) { setClauses.Add("proc_pos = @ProcPos"); parms.Add(new("ProcPos", procPos)); }

            if (setClauses.Count == 0) return 0;

            parms.Add(new("CaseSeqNo", caseSeqNo));
            parms.Add(new("OldCaseStatus", oldCaseStatus));

            string sql = $@"
UPDATE iplt_file_proc_log
SET {string.Join(", ", setClauses)}
WHERE case_seq_no = @CaseSeqNo AND case_status = @OldCaseStatus";

            return await _db.ExecuteNonQueryAsync(sql, parms, NLog.LogManager.GetCurrentClassLogger());
        }

        /// <inheritdoc/>
        public async Task<int> UpdateFileProcLogErrorAsync(string caseSeqNo, string errCode,
            string procErrMsg, string errorPhase)
        {
            using var _ = _activitySource.StartActivity("PostgresDB-" + MethodBase.GetCurrentMethod()!.Name);
            string statusSet = errorPhase == "PGM"
                ? "pgm_proc_status = '2', pgm_time_e = CURRENT_TIMESTAMP"
                : "file_send_status = '2', file_time_e = CURRENT_TIMESTAMP";

            string sql = $@"
UPDATE iplt_file_proc_log
SET err_code = @ErrCode, proc_err_msg = @ProcErrMsg, {statusSet}
WHERE case_seq_no = @CaseSeqNo";

            var parms = new[]
            {
                new NpgsqlParameter("CaseSeqNo", caseSeqNo),
                new NpgsqlParameter("ErrCode", errCode),
                new NpgsqlParameter("ProcErrMsg", procErrMsg),
            };

            return await _db.ExecuteNonQueryAsync(sql, parms, NLog.LogManager.GetCurrentClassLogger());
        }

        /// <inheritdoc/>
        public async Task<int> UpdateFileProcLogLocationAsync(string caseSeqNo, string fileObjectKey)
        {
            using var _ = _activitySource.StartActivity("PostgresDB-" + MethodBase.GetCurrentMethod()!.Name);
            const string sql = @"
UPDATE iplt_file_proc_log
SET proc_pos = @FileObjectKey
WHERE case_seq_no = @CaseSeqNo";

            var parms = new[]
            {
                new NpgsqlParameter("CaseSeqNo", caseSeqNo),
                new NpgsqlParameter("FileObjectKey", fileObjectKey),
            };

            return await _db.ExecuteNonQueryAsync(sql, parms, NLog.LogManager.GetCurrentClassLogger());
        }

        #endregion

        #region IPLE_RECV_LOG

        /// <inheritdoc/>
        public async Task<int> InsertRecvLogAsync(IpleRecvLog entity)
        {
            using var _ = _activitySource.StartActivity("PostgresDB-" + MethodBase.GetCurrentMethod()!.Name);
            const string sql = @"
INSERT INTO iple_recv_log
    (recv_seq_no, bus_code, hosp_id, branch_code, file_type, recv_mode,
     orig_file_name, file_size, file_object_key, sam_id, job_id, pc_code,
     read_pos, add_time)
VALUES
    (@RecvSeqNo, @BusCode, @HospId, @BranchCode, @FileType, @RecvMode,
     @OrigFileName, @FileSize, @FileObjectKey, @SamId, @JobId, @PcCode,
     @ReadPos, CURRENT_TIMESTAMP)";

            var parms = new[]
            {
                new NpgsqlParameter("RecvSeqNo", entity.RecvSeqNo),
                new NpgsqlParameter("BusCode", entity.BusCode),
                new NpgsqlParameter("HospId", (object?)entity.HospId ?? DBNull.Value),
                new NpgsqlParameter("BranchCode", (object?)entity.BranchCode ?? DBNull.Value),
                new NpgsqlParameter("FileType", (object?)entity.FileType ?? DBNull.Value),
                new NpgsqlParameter("RecvMode", (object?)entity.RecvMode ?? DBNull.Value),
                new NpgsqlParameter("OrigFileName", (object?)entity.OrigFileName ?? DBNull.Value),
                new NpgsqlParameter("FileSize", (object?)entity.FileSize ?? DBNull.Value),
                new NpgsqlParameter("FileObjectKey", (object?)entity.FileObjectKey ?? DBNull.Value),
                new NpgsqlParameter("SamId", (object?)entity.SamId ?? DBNull.Value),
                new NpgsqlParameter("JobId", (object?)entity.JobId ?? DBNull.Value),
                new NpgsqlParameter("PcCode", (object?)entity.PcCode ?? DBNull.Value),
                new NpgsqlParameter("ReadPos", (object?)entity.ReadPos ?? DBNull.Value),
            };

            return await _db.ExecuteNonQueryAsync(sql, parms, NLog.LogManager.GetCurrentClassLogger());
        }

        /// <inheritdoc/>
        public async Task<IpleRecvLog?> GetRecvLogBySeqNoAsync(string recvSeqNo)
        {
            using var _ = _activitySource.StartActivity("PostgresDB-" + MethodBase.GetCurrentMethod()!.Name);
            const string sql = @"
SELECT recv_seq_no, bus_code, hosp_id, branch_code, file_type, recv_mode,
       orig_file_name, file_size, file_object_key, sam_id, job_id, pc_code,
       read_pos, add_time, proc_result, proc_time
FROM iple_recv_log
WHERE recv_seq_no = @RecvSeqNo";

            var parms = new[] { new NpgsqlParameter("RecvSeqNo", recvSeqNo) };
            var rows = await _db.ExecuteReaderAsync(sql, parms, NLog.LogManager.GetCurrentClassLogger());
            return MapDynamicToRecvLog(rows.FirstOrDefault());
        }

        /// <inheritdoc/>
        public async Task<int> UpdateRecvLogAsync(UpdateRecvLogDto dto)
        {
            using var _ = _activitySource.StartActivity("PostgresDB-" + MethodBase.GetCurrentMethod()!.Name);
            var setClauses = new List<string>();
            var parms = new List<NpgsqlParameter>();

            if (dto.ReadPos is not null) { setClauses.Add("read_pos = @ReadPos"); parms.Add(new("ReadPos", dto.ReadPos)); }
            if (dto.ProcResult is not null)
            {
                setClauses.Add("proc_result = @ProcResult"); parms.Add(new("ProcResult", dto.ProcResult));
                setClauses.Add("proc_time = CURRENT_TIMESTAMP");
            }
            if (dto.FileObjectKey is not null) { setClauses.Add("file_object_key = @FileObjectKey"); parms.Add(new("FileObjectKey", dto.FileObjectKey)); }
            if (dto.OrigFileName is not null) { setClauses.Add("orig_file_name = @OrigFileName"); parms.Add(new("OrigFileName", dto.OrigFileName)); }
            if (dto.FileSize is not null) { setClauses.Add("file_size = @FileSize"); parms.Add(new("FileSize", dto.FileSize)); }
            if (dto.SamId is not null) { setClauses.Add("sam_id = @SamId"); parms.Add(new("SamId", dto.SamId)); }
            if (dto.JobId is not null) { setClauses.Add("job_id = @JobId"); parms.Add(new("JobId", dto.JobId)); }
            if (dto.PcCode is not null) { setClauses.Add("pc_code = @PcCode"); parms.Add(new("PcCode", dto.PcCode)); }

            if (setClauses.Count == 0) return 0;

            parms.Add(new("RecvSeqNo", dto.RecvSeqNo));

            string sql = $"UPDATE iple_recv_log SET {string.Join(", ", setClauses)} WHERE recv_seq_no = @RecvSeqNo";
            return await _db.ExecuteNonQueryAsync(sql, parms, NLog.LogManager.GetCurrentClassLogger());
        }

        #endregion

        #region IPLE_RECV_ATT_FILE & IPLI_EXM_ATT_FILE

        /// <inheritdoc/>
        public async Task<List<IpliExmAttFile>> QueryAttFilesBySeqNoAsync(string caseSeqNo, string? fileAttach)
        {
            using var _ = _activitySource.StartActivity("PostgresDB-" + MethodBase.GetCurrentMethod()!.Name);
            var sb = new StringBuilder(@"
SELECT case_seq_no, item_no, file_name, file_type, file_object_key,
       file_memo, hl7, orig_file_name, file_attach, document_type
FROM ipli_exm_att_file
WHERE case_seq_no = @CaseSeqNo");

            var parms = new List<NpgsqlParameter> { new("CaseSeqNo", caseSeqNo) };
            if (fileAttach is not null)
            {
                sb.Append(" AND file_attach = @FileAttach");
                parms.Add(new("FileAttach", fileAttach));
            }
            sb.Append(" ORDER BY item_no");

            var rows = await _db.ExecuteReaderAsync(sb.ToString(), parms, NLog.LogManager.GetCurrentClassLogger());
            return rows.Select(r => (IpliExmAttFile)MapDynamicToAttFile(r)!).ToList();
        }

        /// <inheritdoc/>
        public async Task<int> SyncAttFilesFromRemoteAsync(string caseSeqNo)
        {
            using var _ = _activitySource.StartActivity("PostgresDB-" + MethodBase.GetCurrentMethod()!.Name);
            // 交易內先刪本地再從遠端複製
            var logger = NLog.LogManager.GetCurrentClassLogger();
            return await _db.ExecuteTransactionAsync(async (conn, tx) =>
            {
                // 刪除本地既有
                string deleteSql = "DELETE FROM ipli_exm_att_file WHERE case_seq_no = @caseSeqNo";
                await conn.ExecuteAsync(deleteSql, new { caseSeqNo }, tx);

                // 從遠端 schema 複製
                string insertSql = @"
INSERT INTO ipli_exm_att_file
    (case_seq_no, item_no, file_name, file_type, file_object_key,
     file_memo, hl7, orig_file_name, file_attach, document_type)
SELECT case_seq_no, item_no, file_name, file_type, file_object_key,
       file_memo, hl7, orig_file_name, file_attach, document_type
FROM nhi_idc.iple_recv_att_file
WHERE case_seq_no = @caseSeqNo";
                return await conn.ExecuteAsync(insertSql, new { caseSeqNo }, tx);
            }, logger);
        }

        /// <inheritdoc/>
        public async Task<int> InsertAttFileAsync(IpliExmAttFile entity)
        {
            using var _ = _activitySource.StartActivity("PostgresDB-" + MethodBase.GetCurrentMethod()!.Name);
            var logger = NLog.LogManager.GetCurrentClassLogger();
            // UPSERT 雙寫：本地 + 遠端
            return await _db.ExecuteTransactionAsync(async (conn, tx) =>
            {
                const string upsertLocal = @"
INSERT INTO ipli_exm_att_file
    (case_seq_no, item_no, file_name, file_type, file_object_key,
     file_memo, hl7, orig_file_name, file_attach, document_type)
VALUES
    (@CaseSeqNo, @ItemNo, @FileName, @FileType, @FileObjectKey,
     @FileMemo, @Hl7, @OrigFileName, @FileAttach, @DocumentType)
ON CONFLICT (case_seq_no, item_no) DO UPDATE SET
    file_name = EXCLUDED.file_name,
    file_type = EXCLUDED.file_type,
    file_object_key = EXCLUDED.file_object_key,
    file_memo = EXCLUDED.file_memo,
    hl7 = EXCLUDED.hl7,
    orig_file_name = EXCLUDED.orig_file_name,
    file_attach = EXCLUDED.file_attach,
    document_type = EXCLUDED.document_type";

                const string upsertRemote = @"
INSERT INTO nhi_idc.iple_recv_att_file
    (case_seq_no, item_no, file_name, file_type, file_object_key,
     file_memo, hl7, orig_file_name, file_attach, document_type)
VALUES
    (@CaseSeqNo, @ItemNo, @FileName, @FileType, @FileObjectKey,
     @FileMemo, @Hl7, @OrigFileName, @FileAttach, @DocumentType)
ON CONFLICT (case_seq_no, item_no) DO UPDATE SET
    file_name = EXCLUDED.file_name,
    file_type = EXCLUDED.file_type,
    file_object_key = EXCLUDED.file_object_key,
    file_memo = EXCLUDED.file_memo,
    hl7 = EXCLUDED.hl7,
    orig_file_name = EXCLUDED.orig_file_name,
    file_attach = EXCLUDED.file_attach,
    document_type = EXCLUDED.document_type";

                var param = new
                {
                    entity.CaseSeqNo,
                    entity.ItemNo,
                    entity.FileName,
                    entity.FileType,
                    entity.FileObjectKey,
                    entity.FileMemo,
                    entity.Hl7,
                    entity.OrigFileName,
                    entity.FileAttach,
                    entity.DocumentType,
                };

                await conn.ExecuteAsync(upsertLocal, param, tx);
                await conn.ExecuteAsync(upsertRemote, param, tx);
                return 1;
            }, logger);
        }

        #endregion

        #region Private helpers

        private static (string where, IEnumerable<NpgsqlParameter> parms) BuildQueryFilter(QueryStatusDto filter)
        {
            var conditions = new List<string>();
            var parms = new List<NpgsqlParameter>();

            if (!string.IsNullOrEmpty(filter.CaseSeqNo))
            {
                conditions.Add("case_seq_no = @CaseSeqNo");
                parms.Add(new("CaseSeqNo", filter.CaseSeqNo));
            }
            if (!string.IsNullOrEmpty(filter.BusCode))
            {
                conditions.Add("bus_code = @BusCode");
                parms.Add(new("BusCode", filter.BusCode));
            }
            if (!string.IsNullOrEmpty(filter.HospId))
            {
                conditions.Add("hosp_id = @HospId");
                parms.Add(new("HospId", filter.HospId));
            }
            if (!string.IsNullOrEmpty(filter.CaseStatus))
            {
                conditions.Add("case_status = @CaseStatus");
                parms.Add(new("CaseStatus", filter.CaseStatus));
            }
            if (!string.IsNullOrEmpty(filter.PgmProcStatus))
            {
                conditions.Add("pgm_proc_status = @PgmProcStatus");
                parms.Add(new("PgmProcStatus", filter.PgmProcStatus));
            }
            if (!string.IsNullOrEmpty(filter.FileSendStatus))
            {
                conditions.Add("file_send_status = @FileSendStatus");
                parms.Add(new("FileSendStatus", filter.FileSendStatus));
            }
            if (filter.DaysBack is > 0)
            {
                conditions.Add("add_time >= CURRENT_TIMESTAMP - (@DaysBack || ' days')::interval");
                parms.Add(new("DaysBack", filter.DaysBack.Value.ToString()));
            }

            string where = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : "";
            return (where, parms);
        }

        private static IpltFileProcLog? MapDynamicToFileProcLog(dynamic? row)
        {
            if (row is null) return null;
            var d = (IDictionary<string, object>)row;
            return new IpltFileProcLog
            {
                CaseSeqNo = d["case_seq_no"]?.ToString() ?? "",
                BusCode = d["bus_code"]?.ToString() ?? "",
                HospId = d["hosp_id"]?.ToString(),
                BranchCode = d["branch_code"]?.ToString(),
                CaseStatus = d["case_status"]?.ToString() ?? "0",
                PgmProcStatus = d["pgm_proc_status"]?.ToString() ?? "0",
                FileSendStatus = d["file_send_status"]?.ToString() ?? "0",
                RecvSeqNo = d["recv_seq_no"]?.ToString(),
                ReadPos = d["read_pos"]?.ToString(),
                Hl7 = d["hl7"]?.ToString(),
                UploadFormat = d["upload_format"]?.ToString(),
                AddTime = d["add_time"] is DateTime dt ? dt : null,
                PgmTimeS = d["pgm_time_s"] is DateTime ts ? ts : null,
                PgmTimeE = d["pgm_time_e"] is DateTime te ? te : null,
                FileTimeS = d["file_time_s"] is DateTime fs ? fs : null,
                FileTimeE = d["file_time_e"] is DateTime fe ? fe : null,
                ProcPos = d["proc_pos"]?.ToString(),
                ErrCode = d["err_code"]?.ToString(),
                ProcErrMsg = d["proc_err_msg"]?.ToString(),
            };
        }

        private static IpleRecvLog? MapDynamicToRecvLog(dynamic? row)
        {
            if (row is null) return null;
            var d = (IDictionary<string, object>)row;
            return new IpleRecvLog
            {
                RecvSeqNo = d["recv_seq_no"]?.ToString() ?? "",
                BusCode = d["bus_code"]?.ToString() ?? "",
                HospId = d["hosp_id"]?.ToString(),
                BranchCode = d["branch_code"]?.ToString(),
                FileType = d["file_type"]?.ToString(),
                RecvMode = d["recv_mode"]?.ToString(),
                OrigFileName = d["orig_file_name"]?.ToString(),
                FileSize = d["file_size"] is decimal sz ? sz : null,
                FileObjectKey = d["file_object_key"]?.ToString(),
                SamId = d["sam_id"]?.ToString(),
                JobId = d["job_id"]?.ToString(),
                PcCode = d["pc_code"]?.ToString(),
                ReadPos = d["read_pos"]?.ToString(),
                AddTime = d["add_time"] is DateTime at ? at : null,
                ProcResult = d["proc_result"]?.ToString(),
                ProcTime = d["proc_time"] is DateTime pt ? pt : null,
            };
        }

        private static IpliExmAttFile? MapDynamicToAttFile(dynamic? row)
        {
            if (row is null) return null;
            var d = (IDictionary<string, object>)row;
            return new IpliExmAttFile
            {
                CaseSeqNo = d["case_seq_no"]?.ToString() ?? "",
                ItemNo = d["item_no"] is decimal n ? n : 0,
                FileName = d["file_name"]?.ToString(),
                FileType = d["file_type"]?.ToString(),
                FileObjectKey = d["file_object_key"]?.ToString(),
                FileMemo = d["file_memo"]?.ToString(),
                Hl7 = d["hl7"]?.ToString(),
                OrigFileName = d["orig_file_name"]?.ToString(),
                FileAttach = d["file_attach"]?.ToString(),
                DocumentType = d["document_type"]?.ToString(),
            };
        }

        #endregion
    }
}

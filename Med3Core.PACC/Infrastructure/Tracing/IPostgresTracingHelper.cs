using Npgsql;
using System.Runtime.CompilerServices;

namespace Med3Core.PACC.Infrastructure.Tracing
{
    /// <summary>
    /// PostgreSQL 追蹤輔助介面。對齊 IOracleTracingHelper 的三個方法。
    /// PG/Mongo 各自有自己的 helper；Repository 只注入對應 helper、不跨用。
    /// </summary>
    public interface IPostgresTracingHelper
    {
        /// <summary>執行查詢並回傳 dynamic（搭配 Dapper SqlMapper.QueryAsync）。</summary>
        Task<IEnumerable<dynamic>> ExecuteReaderAsync(string sql, IEnumerable<NpgsqlParameter>? parameters, NLog.Logger logger);

        /// <summary>執行 DML 並回傳影響筆數。</summary>
        Task<int> ExecuteNonQueryAsync(string sql, IEnumerable<NpgsqlParameter>? parameters, NLog.Logger logger);

        /// <summary>在交易範圍內執行 work。Commit 失敗時自動 Rollback 再 rethrow。</summary>
        Task<TResult> ExecuteTransactionAsync<TResult>(
            Func<NpgsqlConnection, NpgsqlTransaction, Task<TResult>> work,
            NLog.Logger logger,
            [CallerMemberName] string callerName = "");
    }
}

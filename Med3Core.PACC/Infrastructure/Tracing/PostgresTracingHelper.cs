using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Med3Core.PACC.Infrastructure.Tracing
{
    /// <summary>IPostgresTracingHelper 預設實作。Npgsql + Dapper + OpenTelemetry Activity。</summary>
    public class PostgresTracingHelper : IPostgresTracingHelper
    {
        private readonly string _connectionString;
        private readonly ActivitySource _activitySource = new ActivitySource("OpenTelemetrySource");

        /// <summary>建構子。</summary>
        /// <param name="configuration">需含 ConnectionStrings:Postgres。</param>
        public PostgresTracingHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Postgres")
                ?? throw new InvalidOperationException("ConnectionStrings:Postgres is not configured.");
            DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<dynamic>> ExecuteReaderAsync(string sql, IEnumerable<NpgsqlParameter>? parameters, NLog.Logger logger)
            => await ExecuteWithTracingAsync(sql, parameters, logger, async (conn, p) => await conn.QueryAsync(sql, p));

        /// <inheritdoc/>
        public async Task<int> ExecuteNonQueryAsync(string sql, IEnumerable<NpgsqlParameter>? parameters, NLog.Logger logger)
            => await ExecuteWithTracingAsync(sql, parameters, logger, async (conn, p) => await conn.ExecuteAsync(sql, p));

        /// <inheritdoc/>
        public async Task<TResult> ExecuteTransactionAsync<TResult>(
            Func<NpgsqlConnection, NpgsqlTransaction, Task<TResult>> work,
            NLog.Logger logger,
            [CallerMemberName] string callerName = "")
        {
            using Activity? activity = _activitySource.StartActivity("PostgresDB-Tx-" + callerName);
            activity?.SetTag("db.system", "postgresql");

            await using NpgsqlConnection conn = new(_connectionString);
            await conn.OpenAsync();
            await using NpgsqlTransaction tx = await conn.BeginTransactionAsync();

            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                TResult result = await work(conn, tx);
                await tx.CommitAsync();
                sw.Stop();
                logger.Trace("[PG-TX {Caller}] OK {ElapsedMs}ms", callerName, sw.ElapsedMilliseconds);
                return result;
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                sw.Stop();
                logger.Error(ex, "[PG-TX {Caller}] FAIL {ElapsedMs}ms", callerName, sw.ElapsedMilliseconds);
                throw;
            }
        }

        private async Task<TResult> ExecuteWithTracingAsync<TResult>(
            string sql, IEnumerable<NpgsqlParameter>? parameters, NLog.Logger logger,
            Func<NpgsqlConnection, DynamicParameters, Task<TResult>> body,
            [CallerMemberName] string callerName = "")
        {
            using Activity? activity = _activitySource.StartActivity("PostgresDB-" + callerName);
            activity?.SetTag("db.system", "postgresql");
            activity?.SetTag("db.statement", sql.Length > 500 ? sql[..500] : sql);

            await using NpgsqlConnection conn = new(_connectionString);
            await conn.OpenAsync();

            DynamicParameters dyn = new();
            if (parameters is not null)
            {
                foreach (NpgsqlParameter p in parameters)
                {
                    dyn.Add(p.ParameterName, p.Value == DBNull.Value ? null : p.Value);
                }
            }

            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                TResult result = await body(conn, dyn);
                sw.Stop();
                logger.Trace("[PG {Caller}] OK {ElapsedMs}ms", callerName, sw.ElapsedMilliseconds);
                return result;
            }
            catch (Exception ex)
            {
                sw.Stop();
                logger.Error(ex, "[PG {Caller}] FAIL {ElapsedMs}ms sql={Sql}", callerName, sw.ElapsedMilliseconds, sql);
                throw;
            }
        }
    }
}

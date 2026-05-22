namespace Med3Core.PACC.Application.StateMachine
{
    /// <summary>
    /// 案件狀態機白名單（規格 §2.3）。LogError 不經此驗證。
    /// </summary>
    public static class CaseStateMachine
    {
        private static readonly HashSet<(string, string)> CaseTransitions = new()
        {
            ("0", "1"), ("1", "2"), ("1", "F"), ("2", "F"),
        };

        private static readonly HashSet<(string, string)> PgmProcTransitions = new()
        {
            ("0", "9"), ("9", "1"), ("9", "2"),
        };

        private static readonly HashSet<(string, string)> FileSendTransitions = new()
        {
            ("0", "9"), ("9", "1"), ("9", "2"), ("9", "3"), ("0", "3"), ("3", "9"),
        };

        /// <summary>檢查 CASE_STATUS 轉換合法。</summary>
        public static bool IsValidCaseStatusTransition(string? from, string to)
            => from is null || from == to || CaseTransitions.Contains((from, to));

        /// <summary>檢查 PGM_PROC_STATUS 轉換合法。</summary>
        public static bool IsValidPgmProcStatusTransition(string? from, string to)
            => from is null || from == to || PgmProcTransitions.Contains((from, to));

        /// <summary>檢查 FILE_SEND_STATUS 轉換合法。</summary>
        public static bool IsValidFileSendStatusTransition(string? from, string to)
            => from is null || from == to || FileSendTransitions.Contains((from, to));
    }
}

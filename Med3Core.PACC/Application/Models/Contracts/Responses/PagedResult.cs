namespace Med3Core.PACC.Application.Models.Contracts.Responses
{
    /// <summary>分頁查詢結果包裝。</summary>
    public class PagedResult<T>
    {
        /// <summary>總筆數。</summary>
        public int TotalCount { get; set; }

        /// <summary>頁碼（從 1 開始）。</summary>
        public int PageNo { get; set; }

        /// <summary>每頁筆數。</summary>
        public int PageSize { get; set; }

        /// <summary>當頁資料。</summary>
        public List<T> Items { get; set; } = new();
    }
}

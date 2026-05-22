using FluentValidation;
using Med3Core.PACC.Application.Models.Contracts.Requests;

namespace Med3Core.PACC.Application.Models.Validators
{
    /// <summary>查詢案件狀態 Validator（規格 §2.2）。</summary>
    public class QueryStatusDtoValidator : AbstractValidator<QueryStatusDto>
    {
        /// <summary>建構 Validator。</summary>
        public QueryStatusDtoValidator()
        {
            RuleFor(x => x.CaseStatus).Must(x => x is null || new[] { "0", "1", "2", "F" }.Contains(x)).WithErrorCode("E004");
            RuleFor(x => x.PgmProcStatus).Must(x => x is null || new[] { "0", "9", "1", "2" }.Contains(x)).WithErrorCode("E004");
            RuleFor(x => x.FileSendStatus).Must(x => x is null || x.Split(',').All(v => new[] { "0", "9", "1", "2", "3" }.Contains(v.Trim()))).WithErrorCode("E004").WithMessage("FileSendStatus 含非法值");
            RuleFor(x => x.DaysBack).Must(x => x is null || (x >= 1 && x <= 365)).WithErrorCode("E004").WithMessage("DaysBack 範圍 1-365");
            RuleFor(x => x.PageSize).Must(x => x is null || (x >= 1 && x <= 1000)).WithErrorCode("E004").WithMessage("PageSize 範圍 1-1000");
            RuleFor(x => x).Must(HasAtLeastOneFilter).WithErrorCode("E004").WithMessage("至少需提供一個篩選條件");
        }

        private static bool HasAtLeastOneFilter(QueryStatusDto x)
            => !string.IsNullOrWhiteSpace(x.CaseSeqNo) || !string.IsNullOrWhiteSpace(x.BusCode)
            || !string.IsNullOrWhiteSpace(x.CaseStatus) || !string.IsNullOrWhiteSpace(x.PgmProcStatus)
            || !string.IsNullOrWhiteSpace(x.FileSendStatus);
    }
}

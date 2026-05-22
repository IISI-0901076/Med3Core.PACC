using FluentValidation;
using Med3Core.PACC.Application.Models.Contracts.Requests;

namespace Med3Core.PACC.Application.Models.Validators
{
    /// <summary>更新案件狀態 Validator（規格 §2.3）。狀態機轉換由 Service 層做。</summary>
    public class UpdateCaseStatusDtoValidator : AbstractValidator<UpdateCaseStatusDto>
    {
        /// <summary>建構 Validator。</summary>
        public UpdateCaseStatusDtoValidator()
        {
            RuleFor(x => x.CaseSeqNo).NotEmpty().WithErrorCode("E004");
            RuleFor(x => x.CaseStatus).Must(x => x is null || new[] { "0", "1", "2", "F" }.Contains(x)).WithErrorCode("E004");
            RuleFor(x => x.PgmProcStatus).Must(x => x is null || new[] { "0", "9", "1", "2" }.Contains(x)).WithErrorCode("E004");
            RuleFor(x => x.FileSendStatus).Must(x => x is null || new[] { "0", "9", "1", "2", "3" }.Contains(x)).WithErrorCode("E004");
        }
    }
}

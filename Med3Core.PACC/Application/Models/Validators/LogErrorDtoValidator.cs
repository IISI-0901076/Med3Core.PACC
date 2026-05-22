using FluentValidation;
using Med3Core.PACC.Application.Models.Contracts.Requests;

namespace Med3Core.PACC.Application.Models.Validators
{
    /// <summary>記錄錯誤 Validator（規格 §2.6）。</summary>
    public class LogErrorDtoValidator : AbstractValidator<LogErrorDto>
    {
        /// <summary>建構 Validator。</summary>
        public LogErrorDtoValidator()
        {
            RuleFor(x => x.CaseSeqNo).NotEmpty().WithErrorCode("E004");
            RuleFor(x => x.ErrCode).NotEmpty().MaximumLength(10).WithErrorCode("E004");
            RuleFor(x => x.ProcErrMsg).NotEmpty().MaximumLength(200).WithErrorCode("E004");
            RuleFor(x => x.ErrorPhase).NotEmpty().Must(x => x == "PGM" || x == "FILE_SEND").WithErrorCode("E004").WithMessage("ErrorPhase 限 PGM 或 FILE_SEND");
        }
    }
}

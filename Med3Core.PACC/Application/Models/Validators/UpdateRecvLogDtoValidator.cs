using FluentValidation;
using Med3Core.PACC.Application.Models.Contracts.Requests;

namespace Med3Core.PACC.Application.Models.Validators
{
    /// <summary>更新收件記錄 Validator（規格 §2.8）。</summary>
    public class UpdateRecvLogDtoValidator : AbstractValidator<UpdateRecvLogDto>
    {
        /// <summary>建構 Validator。</summary>
        public UpdateRecvLogDtoValidator()
        {
            RuleFor(x => x.RecvSeqNo).NotEmpty().WithErrorCode("E004");
            RuleFor(x => x.ProcResult).Must(x => x is null || x == "1" || x == "2").WithErrorCode("E004").WithMessage("ProcResult 限 1 或 2");
        }
    }
}

using FluentValidation;
using Med3Core.PACC.Application.Models.Contracts.Requests;

namespace Med3Core.PACC.Application.Models.Validators
{
    /// <summary>新增案件 Validator（規格 §2.1）。</summary>
    public class CreateCaseDtoValidator : AbstractValidator<CreateCaseDto>
    {
        private static readonly string[] AllowedBusCodes = { "IAV", "IPL", "QBJ", "QB1", "QB2", "QB3", "QB4", "QB6", "REA", "RWM", "RCP" };

        /// <summary>建構 Validator。</summary>
        public CreateCaseDtoValidator()
        {
            RuleFor(x => x.CaseSeqNo).NotEmpty().MaximumLength(20).WithErrorCode("E004");
            RuleFor(x => x.BusCode).NotEmpty().Must(x => AllowedBusCodes.Contains(x)).WithErrorCode("E004").WithMessage("BusCode 非白名單值");
            RuleFor(x => x.HospId).NotEmpty().MaximumLength(10).WithErrorCode("E004");
            RuleFor(x => x.CaseStatus).NotEmpty().Must(x => x == "0" || x == "1").WithErrorCode("E004").WithMessage("CaseStatus 限 0 或 1");
            RuleFor(x => x.ReadPos).Must(x => x is null || x == "1" || x == "2").WithErrorCode("E004").WithMessage("ReadPos 限 1 或 2");
        }
    }
}

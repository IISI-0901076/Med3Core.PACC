using FluentValidation;
using Med3Core.PACC.Application.Models.Contracts.Requests;

namespace Med3Core.PACC.Application.Models.Validators
{
    /// <summary>同步附件 Validator（規格 §2.4）。</summary>
    public class UpdateCaseFilesDtoValidator : AbstractValidator<UpdateCaseFilesDto>
    {
        /// <summary>建構 Validator。</summary>
        public UpdateCaseFilesDtoValidator()
        {
            RuleFor(x => x.CaseSeqNo).NotEmpty().MaximumLength(20).WithErrorCode("E004");
            RuleFor(x => x.FileObjectKey).NotEmpty().Must(HasAtLeastThreeSlashes).WithErrorCode("E005").WithMessage("fileObjectKey 格式不正確");
        }

        private static bool HasAtLeastThreeSlashes(string key) => !string.IsNullOrEmpty(key) && key.Count(c => c == '/') >= 3;
    }
}

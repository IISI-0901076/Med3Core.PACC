using FluentValidation;
using Med3Core.PACC.Application.Models.Contracts.Requests;

namespace Med3Core.PACC.Application.Models.Validators
{
    /// <summary>更新檔案位置 Validator（規格 §2.9）。</summary>
    public class UpdateFileLocationDtoValidator : AbstractValidator<UpdateFileLocationDto>
    {
        /// <summary>建構 Validator。</summary>
        public UpdateFileLocationDtoValidator()
        {
            RuleFor(x => x.CaseSeqNo).NotEmpty().WithErrorCode("E004");
            RuleFor(x => x.FileObjectKey).NotEmpty().Must(x => x.Contains('/')).WithErrorCode("E005").WithMessage("FileObjectKey 格式不正確");
        }
    }
}

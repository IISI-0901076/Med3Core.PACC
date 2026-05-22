using FluentValidation;
using Med3Core.PACC.Application.Models.Contracts.Requests;

namespace Med3Core.PACC.Application.Models.Validators
{
    /// <summary>新增附件記錄 Validator（規格 §2.10）。</summary>
    public class CreateAttFileDtoValidator : AbstractValidator<CreateAttFileDto>
    {
        private static readonly string[] AllowedFileTypes = { "DICOM", "XML", "FHR", "DCF", "MPG" };

        /// <summary>建構 Validator。</summary>
        public CreateAttFileDtoValidator()
        {
            RuleFor(x => x.CaseSeqNo).NotEmpty().WithErrorCode("E004");
            RuleFor(x => x.ItemNo).GreaterThanOrEqualTo(1).WithErrorCode("E004");
            RuleFor(x => x.FileName).NotEmpty().MaximumLength(200).WithErrorCode("E004");
            RuleFor(x => x.FileType).NotEmpty().Must(x => AllowedFileTypes.Contains(x)).WithErrorCode("E004").WithMessage("FileType 非白名單");
            RuleFor(x => x.FileObjectKey).NotEmpty().WithErrorCode("E004");
        }
    }
}

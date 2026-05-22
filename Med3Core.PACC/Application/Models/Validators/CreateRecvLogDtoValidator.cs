using FluentValidation;
using Med3Core.PACC.Application.Models.Contracts.Requests;

namespace Med3Core.PACC.Application.Models.Validators
{
    /// <summary>新增收件記錄 Validator（規格 §2.7）。</summary>
    public class CreateRecvLogDtoValidator : AbstractValidator<CreateRecvLogDto>
    {
        private static readonly string[] AllowedBusCodes = { "IPL", "QBJ", "IAV" };
        private static readonly string[] AllowedFileTypes = { "XML", "FHR", "DCF", "DICOM" };

        /// <summary>建構 Validator。</summary>
        public CreateRecvLogDtoValidator()
        {
            RuleFor(x => x.RecvSeqNo).NotEmpty().MaximumLength(20).WithErrorCode("E004");
            RuleFor(x => x.BusCode).NotEmpty().Must(x => AllowedBusCodes.Contains(x)).WithErrorCode("E004").WithMessage("BusCode 非白名單");
            RuleFor(x => x.FileType).NotEmpty().Must(x => AllowedFileTypes.Contains(x)).WithErrorCode("E004").WithMessage("FileType 非白名單");
            RuleFor(x => x.RecvMode).NotEmpty().Must(x => x == "2" || x == "3").WithErrorCode("E004").WithMessage("RecvMode 限 2 或 3");
            RuleFor(x => x.OrigFileName).NotEmpty().MaximumLength(200).WithErrorCode("E004");
            RuleFor(x => x.FileSize).GreaterThan(0).WithErrorCode("E004").WithMessage("FileSize 須大於 0");
            RuleFor(x => x.FileObjectKey).NotEmpty().Must(x => x.Contains('/')).WithErrorCode("E004").WithMessage("FileObjectKey 須含 / 分隔");
            RuleFor(x => x.ReadPos).NotEmpty().Must(x => x == "1" || x == "2").WithErrorCode("E004").WithMessage("ReadPos 限 1 或 2");
        }
    }
}

using FluentValidation;
using Med3Core.PACC.Application.Models.Contracts.Requests;

namespace Med3Core.PACC.Application.Models.Validators
{
    /// <summary>查詢附件清單 Validator（規格 §2.5）。</summary>
    public class QueryCaseFilesRequestDtoValidator : AbstractValidator<QueryCaseFilesRequestDto>
    {
        private static readonly string[] AllowedFileAttach = { "DCF", "MPG", "FHR", "XML", "DICOM" };

        /// <summary>建構 Validator。</summary>
        public QueryCaseFilesRequestDtoValidator()
        {
            RuleFor(x => x.CaseSeqNo).NotEmpty().MaximumLength(20).WithErrorCode("E004");
            RuleFor(x => x.FileAttach).Must(x => x is null || AllowedFileAttach.Contains(x)).WithErrorCode("E004").WithMessage("FileAttach 非白名單");
        }
    }
}

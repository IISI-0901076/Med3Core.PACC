using Med3Core.PACC.Application.Models.Contracts.Requests;
using Med3Core.PACC.Application.Services;
using Med3Core.PACC.Domain.Common;
using Med3Core.PACC.Test.Fakes;
using Med3Core.PACC.Test.TestUtilities;
using Xunit;

namespace Med3Core.PACC.Test.Services
{
    public class PaccService_LogErrorTests
    {
        private readonly FakePaccRepository _repo = new();
        private readonly PaccService _svc;

        public PaccService_LogErrorTests()
        {
            _svc = new PaccService(_repo);
        }

        [Fact]
        public async Task 記錄錯誤_PGM階段_PgmProcStatus設為2並填入PgmTimeE()
        {
            await _svc.CreateCaseAsync(DtoBuilder.CreateCase("LE001"));

            var result = await _svc.LogErrorAsync(new LogErrorDto
            {
                CaseSeqNo = "LE001",
                ErrCode = "E400",
                ProcErrMsg = "test error",
                ErrorPhase = "PGM"
            });

            Assert.Equal("LE001", result.CaseSeqNo);
            Assert.Equal("E400", result.ErrCode);
            Assert.Equal("2", _repo.FileProcLogs["LE001"].PgmProcStatus);
            Assert.NotNull(_repo.FileProcLogs["LE001"].PgmTimeE);
        }

        [Fact]
        public async Task 記錄錯誤_FILE_SEND階段_FileSendStatus設為2()
        {
            await _svc.CreateCaseAsync(DtoBuilder.CreateCase("LE002"));

            await _svc.LogErrorAsync(new LogErrorDto
            {
                CaseSeqNo = "LE002",
                ErrCode = "E448",
                ProcErrMsg = "file error",
                ErrorPhase = "FILE_SEND"
            });

            Assert.Equal("2", _repo.FileProcLogs["LE002"].FileSendStatus);
            Assert.NotNull(_repo.FileProcLogs["LE002"].FileTimeE);
        }

        [Fact]
        public async Task 記錄錯誤_案件不存在_拋出E001()
        {
            var ex = await Assert.ThrowsAsync<BusinessException>(() =>
                _svc.LogErrorAsync(new LogErrorDto { CaseSeqNo = "NOPE", ErrCode = "E400", ProcErrMsg = "x", ErrorPhase = "PGM" }));
            Assert.Equal("E001", ex.ErrorCode);
        }
    }
}

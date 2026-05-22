using Med3Core.PACC.Application.Models.Contracts.Requests;
using Med3Core.PACC.Application.Models.Contracts.Responses;
using Med3Core.PACC.Application.Services;
using Med3Core.PACC.Domain.Common;
using Med3Core.PACC.Test.Fakes;
using Med3Core.PACC.Test.TestUtilities;
using Xunit;

namespace Med3Core.PACC.Test.Services
{
    public class PaccService_CreateCaseTests
    {
        private readonly FakePaccRepository _repo = new();
        private readonly PaccService _svc;

        public PaccService_CreateCaseTests()
        {
            _svc = new PaccService(_repo);
        }

        [Fact]
        public async Task 新增案件_正常輸入_成功回傳案件資料()
        {
            var dto = DtoBuilder.CreateCase();
            CaseStatusResultDto result = await _svc.CreateCaseAsync(dto);

            Assert.Equal("CASE001", result.CaseSeqNo);
            Assert.Equal("IPL", result.BusCode);
            Assert.Equal("0", result.CaseStatus);
            Assert.NotNull(result.AddTime);
        }

        [Fact]
        public async Task 新增案件_案件序號已存在_拋出E006重複建案()
        {
            var dto = DtoBuilder.CreateCase();
            await _svc.CreateCaseAsync(dto);

            var ex = await Assert.ThrowsAsync<BusinessException>(() => _svc.CreateCaseAsync(dto));
            Assert.Equal("E006", ex.ErrorCode);
        }

        [Fact]
        public async Task 新增案件_不同序號_各自獨立建立()
        {
            await _svc.CreateCaseAsync(DtoBuilder.CreateCase("CASE_A"));
            await _svc.CreateCaseAsync(DtoBuilder.CreateCase("CASE_B"));

            Assert.Equal(2, _repo.FileProcLogs.Count);
        }
    }
}

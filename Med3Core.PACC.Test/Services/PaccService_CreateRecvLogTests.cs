using Med3Core.PACC.Application.Models.Contracts.Requests;
using Med3Core.PACC.Application.Services;
using Med3Core.PACC.Domain.Common;
using Med3Core.PACC.Test.Fakes;
using Med3Core.PACC.Test.TestUtilities;
using Xunit;

namespace Med3Core.PACC.Test.Services
{
    public class PaccService_CreateRecvLogTests
    {
        private readonly FakePaccRepository _repo = new();
        private readonly PaccService _svc;

        public PaccService_CreateRecvLogTests()
        {
            _svc = new PaccService(_repo);
        }

        [Fact]
        public async Task 新增收件記錄_正常輸入_成功回傳收件資料()
        {
            var dto = DtoBuilder.CreateRecvLog();
            var result = await _svc.CreateRecvLogAsync(dto);

            Assert.Equal("RECV001", result.RecvSeqNo);
            Assert.NotNull(result.AddTime);
        }

        [Fact]
        public async Task 新增收件記錄_收件序號已存在_拋出E007()
        {
            await _svc.CreateRecvLogAsync(DtoBuilder.CreateRecvLog("DUP_RECV"));

            var ex = await Assert.ThrowsAsync<BusinessException>(() =>
                _svc.CreateRecvLogAsync(DtoBuilder.CreateRecvLog("DUP_RECV")));
            Assert.Equal("E007", ex.ErrorCode);
        }

        [Fact]
        public async Task 新增收件記錄_不同序號_各自獨立()
        {
            await _svc.CreateRecvLogAsync(DtoBuilder.CreateRecvLog("R1"));
            await _svc.CreateRecvLogAsync(DtoBuilder.CreateRecvLog("R2"));

            Assert.Equal(2, _repo.RecvLogs.Count);
        }
    }
}

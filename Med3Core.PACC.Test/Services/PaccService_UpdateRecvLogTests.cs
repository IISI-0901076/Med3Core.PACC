using Med3Core.PACC.Application.Models.Contracts.Requests;
using Med3Core.PACC.Application.Services;
using Med3Core.PACC.Domain.Common;
using Med3Core.PACC.Test.Fakes;
using Med3Core.PACC.Test.TestUtilities;
using Xunit;

namespace Med3Core.PACC.Test.Services
{
    public class PaccService_UpdateRecvLogTests
    {
        private readonly FakePaccRepository _repo = new();
        private readonly PaccService _svc;

        public PaccService_UpdateRecvLogTests()
        {
            _svc = new PaccService(_repo);
        }

        [Fact]
        public async Task 更新收件記錄_僅提供ProcResult_其他欄位保留原值()
        {
            await _svc.CreateRecvLogAsync(DtoBuilder.CreateRecvLog("UR001"));

            var result = await _svc.UpdateRecvLogAsync(new UpdateRecvLogDto
            {
                RecvSeqNo = "UR001",
                ProcResult = "1"
            });

            Assert.Equal("1", result.ProcResult);
            Assert.Equal("1", _repo.RecvLogs["UR001"].ReadPos); // 保留原值
        }

        [Fact]
        public async Task 更新收件記錄_收件序號不存在_拋出E002()
        {
            var ex = await Assert.ThrowsAsync<BusinessException>(() =>
                _svc.UpdateRecvLogAsync(new UpdateRecvLogDto { RecvSeqNo = "NOPE", ProcResult = "1" }));
            Assert.Equal("E002", ex.ErrorCode);
        }

        [Fact]
        public async Task 更新收件記錄_更新FileObjectKey_成功寫入()
        {
            await _svc.CreateRecvLogAsync(DtoBuilder.CreateRecvLog("UR002"));

            await _svc.UpdateRecvLogAsync(new UpdateRecvLogDto
            {
                RecvSeqNo = "UR002",
                FileObjectKey = "IPL/0401/UR002/new.xml"
            });

            Assert.Equal("IPL/0401/UR002/new.xml", _repo.RecvLogs["UR002"].FileObjectKey);
        }
    }
}

using Med3Core.PACC.Application.Models.Contracts.Requests;
using Med3Core.PACC.Application.Services;
using Med3Core.PACC.Domain.Common;
using Med3Core.PACC.Test.Fakes;
using Med3Core.PACC.Test.TestUtilities;
using Xunit;

namespace Med3Core.PACC.Test.Services
{
    public class PaccService_UpdateFileLocationTests
    {
        private readonly FakePaccRepository _repo = new();
        private readonly PaccService _svc;

        public PaccService_UpdateFileLocationTests()
        {
            _svc = new PaccService(_repo);
        }

        [Fact]
        public async Task 更新檔案位置_案件存在_成功更新ProcPos()
        {
            await _svc.CreateCaseAsync(DtoBuilder.CreateCase("UFL001"));

            var result = await _svc.UpdateFileLocationAsync(new UpdateFileLocationDto
            {
                CaseSeqNo = "UFL001",
                FileObjectKey = "IPL/0401/UFL001/output.xml"
            });

            Assert.Equal("UFL001", result.CaseSeqNo);
            Assert.Equal("IPL/0401/UFL001/output.xml", result.FileObjectKey);
            Assert.Equal("IPL/0401/UFL001/output.xml", _repo.FileProcLogs["UFL001"].ProcPos);
        }

        [Fact]
        public async Task 更新檔案位置_案件不存在_拋出E001()
        {
            var ex = await Assert.ThrowsAsync<BusinessException>(() =>
                _svc.UpdateFileLocationAsync(new UpdateFileLocationDto { CaseSeqNo = "NOPE", FileObjectKey = "a/b/c/d" }));
            Assert.Equal("E001", ex.ErrorCode);
        }

        [Fact]
        public async Task 更新檔案位置_多次更新_最後值生效()
        {
            await _svc.CreateCaseAsync(DtoBuilder.CreateCase("UFL002"));

            await _svc.UpdateFileLocationAsync(new UpdateFileLocationDto { CaseSeqNo = "UFL002", FileObjectKey = "path1" });
            await _svc.UpdateFileLocationAsync(new UpdateFileLocationDto { CaseSeqNo = "UFL002", FileObjectKey = "path2" });

            Assert.Equal("path2", _repo.FileProcLogs["UFL002"].ProcPos);
        }
    }
}

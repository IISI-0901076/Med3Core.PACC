using Med3Core.PACC.Application.Models.Contracts.Requests;
using Med3Core.PACC.Application.Services;
using Med3Core.PACC.Domain.Common;
using Med3Core.PACC.Domain.Entities;
using Med3Core.PACC.Test.Fakes;
using Med3Core.PACC.Test.TestUtilities;
using Xunit;

namespace Med3Core.PACC.Test.Services
{
    public class PaccService_UpdateCaseFilesTests
    {
        private readonly FakePaccRepository _repo = new();
        private readonly PaccService _svc;

        public PaccService_UpdateCaseFilesTests()
        {
            _svc = new PaccService(_repo);
        }

        [Fact]
        public async Task 同步附件_案件存在且遠端有資料_成功同步()
        {
            await _svc.CreateCaseAsync(DtoBuilder.CreateCase("UCF001"));
            _repo.RemoteAttFiles[("UCF001", 1m)] = new IpliExmAttFile
            {
                CaseSeqNo = "UCF001", ItemNo = 1, FileName = "remote.xml", FileType = "XML"
            };

            var result = await _svc.UpdateCaseFilesAsync(new UpdateCaseFilesDto
            {
                CaseSeqNo = "UCF001",
                FileObjectKey = "IPL/0401/UCF001/remote.xml"
            });

            Assert.True(result.SyncedToLocal);
            Assert.Single(_repo.AttFiles);
        }

        [Fact]
        public async Task 同步附件_案件序號不存在_拋出E001()
        {
            var ex = await Assert.ThrowsAsync<BusinessException>(() =>
                _svc.UpdateCaseFilesAsync(new UpdateCaseFilesDto { CaseSeqNo = "NOPE", FileObjectKey = "a/b/c/d" }));
            Assert.Equal("E001", ex.ErrorCode);
        }

        [Fact]
        public async Task 同步附件_遠端無資料_回傳SyncedToLocal為True且附件為空()
        {
            await _svc.CreateCaseAsync(DtoBuilder.CreateCase("UCF002"));

            var result = await _svc.UpdateCaseFilesAsync(new UpdateCaseFilesDto
            {
                CaseSeqNo = "UCF002",
                FileObjectKey = "IPL/0401/UCF002/x.xml"
            });

            Assert.True(result.SyncedToLocal);
            Assert.Empty(_repo.AttFiles);
        }
    }
}

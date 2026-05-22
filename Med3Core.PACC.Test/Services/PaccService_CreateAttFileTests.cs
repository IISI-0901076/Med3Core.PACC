using Med3Core.PACC.Application.Services;
using Med3Core.PACC.Domain.Entities;
using Med3Core.PACC.Test.Fakes;
using Med3Core.PACC.Test.TestUtilities;
using Xunit;

namespace Med3Core.PACC.Test.Services
{
    public class PaccService_CreateAttFileTests
    {
        private readonly FakePaccRepository _repo = new();
        private readonly PaccService _svc;

        public PaccService_CreateAttFileTests()
        {
            _svc = new PaccService(_repo);
        }

        [Fact]
        public async Task 新增附件_正常輸入_成功寫入本地與遠端()
        {
            var dto = DtoBuilder.CreateAttFile("ATT001", 1);
            var result = await _svc.CreateAttFileAsync(dto);

            Assert.Equal("ATT001", result.CaseSeqNo);
            Assert.Equal(1m, result.ItemNo);
            Assert.True(_repo.AttFiles.ContainsKey(("ATT001", 1m)));
            Assert.True(_repo.RemoteAttFiles.ContainsKey(("ATT001", 1m)));
        }

        [Fact]
        public async Task 新增附件_既有ItemNo_執行UPSERT更新()
        {
            await _svc.CreateAttFileAsync(DtoBuilder.CreateAttFile("ATT002", 1));
            var updated = DtoBuilder.CreateAttFile("ATT002", 1);
            updated.FileName = "updated.xml";

            await _svc.CreateAttFileAsync(updated);

            Assert.Equal("updated.xml", _repo.AttFiles[("ATT002", 1m)].FileName);
            Assert.Single(_repo.AttFiles); // 沒有新增第二筆
        }

        [Fact]
        public async Task 新增附件_不同ItemNo_各自獨立()
        {
            await _svc.CreateAttFileAsync(DtoBuilder.CreateAttFile("ATT003", 1));
            await _svc.CreateAttFileAsync(DtoBuilder.CreateAttFile("ATT003", 2));

            Assert.Equal(2, _repo.AttFiles.Count);
        }
    }
}

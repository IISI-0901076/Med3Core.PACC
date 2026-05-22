using Med3Core.PACC.Application.Models.Contracts.Requests;
using Med3Core.PACC.Application.Services;
using Med3Core.PACC.Domain.Entities;
using Med3Core.PACC.Test.Fakes;
using Med3Core.PACC.Test.TestUtilities;
using Xunit;

namespace Med3Core.PACC.Test.Services
{
    public class PaccService_QueryCaseFilesTests
    {
        private readonly FakePaccRepository _repo = new();
        private readonly PaccService _svc;

        public PaccService_QueryCaseFilesTests()
        {
            _svc = new PaccService(_repo);
        }

        [Fact]
        public async Task 查詢附件_有資料_回傳清單()
        {
            _repo.AttFiles[("QCF001", 1m)] = new IpliExmAttFile
            {
                CaseSeqNo = "QCF001", ItemNo = 1, FileName = "a.xml", FileType = "XML",
                FileObjectKey = "IPL/0401/QCF001/a.xml"
            };
            _repo.AttFiles[("QCF001", 2m)] = new IpliExmAttFile
            {
                CaseSeqNo = "QCF001", ItemNo = 2, FileName = "b.dcf", FileType = "DCF",
                FileAttach = "DCF"
            };

            var result = await _svc.QueryCaseFilesAsync(new QueryCaseFilesRequestDto { CaseSeqNo = "QCF001" });

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task 查詢附件_篩選FileAttach_僅回傳符合()
        {
            _repo.AttFiles[("QCF002", 1m)] = new IpliExmAttFile
            {
                CaseSeqNo = "QCF002", ItemNo = 1, FileAttach = "XML"
            };
            _repo.AttFiles[("QCF002", 2m)] = new IpliExmAttFile
            {
                CaseSeqNo = "QCF002", ItemNo = 2, FileAttach = "DCF"
            };

            var result = await _svc.QueryCaseFilesAsync(new QueryCaseFilesRequestDto { CaseSeqNo = "QCF002", FileAttach = "DCF" });

            Assert.Single(result);
            Assert.Equal(2m, result[0].ItemNo);
        }

        [Fact]
        public async Task 查詢附件_無資料_回傳空集合()
        {
            var result = await _svc.QueryCaseFilesAsync(new QueryCaseFilesRequestDto { CaseSeqNo = "NOPE" });
            Assert.Empty(result);
        }
    }
}

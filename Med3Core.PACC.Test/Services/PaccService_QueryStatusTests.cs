using Med3Core.PACC.Application.Models.Contracts.Requests;
using Med3Core.PACC.Application.Models.Contracts.Responses;
using Med3Core.PACC.Application.Services;
using Med3Core.PACC.Test.Fakes;
using Med3Core.PACC.Test.TestUtilities;
using Xunit;

namespace Med3Core.PACC.Test.Services
{
    public class PaccService_QueryStatusTests
    {
        private readonly FakePaccRepository _repo = new();
        private readonly PaccService _svc;

        public PaccService_QueryStatusTests()
        {
            _svc = new PaccService(_repo);
        }

        [Fact]
        public async Task 查詢案件狀態_指定案件序號_回傳單筆()
        {
            await _svc.CreateCaseAsync(DtoBuilder.CreateCase("Q001"));

            var result = await _svc.QueryStatusAsync(new QueryStatusDto { CaseSeqNo = "Q001", PageNo = 1, PageSize = 10 });

            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Items);
            Assert.Equal("Q001", result.Items[0].CaseSeqNo);
        }

        [Fact]
        public async Task 查詢案件狀態_BusCode篩選_僅回傳符合資料()
        {
            await _svc.CreateCaseAsync(DtoBuilder.CreateCase("Q002", "IPL"));
            await _svc.CreateCaseAsync(DtoBuilder.CreateCase("Q003", "QBJ"));

            var result = await _svc.QueryStatusAsync(new QueryStatusDto { BusCode = "IPL", PageNo = 1, PageSize = 10 });

            Assert.Equal(1, result.TotalCount);
            Assert.Equal("Q002", result.Items[0].CaseSeqNo);
        }

        [Fact]
        public async Task 查詢案件狀態_無符合資料_回傳空集合()
        {
            var result = await _svc.QueryStatusAsync(new QueryStatusDto { CaseSeqNo = "NOPE", PageNo = 1, PageSize = 10 });

            Assert.Equal(0, result.TotalCount);
            Assert.Empty(result.Items);
        }
    }
}

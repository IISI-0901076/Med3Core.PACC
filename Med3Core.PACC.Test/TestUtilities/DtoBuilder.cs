using Med3Core.PACC.Application.Models.Contracts.Requests;

namespace Med3Core.PACC.Test.TestUtilities
{
    /// <summary>建立 DTO 的輔助方法。</summary>
    public static class DtoBuilder
    {
        public static CreateCaseDto CreateCase(string caseSeqNo = "CASE001", string busCode = "IPL") => new()
        {
            CaseSeqNo = caseSeqNo,
            BusCode = busCode,
            HospId = "0401",
            CaseStatus = "0",
            PgmProcStatus = "0",
            RecvSeqNo = "RECV001",
            ReadPos = "1",
        };

        public static CreateRecvLogDto CreateRecvLog(string recvSeqNo = "RECV001", string busCode = "IPL") => new()
        {
            RecvSeqNo = recvSeqNo,
            BusCode = busCode,
            HospId = "0401",
            FileType = "XML",
            RecvMode = "FTP",
            ReadPos = "1",
        };

        public static CreateAttFileDto CreateAttFile(string caseSeqNo = "CASE001", decimal itemNo = 1) => new()
        {
            CaseSeqNo = caseSeqNo,
            ItemNo = itemNo,
            FileName = "test.xml",
            FileType = "XML",
            FileObjectKey = $"IPL/0401/{caseSeqNo}/test.xml",
        };
    }
}

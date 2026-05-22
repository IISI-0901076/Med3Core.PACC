using Med3Core.PACC.Application.StateMachine;
using Xunit;

namespace Med3Core.PACC.Test.StateMachine
{
    public class CaseStateMachineTests
    {
        [Theory]
        [InlineData("0", "1", true)]
        [InlineData("1", "2", true)]
        [InlineData("1", "F", true)]
        [InlineData("2", "F", true)]
        [InlineData("0", "2", false)]
        [InlineData("0", "F", false)]
        [InlineData("F", "0", false)]
        [InlineData("2", "1", false)]
        public void CaseStatus轉換_驗證白名單(string from, string to, bool expected)
        {
            Assert.Equal(expected, CaseStateMachine.IsValidCaseStatusTransition(from, to));
        }

        [Theory]
        [InlineData("0", "9", true)]
        [InlineData("9", "1", true)]
        [InlineData("9", "2", true)]
        [InlineData("0", "1", false)]
        [InlineData("1", "0", false)]
        public void PgmProcStatus轉換_驗證白名單(string from, string to, bool expected)
        {
            Assert.Equal(expected, CaseStateMachine.IsValidPgmProcStatusTransition(from, to));
        }

        [Theory]
        [InlineData("0", "9", true)]
        [InlineData("9", "1", true)]
        [InlineData("9", "2", true)]
        [InlineData("9", "3", true)]
        [InlineData("0", "1", false)]
        [InlineData("3", "0", false)]
        public void FileSendStatus轉換_驗證白名單(string from, string to, bool expected)
        {
            Assert.Equal(expected, CaseStateMachine.IsValidFileSendStatusTransition(from, to));
        }
    }
}

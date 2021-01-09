using Assignment;
using Xunit;
using static Assignment.Enums;
using static Assignment.Program;

namespace XUnitTestProject
{
    public class UnitTest1
    {
        [Fact]
        public void TestWrongDirectory()
        {
            Assert.False(Program.ChangeDirectory("Some dummy path"));
        }

        [Fact]
        public void TestRightDirectory()
        {
            Assert.True(Program.ChangeDirectory("c:\\"));
        }

        [Fact]
        public void TestReadConfigFilePass()
        {
            Assert.Equal(ReadConfigStatus.OK, ReadConfigSettings("Config.txt"));
        }

        [Fact]
        public void TestReadConfigFileFail()
        {
            Assert.Equal(ReadConfigStatus.CONFIG_FILE_NOT_FOUND, ReadConfigSettings("Config1.txt"));
        }
    }
}

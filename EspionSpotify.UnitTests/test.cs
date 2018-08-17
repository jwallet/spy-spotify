using Xunit;

namespace EspionSpotify.UnitTests
{
    public class Test
    {
        [Fact]
        public void Test_ShouldDoAsExpected()
        {
            var r = (8 + 2) / 2.0;
            Assert.Equal(5, r, 0);
        }
    }
}

using DiploHex.Common;

namespace DiploHex.Tests
{
    public class UtilsTests
    {
        [Fact]
        public void GetHexColor_ValidInput_ReturnsCorrectHexColor()
        {
            // Arrange
            int r = 255;
            int g = 0;
            int b = 0;
            string expectedHexColor = "#FF0000";

            // Act
            string actualHexColor = Utils.GetHexColor(r, g, b);

            // Assert
            Assert.Equal(expectedHexColor, actualHexColor);
        }

        [Fact]
        public void GetHexColor_ZeroInput_ReturnsBlack()
        {
            // Arrange
            int r = 0;
            int g = 0;
            int b = 0;
            string expectedHexColor = "#000000";

            // Act
            string actualHexColor = Utils.GetHexColor(r, g, b);

            // Assert
            Assert.Equal(expectedHexColor, actualHexColor);
        }
    }
}

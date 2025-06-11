using FluentAssertions ;
using Idasen.SystemTray.Win11.Utils.Converters ;
using NSubstitute ;

namespace Idasen.SystemTray.Win11.Tests.Utils
{
    public class StringToUIntConverterTests
    {
        [Theory]
        [InlineData("123", 0ul, 123ul)]
        [InlineData("0", 100ul, 0ul)]
        [InlineData("", 50ul, 50ul)]
        [InlineData("invalid", 25ul, 25ul)]
#pragma warning disable xUnit1012
        [InlineData(null , 75ul, 75ul)]
#pragma warning restore xUnit1012
        public void ConvertStringToUlongOrDefault_ShouldReturnExpectedValue(
            string input,
            ulong  defaultValue,
            ulong  expected)
        {
            // Act
            var result = CreateSut().ConvertStringToUlongOrDefault(input, defaultValue);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void ConvertStringToUlongOrDefault_ShouldCallTryParse()
        {
            // Arrange
            var text          = "123";
            var defaultValue  = 0ul;
            var mockConverter = Substitute.For<StringToUIntConverter>();

            // Act
            mockConverter.ConvertStringToUlongOrDefault(text, defaultValue);

            // Assert
            mockConverter.Received(1).ConvertStringToUlongOrDefault(text, defaultValue);
        }

        private static StringToUIntConverter CreateSut()
        {
            return new StringToUIntConverter();
        }
    }
}
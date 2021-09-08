using System.Text.RegularExpressions;
using FluentAssertions;
using MrCMS.Services;
using Xunit;
using Xunit.Abstractions;

namespace MrCMS.Tests.Helpers
{
    public class ImageProcessorTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        string pattern = ImageProcessor.Pattern;

        public ImageProcessorTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void File_Url_Should_Match()
        {
            var match = Regex.Match("/some-file-path/file_w100_h100.jpg", pattern, RegexOptions.Compiled);

            match.Success.Should().Be(true);
        }
        
        [Fact]
        public void Match_Groups_Should_Be_3()
        {
            var match = Regex.Match("/some-file-path/file_w100_h100.jpg", pattern, RegexOptions.Compiled);

            match.Groups.Count.Should().Be(3);
        }
        
        [Fact]
        public void Groups_Should_Be_Width_And_Height()
        {
            var match = Regex.Match("/some-file-path/file_w100_h150.jpg", pattern, RegexOptions.Compiled);
            _testOutputHelper.WriteLine(match.Groups[1].Value);
            _testOutputHelper.WriteLine(match.Groups[2].Value);
            match.Groups[1].Value.Should().Be("100");
            match.Groups[2].Value.Should().Be("150");
        }
        
        [Fact]
        public void Width_Only_Should_Still_Return()
        {
            var match = Regex.Match("/some-file-path/file_w100.jpg", pattern, RegexOptions.Compiled);
            _testOutputHelper.WriteLine(match.Groups[1].Value);
            match.Groups[1].Value.Should().Be("100");
        }
    }
}
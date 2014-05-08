using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MrCMS.Helpers;
using Xunit;

namespace MrCMS.Tests.Helpers
{
    public class StringHelperTests
    {
        const string testShortString = "my short string";
        const string testLongString = "my long stringmy long stringmy long stringmy long string my long string my long string my long string my long stringmy long string";
        private const string htmlString = "<p>hello world</p>";

        [Fact]
        public void TruncateString_LengthLessThanRequestedLengthShouldRemainTheSame()
        {
            var truncatedString = testShortString.TruncateString(100);
            truncatedString.Length.Should().Be(testShortString.Length);
        }

        [Fact]
        public void TruncateString_EmptyStringShouldReturnEmptyString()
        {
            var truncatedString = string.Empty.TruncateString(100);
            truncatedString.Length.Should().Be(0);
        }

        [Fact]
        public void TruncateString_LongStringShouldReturnRequestedTruncatedLength()
        {
            var truncatedString = testLongString.TruncateString(50, "");
            truncatedString.Length.Should().Be(50);
        }

        [Fact]
        public void TruncateString_LongStringShouldReturnRequestedTruncatedLengthAndDotDotDotIfNoParamEntered()
        {
            var truncatedString = testLongString.TruncateString(50);
            truncatedString.Length.Should().Be(53); //add 3 for ...
            truncatedString.Should().EndWith("...");
        }

        [Fact]
        public void TruncateString_LongStringShouldReturnRequestedTruncatedLengthAndSpecifiedTrailingWord()
        {
            var truncatedString = testLongString.TruncateString(50, "test");
            truncatedString.Length.Should().Be(54); //add 4 for 'test'
            truncatedString.Should().EndWith("test");
        }

        [Fact]
        public void StripHtml_ShouldReturnEmptyStringIfEmptyStringIsPassed()
        {
            var strippedSting = string.Empty.StripHtml();
            strippedSting.Should().Be(string.Empty);
        }

        [Fact]
        public void StripHtml_ShouldReturnStringWithoutHtml()
        {
            var strippedSting = htmlString.StripHtml("");
            strippedSting.Should().EndWith("hello world");
        }
    }
}

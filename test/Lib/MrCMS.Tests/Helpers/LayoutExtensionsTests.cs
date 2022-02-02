using System.Collections.Generic;
using FluentAssertions;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Helpers;
using Xunit;

namespace MrCMS.Tests.Helpers
{
    public class LayoutExtensionsTests
    {
        public static IEnumerable<object[]> GetLayoutNameInputOutputValues => new List<object[]>
        {

            new object[] {"test", "_test"},
            new object[] {"test   ", "_test"},
            new object[] {"TEST", "_TEST"},
            new object[] {"Split Word Test", "_SplitWordTest"},
            new object[] {"Allow 2 Numbers", "_Allow2Numbers"},
            new object[] {"Remove-Dashes", "_RemoveDashes"}
        };

        [Theory, MemberData(nameof(GetLayoutNameInputOutputValues))]
        public void GetLayoutName_BehavesAsExpectedForConventions(string input, string result)
        {
            new Layout {Name = input}.GetLayoutName().Should().Be(result);
        }

        [Fact]
        public void GetLayoutName_IfUrlSegmentIsSetThatIsUsed()
        {
            new Layout {Name = "Layout", Path = "_LayoutOverride"}.GetLayoutName().Should().Be("_LayoutOverride");
        }
    }
}
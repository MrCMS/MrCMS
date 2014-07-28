using System.Collections.Generic;
using FluentAssertions;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Helpers;
using Xunit;
using Xunit.Extensions;

namespace MrCMS.Tests.Helpers
{
    public class LayoutExtensionsTests
    {
        public static IEnumerable<object[]> GetLayoutNameInputOutputValues
        {
            get
            {
                yield return new[] {"test", "_test"};
                yield return new[] {"test   ", "_test"};
                yield return new[] {"TEST", "_TEST"};
                yield return new[] {"Split Word Test", "_SplitWordTest"};
                yield return new[] {"Allow 2 Numbers", "_Allow2Numbers"};
                yield return new[] {"Remove-Dashes", "_RemoveDashes"};
            }
        }

        [Theory, PropertyData("GetLayoutNameInputOutputValues")]
        public void GetLayoutName_BehavesAsExpectedForConventions(string input, string result)
        {
            new Layout {Name = input}.GetLayoutName().Should().Be(result);
        }

        [Fact]
        public void GetLayoutName_IfUrlSegmentIsSetThatIsUsed()
        {
            new Layout {Name = "Layout", UrlSegment = "_LayoutOverride"}.GetLayoutName().Should().Be("_LayoutOverride");
        }
    }
}
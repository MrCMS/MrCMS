using System.Collections.Generic;
using FluentAssertions;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Helpers;
using Xunit.Extensions;

namespace MrCMS.Tests.Helpers
{
    public class LayoutExtensionsTests
    {
        public static IEnumerable<object[]> GetLayoutNameInputOutputValues
        {
            get
            {
                yield return new[] {"test", "test"};
                yield return new[] {"test   ", "test"};
                yield return new[] {"TEST", "TEST"};
                yield return new[] {"Split Word Test", "SplitWordTest"};
                yield return new[] {"Allow 2 Numbers", "Allow2Numbers"};
                yield return new[] {"Remove-Dashes", "RemoveDashes"};
            }
        }

        [Theory, PropertyData("GetLayoutNameInputOutputValues")]
        public void GetLayoutName_BehavesAsExpected(string input, string result)
        {
            new Layout {Name = input}.GetLayoutName().Should().Be(result);
        }
    }
}
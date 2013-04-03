using System.Collections.Generic;
using System.Drawing;
using FluentAssertions;
using MrCMS.Services;
using Xunit;
using Xunit.Extensions;

namespace MrCMS.Tests.Services
{
    public class ImageProcessorTests 
    {
        [Fact]
        public void ImageProcessor_RequiresResize_IfTargetSizeIsEqualToOriginalSizeReturnFalse()
        {
            ImageProcessor.RequiresResize(new Size(20, 20), new Size(20, 20)).Should().BeFalse();
        }

        [Fact]
        public void ImageProcessor_RequiresResize_IfTargetSizeWidthIsSmallerThanOriginalWidthReturnTrue()
        {
            ImageProcessor.RequiresResize(new Size(20, 20), new Size(19, 20)).Should().BeTrue();
        }

        [Fact]
        public void ImageProcessor_RequiresResize_IfTargetSizeHeightIsSmallerThanOriginalWidthReturnTrue()
        {
            ImageProcessor.RequiresResize(new Size(20, 20), new Size(20, 19)).Should().BeTrue();
        }

        [Theory, PropertyData("ImageProcessingValues")]
        public void ImageProcessor_CalculateDimensions_ShouldResizeToLongestSide(Size from, Size to, Size expected)
        {
            var size = ImageProcessor.CalculateDimensions(from, to);

            size.Should().Be(expected);
        }

        public static IEnumerable<object[]> ImageProcessingValues
        {
            get
            {
                yield return new object[] { new Size(1000, 1000), new Size(500, 500), new Size(500, 500) };
                yield return new object[] { new Size(1000, 1000), new Size(500, 250), new Size(250, 250) };
                yield return new object[] { new Size(1000, 1000), new Size(250, 500), new Size(250, 250) };
                yield return new object[] { new Size(620, 297), new Size(300, 300), new Size(300, 144) };
                yield return new object[] { new Size(300, 200), new Size(100, 100), new Size(100, 67) };
                yield return new object[] { new Size(200, 300), new Size(100, 100), new Size(67, 100) };
                yield return new object[] { new Size(270, 337), new Size(320, 240), new Size(193, 240) };
                yield return new object[] { new Size(298, 350), new Size(248, 0), new Size(248, 292) };
            }
        }
    }
}
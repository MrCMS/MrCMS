using System.Drawing;
using FluentAssertions;
using MrCMS.Services;
using Xunit;

namespace MrCMS.Tests.Services
{
    public class ImageProcessorTests : InMemoryDatabaseTest
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
    }
}
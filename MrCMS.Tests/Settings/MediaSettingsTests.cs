using System.Collections.Generic;
using System.Drawing;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Settings;
using NHibernate;
using Xunit;

namespace MrCMS.Tests.Settings
{
    public class MediaSettingsTests
    {
        private readonly MediaSettings _mediaSettings;

        public MediaSettingsTests()
        {
            _mediaSettings = new MediaSettings
                                    {
                                        ThumbnailImageHeight = 64,
                                        ThumbnailImageWidth = 64,
                                        SmallImageHeight = 150,
                                        SmallImageWidth = 200,
                                        MediumImageHeight = 240,
                                        MediumImageWidth = 320,
                                        LargeImageHeight = 480,
                                        LargeImageWidth = 640,
                                        MaxImageSizeHeight = 1024,
                                        MaxImageSizeWidth = 1024
                                    };
        }
        
        [Fact]
        public void MediaSettings_GetTypeName_ReturnsMediaSettings()
        {
            _mediaSettings.TypeName.Should().Be("Media Settings");
        }

        [Fact]
        public void MediaSettings_GetDivId_ReturnsMediaDashSettings()
        {
            _mediaSettings.DivId.Should().Be("media-settings");
        }

        [Fact]
        public void MediaSettings_ThumbnailSize_ShouldBeCalculatedFromTheSettings()
        {
            _mediaSettings.ThumbnailSize.Should().Be(new Size(64, 64));
        }

        [Fact]
        public void MediaSettings_SmallSize_ShouldBeCalculatedFromTheSettings()
        {
            _mediaSettings.SmallSize.Should().Be(new Size(200, 150));
        }

        [Fact]
        public void MediaSettings_MediumSize_ShouldBeCalculatedFromTheSettings()
        {
            _mediaSettings.MediumSize.Should().Be(new Size(320, 240));
        }

        [Fact]
        public void MediaSettings_LargeSize_ShouldBeCalculatedFromTheSettings()
        {
            _mediaSettings.LargeSize.Should().Be(new Size(640, 480));
        }

        [Fact]
        public void MediaSettings_MaxSize_ShouldBeCalculatedFromTheSettings()
        {
            _mediaSettings.MaxSize.Should().Be(new Size(1024, 1024));
        }

        [Fact]
        public void MediaSettings_Sizes_ShouldContainTheCalculatedSizes()
        {
            _mediaSettings.Sizes.Should()
                         .ContainInOrder(new List<Size>
                                             {
                                                 _mediaSettings.LargeSize,
                                                 _mediaSettings.MediumSize,
                                                 _mediaSettings.SmallSize,
                                                 _mediaSettings.ThumbnailSize
                                             });
        }

        [Fact]
        public void MediaSettings_SetViewData_ShouldNotThrow()
        {
            this.Invoking(tests =>
                          _mediaSettings.SetViewData(A.Fake<ISession>(), A.Fake<ViewDataDictionary>())).ShouldNotThrow();
        }
    }
}
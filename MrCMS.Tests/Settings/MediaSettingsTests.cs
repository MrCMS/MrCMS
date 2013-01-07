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
        [Fact]
        public void MediaSettings_GetTypeName_ReturnsMediaSettings()
        {
            var mediaSettings = GetMediaSettings();

            mediaSettings.TypeName.Should().Be("Media Settings");
        }

        [Fact]
        public void MediaSettings_GetDivId_ReturnsMediaDashSettings()
        {
            var mediaSettings = GetMediaSettings();

            mediaSettings.DivId.Should().Be("media-settings");
        }

        [Fact]
        public void MediaSettings_ThumbnailSize_ShouldBeCalculatedFromTheSettings()
        {
            var mediaSettings = GetMediaSettings();

            mediaSettings.ThumbnailSize.Should().Be(new Size(64, 64));
        }

        [Fact]
        public void MediaSettings_SmallSize_ShouldBeCalculatedFromTheSettings()
        {
            var mediaSettings = GetMediaSettings();

            mediaSettings.SmallSize.Should().Be(new Size(200, 150));
        }

        [Fact]
        public void MediaSettings_MediumSize_ShouldBeCalculatedFromTheSettings()
        {
            var mediaSettings = GetMediaSettings();

            mediaSettings.MediumSize.Should().Be(new Size(320, 240));
        }

        [Fact]
        public void MediaSettings_LargeSize_ShouldBeCalculatedFromTheSettings()
        {
            var mediaSettings = GetMediaSettings();

            mediaSettings.LargeSize.Should().Be(new Size(640, 480));
        }

        [Fact]
        public void MediaSettings_Sizes_ShouldContainTheCalculatedSizes()
        {
            var mediaSettings = GetMediaSettings();

            mediaSettings.Sizes.Should()
                         .ContainInOrder(new List<Size>
                                             {
                                                 mediaSettings.LargeSize,
                                                 mediaSettings.MediumSize,
                                                 mediaSettings.SmallSize,
                                                 mediaSettings.ThumbnailSize
                                             });
        }

        [Fact]
        public void MediaSettings_SetViewData_ShouldNotThrow()
        {
            var mediaSettings = GetMediaSettings();
            this.Invoking(tests =>
                          mediaSettings.SetViewData(A.Fake<ISession>(), A.Fake<ViewDataDictionary>())).ShouldNotThrow();
        }

        private static MediaSettings GetMediaSettings()
        {
            var mediaSettings = new MediaSettings
                                    {
                                        ThumbnailImageHeight = 64,
                                        ThumbnailImageWidth = 64,
                                        SmallImageHeight = 150,
                                        SmallImageWidth = 200,
                                        MediumImageHeight = 240,
                                        MediumImageWidth = 320,
                                        LargeImageHeight = 480,
                                        LargeImageWidth = 640
                                    };
            return mediaSettings;
        }
    }
}
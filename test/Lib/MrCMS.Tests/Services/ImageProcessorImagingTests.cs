using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Entities.Documents.Media;
using MrCMS.Services;
using MrCMS.Settings;
using NHibernate;
using SkiaSharp;
using Xunit;

namespace MrCMS.Tests.Services
{
    public class ImageProcessorImagingTests
    {
        private readonly IFileSystem _fileSystem = A.Fake<IFileSystem>();
        private readonly ImageProcessor _processor;
        private byte[] _saved;

        public ImageProcessorImagingTests()
        {
            var factory = A.Fake<IFileSystemFactory>();
            A.CallTo(() => factory.GetForCurrentSite()).Returns(_fileSystem);
            A.CallTo(() => _fileSystem.SaveFile(A<Stream>._, A<string>._, A<string>._))
                .Invokes((Stream stream, string _, string _) =>
                {
                    using var copy = new MemoryStream();
                    stream.CopyTo(copy);
                    _saved = copy.ToArray();
                })
                .Returns("saved");
            _processor = new ImageProcessor(A.Fake<ISession>(), factory);
        }

        private static byte[] CreateImage(int width, int height, SKEncodedImageFormat format)
        {
            using var bitmap = new SKBitmap(width, height);
            // left half red, right half blue so crops can be checked
            using (var canvas = new SKCanvas(bitmap))
            {
                canvas.Clear(SKColors.Red);
                using var paint = new SKPaint { Color = SKColors.Blue };
                canvas.DrawRect(width / 2f, 0, width / 2f, height, paint);
            }

            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(format, 90);
            return data.ToArray();
        }

        private static SKBitmap Decode(byte[] bytes) => SKBitmap.Decode(bytes);

        [Fact]
        public void SetFileDimensions_ReadsWidthAndHeight()
        {
            var file = new MediaFile();
            using var stream = new MemoryStream(CreateImage(320, 240, SKEncodedImageFormat.Png));

            _processor.SetFileDimensions(file, stream);

            file.Width.Should().Be(320);
            file.Height.Should().Be(240);
            stream.Position.Should().Be(0);
        }

        [Theory]
        [InlineData(SKEncodedImageFormat.Jpeg)]
        [InlineData(SKEncodedImageFormat.Png)]
        [InlineData(SKEncodedImageFormat.Webp)]
        public async Task SaveResizedImage_KeepsAspectRatioAndFormat(SKEncodedImageFormat format)
        {
            var file = new MediaFile { Width = 400, Height = 200, ContentType = "image/test" };

            await _processor.SaveResizedImage(file, new Size(100, 0), CreateImage(400, 200, format), "/a_w100.img");

            using var codec = SKCodec.Create(new MemoryStream(_saved));
            codec.EncodedFormat.Should().Be(format);
            codec.Info.Width.Should().Be(100);
            codec.Info.Height.Should().Be(50);
        }

        [Fact]
        public async Task SaveCrop_UsesTheCropsXAndY()
        {
            var bytes = CreateImage(200, 100, SKEncodedImageFormat.Png);
            var file = new MediaFile { Width = 200, Height = 100, ContentType = "image/png" };
            var cropType = new CropType { Width = 200, Height = 100 };

            // the right-hand (blue) half, offset vertically
            await _processor.SaveCrop(file, cropType, new Rectangle(100, 20, 100, 50), bytes, "/a_c1.png");

            using var result = Decode(_saved);
            result.Width.Should().Be(100);
            result.Height.Should().Be(50);
            result.GetPixel(50, 25).Should().Be(SKColors.Blue);
        }

        [Fact]
        public void EnforceMaxSize_ShrinksLargeImages()
        {
            var file = new MediaFile();
            Stream stream = new MemoryStream(CreateImage(2400, 1200, SKEncodedImageFormat.Jpeg));
            var settings = new MediaSettings { EnforceMaxImageSize = true, MaxImageSizeWidth = 1200, MaxImageSizeHeight = 1200 };

            _processor.EnforceMaxSize(ref stream, file, settings);

            file.Width.Should().Be(1200);
            file.Height.Should().Be(600);
            file.ContentLength.Should().Be(stream.Length);
            using var codec = SKCodec.Create(stream);
            codec.Info.Width.Should().Be(1200);
            codec.EncodedFormat.Should().Be(SKEncodedImageFormat.Jpeg);
        }

        [Fact]
        public async Task UnsupportedEncodeFormat_SavesTheOriginalBytes()
        {
            var gif = CreateGif();
            var file = new MediaFile { Width = 2, Height = 2, ContentType = "image/gif" };

            await _processor.SaveResizedImage(file, new Size(1, 1), gif, "/a_w1.gif");

            _saved.Should().Equal(gif);
        }

        // minimal 1x1 GIF (SkiaSharp can decode GIF but not encode it)
        private static byte[] CreateGif() =>
        [
            0x47, 0x49, 0x46, 0x38, 0x39, 0x61, 0x01, 0x00, 0x01, 0x00, 0x80, 0x00, 0x00, 0xFF, 0x00, 0x00,
            0x00, 0x00, 0xFF, 0x21, 0xF9, 0x04, 0x01, 0x00, 0x00, 0x00, 0x00, 0x2C, 0x00, 0x00, 0x00, 0x00,
            0x01, 0x00, 0x01, 0x00, 0x00, 0x02, 0x02, 0x44, 0x01, 0x00, 0x3B
        ];
    }
}

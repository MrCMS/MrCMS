using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Services
{
    public class CropService : ICropService
    {
        private readonly ISession _session;
        private readonly IImageProcessor _imageProcessor;
        private readonly IFileSystemFactory _fileSystemFactory;

        public CropService(ISession session, IImageProcessor imageProcessor, IFileSystemFactory fileSystemFactory)
        {
            _session = session;
            _imageProcessor = imageProcessor;
            _fileSystemFactory = fileSystemFactory;
        }

        public async Task<Crop> CreateCrop(MediaFile file, CropType cropType, Rectangle details)
        {
            if (!file.IsImage())
                throw new ArgumentException("file is not an image");

            var existing = await _session.QueryOver<Crop>()
                .Where(c => c.MediaFile.Id == file.Id && c.CropType.Id == cropType.Id)
                .Cacheable()
                .ListAsync();
            if (existing.Any())
                return existing.First();


            var filePath = file.FileUrl;
            var fileSystem = _fileSystemFactory.GetForCurrentSite();
            if (!await fileSystem.Exists(filePath))
                throw new Exception($"Cannot find {filePath} on filesystem");
            var bytes = await fileSystem.ReadAllBytes(filePath);
            var cropUrl = ImageProcessor.RequestedCropUrl(file, cropType);
            await _imageProcessor.SaveCrop(file, cropType, details, bytes, cropUrl);
            var crop = new Crop
            {
                MediaFile = file,
                Bottom = details.Bottom,
                CropType = cropType,
                Left = details.Left,
                Right = details.Right,
                Top = details.Top,
                Url = cropUrl
            };
            await _session.TransactAsync(session => session.SaveAsync(crop));
            return crop;
        }
    }
}
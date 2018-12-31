using System;
using System.Drawing;
using System.Linq;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Services
{
    public class CropService : ICropService
    {
        private readonly ISession _session;
        private readonly IImageProcessor _imageProcessor;
        private readonly IFileSystem _fileSystem;

        public CropService(ISession session, IImageProcessor imageProcessor, IFileSystem fileSystem)
        {
            _session = session;
            _imageProcessor = imageProcessor;
            _fileSystem = fileSystem;
        }

        public Crop CreateCrop(MediaFile file, CropType cropType, Rectangle details)
        {
            if (!file.IsImage())
                throw new ArgumentException("file is not an image");

            var existing =
                _session.QueryOver<Crop>()
                    .Where(c => c.MediaFile.Id == file.Id && c.CropType.Id == cropType.Id)
                    .Cacheable()
                    .List();
            if (existing.Any())
                return existing.First();


            var filePath = file.FileUrl;
            if (!_fileSystem.Exists(filePath))
                throw new Exception(string.Format("Cannot find {0} on filesystem", filePath));
            var bytes = _fileSystem.ReadAllBytes(filePath);
            var cropUrl = ImageProcessor.RequestedCropUrl(file, cropType);
            _imageProcessor.SaveCrop(file, cropType, details, bytes, cropUrl);
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
            _session.Transact(session => session.Save(crop));
            return crop;
        }
    }
}
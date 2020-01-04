using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;

namespace MrCMS.Services
{
    public class CropService : ICropService
    {
        private readonly IRepository<Crop> _repository;
        private readonly IImageProcessor _imageProcessor;
        private readonly IFileSystem _fileSystem;

        public CropService(IRepository<Crop> repository, IImageProcessor imageProcessor, IFileSystem fileSystem)
        {
            _repository = repository;
            _imageProcessor = imageProcessor;
            _fileSystem = fileSystem;
        }

        public async Task<Crop> CreateCrop(MediaFile file, CropType cropType, Rectangle details)
        {
            if (!file.IsImage())
                throw new ArgumentException("file is not an image");


            var existing = await _repository.Query().FirstOrDefaultAsync(c => c.MediaFile.Id == file.Id && c.CropType.Id == cropType.Id);
            if (existing != null)
                return existing;


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
            await _repository.Add(crop);
            return crop;
        }
    }
}
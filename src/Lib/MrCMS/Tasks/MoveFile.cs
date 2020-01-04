using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;
using MrCMS.Services;
using Newtonsoft.Json;

namespace MrCMS.Tasks
{
    public class MoveFile : AdHocTask
    {
        private readonly IRepository<MediaFile> _mediaFileRepository;
        private readonly IRepository<ResizedImage> _resizedImageRepository;
        private readonly IEnumerable<IFileSystem> _fileSystems;

        public MoveFile(IServiceProvider kernel, IRepository<MediaFile> mediaFileRepository, IRepository<ResizedImage> resizedImageRepository)
        {
            _mediaFileRepository = mediaFileRepository;
            _resizedImageRepository = resizedImageRepository;
            _fileSystems =
                TypeHelper.GetAllTypesAssignableFrom<IFileSystem>()
                    .Select(type => kernel.GetService(type) as IFileSystem)
                    .ToList();
        }

        public override int Priority => 0;

        private MoveFileData FileData { get; set; }

        protected override async Task OnExecute(CancellationToken token)
        {
            await _mediaFileRepository.Transact(async (repo, ct) =>
             {
                 var file = await repo.Load(FileData.FileId, ct);
                 var from = _fileSystems.FirstOrDefault(system => system.GetType().FullName == FileData.From);
                 var to = _fileSystems.FirstOrDefault(system => system.GetType().FullName == FileData.To);

                 // remove resized images (they will be regenerated on the to system)
                 foreach (var resizedImage in await _resizedImageRepository.Query().Where(x => x.MediaFileId == file.Id).ToListAsync(ct))
                 {
                     // check for resized file having same url as the original - 
                     // do not delete from disc yet in that case, or else it will cause an error when copying
                     if (resizedImage.Url != file.FileUrl) from.Delete(resizedImage.Url);
                     file.ResizedImages.Remove(resizedImage);
                     await _resizedImageRepository.Delete(resizedImage, ct);
                 }

                 var existingUrl = file.FileUrl;
                 await using (var readStream = from.GetReadStream(existingUrl))
                 {
                     file.FileUrl = to.SaveFile(readStream, GetNewFilePath(file),
                         file.ContentType);
                 }

                 from.Delete(existingUrl);

                 await repo.Update(file, ct);
             }, token);
        }

        private string GetNewFilePath(MediaFile file)
        {
            var fileUrl = file.FileUrl;
            var id = file.Site.Id;
            var indexOf = file.FileUrl.IndexOf(string.Format("/{0}/", id), StringComparison.OrdinalIgnoreCase);
            var newFilePath = fileUrl.Substring(indexOf + 1);
            return newFilePath;
        }

        public override string GetData()
        {
            return JsonConvert.SerializeObject(FileData);
        }

        public override void SetData(string data)
        {
            FileData = JsonConvert.DeserializeObject<MoveFileData>(data);
        }
    }
}
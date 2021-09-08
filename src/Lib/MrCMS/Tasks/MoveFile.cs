using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;
using MrCMS.Services;
using Newtonsoft.Json;
using NHibernate;

namespace MrCMS.Tasks
{
    public class MoveFile : AdHocTask
    {
        private readonly IEnumerable<IFileSystem> _fileSystems;
        private readonly ISession _session;

        public MoveFile(IServiceProvider kernel, ISession session)
        {
            _fileSystems =
                TypeHelper.GetAllTypesAssignableFrom<IFileSystem>()
                    .Select(type => kernel.GetService(type) as IFileSystem)
                    .ToList();
            _session = session;
        }

        public override int Priority => 0;

        private MoveFileData FileData { get; set; }

        protected override async Task OnExecute()
        {
            await _session.TransactAsync(async (session, token) =>
            {
                var file = _session.Get<MediaFile>(FileData.FileId);
                var from = _fileSystems.FirstOrDefault(system => system.GetType().FullName == FileData.From);
                var to = _fileSystems.FirstOrDefault(system => system.GetType().FullName == FileData.To);

                // remove resized images (they will be regenerated on the to system)
                foreach (var resizedImage in file.ResizedImages.ToList())
                {
                    // check for resized file having same url as the original - 
                    // do not delete from disc yet in that case, or else it will cause an error when copying
                    if (resizedImage.Url != file.FileUrl) await @from.Delete(resizedImage.Url);
                    file.ResizedImages.Remove(resizedImage);
                    await session.DeleteAsync(resizedImage, token);
                }

                var existingUrl = file.FileUrl;
                await using (var readStream = await from.GetReadStream(existingUrl))
                {
                    file.FileUrl = await to.SaveFile(readStream, GetNewFilePath(file),
                        file.ContentType);
                }

                await @from.Delete(existingUrl);

                await session.UpdateAsync(file, token);
            });
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
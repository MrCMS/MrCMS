using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;
using MrCMS.Services;
using Newtonsoft.Json;
using NHibernate;
using Ninject;

namespace MrCMS.Tasks
{
    public class MoveFile : AdHocTask
    {
        private readonly IEnumerable<IFileSystem> _fileSystems;
        private readonly ISession _session;

        public MoveFile(IKernel kernel, ISession session)
        {
            _fileSystems =
                TypeHelper.GetAllTypesAssignableFrom<IFileSystem>()
                    .Select(type => kernel.Get(type) as IFileSystem)
                    .ToList();
            _session = session;
        }

        public override int Priority
        {
            get { return 0; }
        }
        private MoveFileData FileData { get; set; }

        protected override void OnExecute()
        {
            _session.Transact(session =>
                              {
                                  var file = _session.Get<MediaFile>(FileData.FileId);
                                  var from = _fileSystems.FirstOrDefault(system => system.GetType().FullName == FileData.From);
                                  var to = _fileSystems.FirstOrDefault(system => system.GetType().FullName == FileData.To);

                                  // remove resized images (they will be regenerated on the to system)
                                  foreach (var resizedImage in file.ResizedImages.ToList())
                                  {
                                      // check for resized file having same url as the original - 
                                      // do not delete from disc yet in that case, or else it will cause an error when copying
                                      if (resizedImage.Url != file.FileUrl)
                                      {
                                          from.Delete(resizedImage.Url);
                                      }
                                      file.ResizedImages.Remove(resizedImage);
                                      session.Delete(resizedImage);
                                  }

                                  var existingUrl = file.FileUrl;
                                  using (var readStream = @from.GetReadStream(existingUrl))
                                  {
                                      file.FileUrl = to.SaveFile(readStream, GetNewFilePath(file),
                                          file.ContentType);
                                  }
                                  from.Delete(existingUrl);

                                  session.Update(file);
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
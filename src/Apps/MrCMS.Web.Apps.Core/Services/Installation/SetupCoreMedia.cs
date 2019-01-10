using System.IO;
using System.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;
using MrCMS.Services;
using ISession = NHibernate.ISession;

namespace MrCMS.Web.Apps.Core.Services.Installation
{
    public class SetupCoreMedia : ISetupCoreMedia
    {
        private readonly ISession _session;
        private readonly IFileService _fileService;
        private readonly IFileProvider _fileProvider;

        public SetupCoreMedia(ISession session, IFileService fileService, IFileProvider fileProvider)
        {
            _session = session;
            _fileService = fileService;
            _fileProvider = fileProvider;
        }

        public void Setup()
        {
            _session.Transact(session =>
            {
                var defaultMediaCategory = new MediaCategory
                {
                    Name = "Default",
                    UrlSegment = "default",
                };
                session.Save(defaultMediaCategory);

                string logoPath = ("/images/mrcms-logo.png");
                var fileStream = _fileProvider.GetFileInfo(logoPath).CreateReadStream();
                MediaFile dbFile = _fileService.AddFile(fileStream, Path.GetFileName(logoPath), "image/png",
                    fileStream.Length,
                    defaultMediaCategory);

                string logoPath1 = ("/Images/mrcms-hat.gif");
                var fileStream1 = _fileProvider.GetFileInfo(logoPath).CreateReadStream(); 
                MediaFile dbFile1 = _fileService.AddFile(fileStream1, Path.GetFileName(logoPath1), "image/gif",
                    fileStream1.Length,
                    defaultMediaCategory);
            });
        }
    }
}
using System.IO;
using System.Web;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Settings;
using NHibernate;

namespace MrCMS.Web.Apps.Core.Services.Installation
{
    public class SetupCoreMedia : ISetupCoreMedia
    {
        private readonly ISession _session;
        private readonly IConfigurationProvider _configurationProvider;
        private readonly IFileService _fileService;

        public SetupCoreMedia(ISession session, IConfigurationProvider configurationProvider, IFileService fileService)
        {
            _session = session;
            _configurationProvider = configurationProvider;
            _fileService = fileService;
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
                var mediaSettings = _configurationProvider.GetSiteSettings<MediaSettings>();
                mediaSettings.DefaultCategory = defaultMediaCategory.Id;
                _configurationProvider.SaveSettings(mediaSettings);

                string logoPath = HttpContext.Current.Server.MapPath("/Apps/Core/Content/images/mrcms-logo.png");
                var fileStream = new FileStream(logoPath, FileMode.Open);
                MediaFile dbFile = _fileService.AddFile(fileStream, Path.GetFileName(logoPath), "image/png",
                    fileStream.Length,
                    defaultMediaCategory);

                string logoPath1 = HttpContext.Current.Server.MapPath("/Apps/Core/Content/Images/mrcms-hat.gif");
                var fileStream1 = new FileStream(logoPath1, FileMode.Open);
                MediaFile dbFile1 = _fileService.AddFile(fileStream1, Path.GetFileName(logoPath1), "image/gif",
                    fileStream1.Length,
                    defaultMediaCategory);
            });
        }
    }
}
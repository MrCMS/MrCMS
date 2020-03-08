using System.IO;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using MrCMS.Data;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;
using MrCMS.Installation;
using MrCMS.Services;

namespace MrCMS.Web.Apps.Core.Services.Installation
{
    public class SetupCoreMedia : ISetupCoreMedia
    {
        private readonly IRepository<MediaCategory> _repository;
        private readonly IFileService _fileService;
        private readonly InstallationContentFileProvider _fileProvider;

        public SetupCoreMedia(IFileService fileService, InstallationContentFileProvider fileProvider, IRepository<MediaCategory> repository)
        {
            _fileService = fileService;
            _fileProvider = fileProvider;
            _repository = repository;
        }

        public async Task Setup()
        {
            await _repository.Transact(async (repo, ct) =>
             {
                 var defaultMediaCategory = new MediaCategory
                 {
                     Name = "Default",
                     UrlSegment = "default",
                 };
                 await repo.Add(defaultMediaCategory, ct);

                 string logoPath = ("/images/mrcms-logo.png");
                 var fileStream = _fileProvider.GetFileInfo(logoPath).CreateReadStream();
                 MediaFile dbFile = await _fileService.AddFile(fileStream, Path.GetFileName(logoPath), "image/png",
                     fileStream.Length,
                     defaultMediaCategory);

                 string logoPath1 = ("/Images/mrcms-hat.gif");
                 var fileStream1 = _fileProvider.GetFileInfo(logoPath).CreateReadStream();
                 MediaFile dbFile1 = await _fileService.AddFile(fileStream1, Path.GetFileName(logoPath1), "image/gif",
                     fileStream1.Length,
                     defaultMediaCategory);
             });
        }
    }
}
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Media;
using MrCMS.Events;

namespace MrCMS.Services
{
    public class CreateFolderOnAddMediaCategory : IOnAdded<MediaCategory>
    {
        private readonly IFileService _fileService;

        public CreateFolderOnAddMediaCategory(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task Execute(OnAddedArgs<MediaCategory> args)
        {
            var mediaCategory = args.Item;

            await _fileService.CreateFolder(mediaCategory);
        }
    }
}
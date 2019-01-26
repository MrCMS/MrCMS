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

        public void Execute(OnAddedArgs<MediaCategory> args)
        {
            var mediaCategory = args.Item;

            _fileService.CreateFolder(mediaCategory);
        }
    }
}
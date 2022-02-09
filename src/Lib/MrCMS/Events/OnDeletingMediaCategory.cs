using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Media;
using MrCMS.Services;

namespace MrCMS.Events
{
    public class OnDeletingMediaCategory : IOnDeleting<MediaCategory>
    {
        private readonly IFileService _fileService;

        public OnDeletingMediaCategory(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task Execute(OnDeletingArgs<MediaCategory> args)
        {
            MediaCategory category = args.Item;
            if (category == null)
                return;
            List<MediaFile> mediaFiles = category.Files.ToList();

            foreach (MediaFile mediaFile in mediaFiles)
            {
                await _fileService.DeleteFile(mediaFile.Id);
            }

          await  _fileService.RemoveFolder(category);
        }
    }
}
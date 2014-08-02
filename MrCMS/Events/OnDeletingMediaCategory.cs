using System.Linq;
using MrCMS.DbConfiguration;
using MrCMS.Entities.Documents.Media;
using MrCMS.Services;

namespace MrCMS.Events
{
    public class OnDeletingMediaCategory : IOnDeleting
    {
        private readonly IFileService _fileService;

        public OnDeletingMediaCategory(IFileService fileService)
        {
            _fileService = fileService;
        }

        public void Execute(OnDeletingArgs args)
        {
            var category = args.Item as MediaCategory;
            if (category == null)
                return;
            var mediaFiles = category.Files.ToList();

            foreach (var mediaFile in mediaFiles)
            {
                _fileService.DeleteFile(mediaFile);
            }
            _fileService.RemoveFolder(category);
        }
    }
}
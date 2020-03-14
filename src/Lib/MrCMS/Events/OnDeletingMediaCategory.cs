using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Common;
using MrCMS.Data;
using MrCMS.Entities.Documents.Media;
using MrCMS.Services;

namespace MrCMS.Events
{
    public class OnDeletingMediaCategory : OnDataDeleting<MediaCategory>
    {
        private readonly IFileService _fileService;

        public OnDeletingMediaCategory(IFileService fileService)
        {
            _fileService = fileService;
        }


        public override async Task<IResult> OnDeleting(MediaCategory entity, DbContext dbContext)
        {
            List<MediaFile> mediaFiles = entity.Files.ToList();

            foreach (MediaFile mediaFile in mediaFiles)
            {
                await _fileService.DeleteFile(mediaFile);
            }
            await _fileService.RemoveFolder(entity);
            return await Success;
        }
    }
}
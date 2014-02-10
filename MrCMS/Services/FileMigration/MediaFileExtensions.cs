using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Media;

namespace MrCMS.Services.FileMigration
{
    public static class MediaFileExtensions
    {
        public static IFileSystem GetFileSystem(this MediaFile file, IEnumerable<IFileSystem> possibleFileSystems)
        {
            return possibleFileSystems.FirstOrDefault(system => system.Exists(file.FileUrl));
        }
    }
}
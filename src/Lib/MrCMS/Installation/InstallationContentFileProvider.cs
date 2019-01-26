using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using MrCMS.FileProviders;

namespace MrCMS.Installation
{
    public class InstallationContentFileProvider : IFileProvider
    {
        private const string Installation = "/Installation/Content";
        private readonly CaseInsensitiveEmbeddedFileProvider _internalProvider;

        public InstallationContentFileProvider()
        {
            _internalProvider = new CaseInsensitiveEmbeddedFileProvider(this.GetType().Assembly);
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            var path = TidyPath(subpath, false);
            var fileInfo = _internalProvider.GetFileInfo(Installation + path);
            if (fileInfo is NotFoundFileInfo) // isn't found
            {
                fileInfo = _internalProvider.GetFileInfo(Installation + path);
            }
            return fileInfo;
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            var path = TidyPath(subpath, true);
            var directoryContents = _internalProvider.GetDirectoryContents(Installation + path);
            if (directoryContents is NotFoundDirectoryContents )// isn't found
            {
                directoryContents = _internalProvider.GetDirectoryContents(Installation + path);
            }
            return directoryContents;
        }

        protected string TidyPath(string path, bool isDirectory)
        {
            var segments = path.Split('/', '\\');

            // Loop through the segments of the provided Uri.
            for (int i = 0; i < segments.Length; i++)
            {
                // Find the first occurrence of the dot character.
                var dotPosition = segments[i].IndexOf('.');

                // Check if this segment is a folder segment.
                if (i < segments.Length - 1 || isDirectory) // all directory segments are folder segments
                {
                    // A dash in a folder segment will cause each following dot occurrence to be appended with an underscore.
                    int findPosition;
                    if ((findPosition = segments[i].IndexOf('-')) != -1 && dotPosition != -1)
                    {
                        segments[i] = segments[i].Substring(0, findPosition + 1) + segments[i].Substring(findPosition + 1).Replace(".", "._");
                    }

                    // A dash is replaced with an underscore when no underscores are in the name or a dot occurrence is before it.
                    //if ((findPosition = segments[i].IndexOf('_')) == -1 || (dotPosition >= 0 && dotPosition < findPosition))
                    {
                        segments[i] = segments[i].Replace('-', '_');
                    }
                }
            }

            return string.Join("/", segments);
        }

        public IChangeToken Watch(string filter) => NullChangeToken.Singleton; // can't change - `it's embedded

    }
}
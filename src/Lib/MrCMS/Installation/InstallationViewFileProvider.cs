using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using MrCMS.FileProviders;

namespace MrCMS.Installation
{
    public class InstallationViewFileProvider : IFileProvider
    {
        private readonly string _prefix = "/Installation";
        private readonly CaseInsensitiveEmbeddedFileProvider _internalProvider;
        public InstallationViewFileProvider()
        {
            _internalProvider = new CaseInsensitiveEmbeddedFileProvider(GetType().Assembly);
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            subpath = _prefix + subpath;
            var fileInfo = _internalProvider.GetFileInfo(subpath);
            //if (fileInfo is NotFoundFileInfo // isn't found
            //    && !string.IsNullOrWhiteSpace(_prefix) // and has prefix
            //    && subpath.StartsWith(_prefix, StringComparison.InvariantCultureIgnoreCase)) // and prefix matches
            //{
            //    fileInfo = _internalProvider.GetFileInfo(subpath.Substring(_prefix.Length));
            //}
            return fileInfo;
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            subpath = _prefix + subpath;

            var directoryContents = _internalProvider.GetDirectoryContents(subpath);
            //if (directoryContents is NotFoundDirectoryContents // isn't found
            //    && !string.IsNullOrWhiteSpace(_prefix) // and has prefix
            //    && subpath.StartsWith(_prefix, StringComparison.InvariantCultureIgnoreCase)) // and prefix matches
            //{
            //    directoryContents = _internalProvider.GetDirectoryContents(subpath.Substring(_prefix.Length));
            //}
            return directoryContents;
        }

        public IChangeToken Watch(string filter) => NullChangeToken.Singleton;
    }
}
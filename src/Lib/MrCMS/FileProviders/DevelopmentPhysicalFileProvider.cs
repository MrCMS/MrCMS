using System.IO;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace MrCMS.FileProviders
{
    public class DevelopmentPhysicalFileProvider : IFileProvider
    {
        private readonly string _prefix;
        private readonly PhysicalFileProvider _internalProvider;

        public DevelopmentPhysicalFileProvider(string wwwrootDirectory, string prefix = null)
        {
            _prefix = prefix ?? string.Empty;
            _internalProvider = new PhysicalFileProvider(wwwrootDirectory);
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            if (!subpath.StartsWith(_prefix))
                return new NotFoundFileInfo(Path.GetFileName(subpath));
            return _internalProvider.GetFileInfo(subpath.Substring(_prefix.Length));
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            if (!subpath.StartsWith(_prefix))
                return new NotFoundDirectoryContents();
            return _internalProvider.GetDirectoryContents(subpath.Substring(_prefix.Length));
        }

        public IChangeToken Watch(string filter) => NullChangeToken.Singleton; // can't change - `it's embedded
    }
}
using System;
using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace MrCMS.FileProviders
{
    public class EmbeddedContentFileProvider : IFileProvider
    {
        private readonly string _prefix;
        private const string Wwwroot = "/wwwroot";
        private readonly EmbeddedFileProvider _internalProvider;

        public EmbeddedContentFileProvider(Assembly assembly, string prefix = null)
        {
            _prefix = prefix;
            _internalProvider = new EmbeddedFileProvider(assembly);
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            var fileInfo = _internalProvider.GetFileInfo(Wwwroot + subpath);
            if (fileInfo is NotFoundFileInfo // isn't found
                && !string.IsNullOrWhiteSpace(_prefix) // and has prefix
                && subpath.StartsWith(_prefix, StringComparison.InvariantCultureIgnoreCase)) // and prefix matches
            {
                fileInfo = _internalProvider.GetFileInfo(Wwwroot + subpath.Substring(_prefix.Length));
            }
            return fileInfo;
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            var directoryContents = _internalProvider.GetDirectoryContents(Wwwroot + subpath);
            if (directoryContents is NotFoundDirectoryContents // isn't found
                && !string.IsNullOrWhiteSpace(_prefix) // and has prefix
                && subpath.StartsWith(_prefix, StringComparison.InvariantCultureIgnoreCase)) // and prefix matches
            {
                directoryContents = _internalProvider.GetDirectoryContents(Wwwroot + subpath.Substring(_prefix.Length));
            }
            return directoryContents;
        }

        public IChangeToken Watch(string filter) => NullChangeToken.Singleton; // can't change - `it's embedded
    }
}
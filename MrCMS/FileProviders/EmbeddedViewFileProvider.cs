using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace MrCMS.FileProviders
{
    public class EmbeddedViewFileProvider : IFileProvider
    {
        private const string Views = "/Views";
        private readonly EmbeddedFileProvider _internalProvider;
        public EmbeddedViewFileProvider(Assembly assembly)
        {
            _internalProvider = new EmbeddedFileProvider(assembly);
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            if (!subpath.StartsWith(Views))
                return new NotFoundFileInfo(subpath);

            return _internalProvider.GetFileInfo(subpath);
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            if (!subpath.StartsWith(Views))
                return new NotFoundDirectoryContents();

            return _internalProvider.GetDirectoryContents(subpath);
        }

        public IChangeToken Watch(string filter) => NullChangeToken.Singleton;
    }
}
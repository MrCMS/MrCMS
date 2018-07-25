using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace MrCMS.FileProviders
{
    public class EmbeddedContentFileProvider : IFileProvider
    {
        private const string Wwwroot = "/wwwroot";
        private readonly EmbeddedFileProvider _internalProvider;

        public EmbeddedContentFileProvider(Assembly assembly)
        {
            _internalProvider = new EmbeddedFileProvider(assembly);
        }

        public IFileInfo GetFileInfo(string subpath) => _internalProvider.GetFileInfo(Wwwroot + subpath);

        public IDirectoryContents GetDirectoryContents(string subpath) => _internalProvider.GetDirectoryContents(Wwwroot + subpath);

        public IChangeToken Watch(string filter) => NullChangeToken.Singleton;
    }
}
using Microsoft.Extensions.FileProviders;
using System.Reflection;

namespace MrCMS.FileProviders
{
    public class ThemeEmbeddedViewFileProvider : EmbeddedViewFileProvider
    {
        private readonly string _prefix;

        public ThemeEmbeddedViewFileProvider(Assembly assembly, string prefix) : base(assembly, prefix)
        {
            _prefix = prefix;
        }

        public override IFileInfo GetFileInfo(string subpath)
        {
            if (!subpath.StartsWith(_prefix))
            {
                return new NotFoundFileInfo(subpath);
            }

            return base.GetFileInfo(subpath);
        }

        public override IDirectoryContents GetDirectoryContents(string subpath)
        {
            if (!subpath.StartsWith(_prefix))
            {
                return new NotFoundDirectoryContents();
            }

            return base.GetDirectoryContents(subpath);
        }
    }
}
using System.Drawing;
using ImageResizer.Plugins.AnimatedGifs;
using ImageResizer.Plugins.Basic;
using ImageResizer.Plugins.PrettyGifs;

namespace MrCMS.Website
{
    public static class ImagePluginInstaller
    {
        public static void Install()
        {
            // currently we will just enable all plugins, but possibly make it configurable in the future
            new AutoRotate().Install(ImageResizer.Configuration.Config.Current);
            new AnimatedGifs().Install(ImageResizer.Configuration.Config.Current);
            new PrettyGifs().Install(ImageResizer.Configuration.Config.Current);
            var sizeLimiting = ImageResizer.Configuration.Config.Current.Plugins.Get<SizeLimiting>();
            sizeLimiting.Limits.ImageSize = new Size(20000, 20000);
            sizeLimiting.Limits.TotalSize = new Size(20000, 20000);
        }
    }
}
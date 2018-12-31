using System.Reflection;

namespace MrCMS.Themes
{
    public interface IMrCMSTheme
    {
        string Name { get; }
        string Version { get; }
        string ContentPrefix { get; set; }
        string ViewPrefix { get; set; }

        Assembly Assembly { get; }
    }
}
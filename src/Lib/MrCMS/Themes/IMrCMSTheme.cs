using System.Reflection;

namespace MrCMS.Themes
{
    public interface IMrCMSTheme
    {
        string Name { get; }
        string OutputPrefix { get; }
        string Version { get; }

        Assembly Assembly { get; }
    }
}
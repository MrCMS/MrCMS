using System.Reflection;

namespace MrCMS.Themes
{
    public abstract class StandardMrCMSTheme : IMrCMSTheme
    {
        protected StandardMrCMSTheme()
        {
        }
        public abstract string Name { get; }
        public abstract string OutputPrefix { get; }
        public abstract string Version { get;  }
        public Assembly Assembly => GetType().Assembly;
    }
}
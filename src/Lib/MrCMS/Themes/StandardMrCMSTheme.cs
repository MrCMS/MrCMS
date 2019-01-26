using System.Reflection;

namespace MrCMS.Themes
{
    public abstract class StandardMrCMSTheme : IMrCMSTheme
    {
        protected StandardMrCMSTheme()
        {
            ContentPrefix = $"/Themes/{Name}";
            ViewPrefix = $"/Themes/{Name}";
        }
        public abstract string Name { get; }
        public abstract string Version { get;  }
        public string ContentPrefix { get; set; }
        public string ViewPrefix { get; set; }
        public Assembly Assembly => GetType().Assembly;
    }
}
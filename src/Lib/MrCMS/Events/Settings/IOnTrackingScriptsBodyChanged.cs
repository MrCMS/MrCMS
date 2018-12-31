using MrCMS.Events.Documents;
using MrCMS.Settings;

namespace MrCMS.Events.Settings
{
    public interface IOnTrackingScriptsBodyChanged : IEvent<ScriptChangedEventArgs<SEOSettings>> { }
}
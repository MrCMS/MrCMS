using MrCMS.Events.Documents;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Settings.Events;

namespace MrCMS.Events.Settings
{
    public class RaiseOnTrackingScriptsBodyUpdated : IOnSavingSiteSettings<SEOSettings>
    {
        public void Execute(OnSavingSiteSettingsArgs<SEOSettings> args)
        {
            if (args.Original?.TrackingScriptsBody == args.Settings?.TrackingScriptsBody)
                return;

            EventContext.Instance.Publish<IOnTrackingScriptsBodyChanged, ScriptChangedEventArgs<SEOSettings>>(
                new ScriptChangedEventArgs<SEOSettings>(args.Settings, args.Settings?.TrackingScriptsBody,
                    args.Original?.TrackingScriptsBody));
        }
    }
}
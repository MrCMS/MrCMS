using MrCMS.Events.Documents;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Settings.Events;

namespace MrCMS.Events.Settings
{
    public class RaiseOnTrackingScriptsUpdated : IOnSavingSiteSettings<SEOSettings>
    {
        public void Execute(OnSavingSiteSettingsArgs<SEOSettings> args)
        {
            if (args.Original?.TrackingScripts == args.Settings?.TrackingScripts)
                return;

            EventContext.Instance.Publish<IOnTrackingScriptsChanged, ScriptChangedEventArgs<SEOSettings>>(
                new ScriptChangedEventArgs<SEOSettings>(args.Settings, args.Settings?.TrackingScripts,
                    args.Original?.TrackingScripts));
        }
    }
}
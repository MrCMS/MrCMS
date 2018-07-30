using MrCMS.Events.Documents;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Settings.Events;

namespace MrCMS.Events.Settings
{
    public class RaiseOnTrackingScriptsUpdated : IOnSavingSiteSettings<SEOSettings>
    {
        private readonly IEventContext _eventContext;

        public RaiseOnTrackingScriptsUpdated(IEventContext eventContext)
        {
            _eventContext = eventContext;
        }
        public void Execute(OnSavingSiteSettingsArgs<SEOSettings> args)
        {
            if (args.Original?.TrackingScripts == args.Settings?.TrackingScripts)
                return;

            _eventContext.Publish<IOnTrackingScriptsChanged, ScriptChangedEventArgs<SEOSettings>>(
                new ScriptChangedEventArgs<SEOSettings>(args.Settings, args.Settings?.TrackingScripts,
                    args.Original?.TrackingScripts));
        }
    }
}
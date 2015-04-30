using MrCMS.Events;

namespace MrCMS.Settings.Events
{
    public interface IOnSavingSiteSettings<T> : IEvent<OnSavingSiteSettingsArgs<T>> where T : SiteSettingsBase
    {

    }
}
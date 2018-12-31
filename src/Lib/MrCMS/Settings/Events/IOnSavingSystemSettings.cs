using MrCMS.Events;

namespace MrCMS.Settings.Events
{
    public interface IOnSavingSystemSettings<T> : IEvent<OnSavingSystemSettingsArgs<T>> where T : SystemSettingsBase
    {
    }
}
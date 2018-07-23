using MrCMS.Events;

namespace MrCMS.Settings.Events
{
    public interface IOnSavingSystemSettings<T> : IEvent<OnSavingSystemSettingsArgs<T>> where T : SystemSettingsBase
    {
    }

    public class SetEnableOptimisations : IOnSavingSystemSettings<BundlingSettings>
    {
        public void Execute(OnSavingSystemSettingsArgs<BundlingSettings> args)
        {
            // TODO: look at bundling
            //BundleTable.EnableOptimizations = args.Settings.EnableOptimisations;
        }
    }
}
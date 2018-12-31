namespace MrCMS.Settings.Events
{
    public class SetEnableOptimisations : IOnSavingSystemSettings<BundlingSettings>
    {
        public void Execute(OnSavingSystemSettingsArgs<BundlingSettings> args)
        {
            // TODO: look at bundling
            //BundleTable.EnableOptimizations = args.Settings.EnableOptimisations;
        }
    }
}
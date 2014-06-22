namespace MrCMS.Settings
{
    public interface ILegacySettingsProvider
    {
        void ApplyLegacySettings<TSettings>(TSettings settings, int siteId) where TSettings : SiteSettingsBase;
    }
}
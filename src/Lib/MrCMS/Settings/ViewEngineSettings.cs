namespace MrCMS.Settings
{
    public class ViewEngineSettings : SystemSettingsBase
    {
        public ViewEngineSettings()
        {
#if DEBUG
            UsePhysicalFilesIfNewer = true;
            UseRazorGeneratorPrecompiledViewEngine = false;
#else
            UsePhysicalFilesIfNewer = false;
            UseRazorGeneratorPrecompiledViewEngine = true;
#endif
        }
        public bool UseRazorGeneratorPrecompiledViewEngine { get; set; }
        public bool UsePhysicalFilesIfNewer { get; set; }
    }
}
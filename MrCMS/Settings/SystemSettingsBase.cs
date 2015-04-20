using MrCMS.Services;

namespace MrCMS.Settings
{
    public abstract class SystemSettingsBase : IStoredInAppData
    {
        public virtual bool RenderInSettings { get { return false; } }
    }
}
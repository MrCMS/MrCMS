namespace MrCMS.Settings.Events
{
    public class OnSavingSiteSettingsArgs<T> where T : SiteSettingsBase
    {
        private readonly T _settings;
        private readonly T _original;

        public OnSavingSiteSettingsArgs(T settings, T original)
        {
            _settings = settings;
            _original = original;
        }

        public T Settings
        {
            get { return _settings; }
        }

        public T Original
        {
            get { return _original; }
        }
    }
}
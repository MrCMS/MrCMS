namespace MrCMS.Settings.Events
{
    public class OnSavingSystemSettingsArgs<T> where T : SystemSettingsBase
    {
        private readonly T _settings;
        private readonly T _original;

        public OnSavingSystemSettingsArgs(T settings, T original)
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
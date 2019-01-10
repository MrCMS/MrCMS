namespace MrCMS.Settings.Events
{
    public class OnSavingSystemSettingsArgs<T> where T : SystemSettingsBase
    {
        public OnSavingSystemSettingsArgs(T settings, T original)
        {
            Settings = settings;
            Original = original;
        }

        public T Settings { get; }

        public T Original { get; }
    }
}
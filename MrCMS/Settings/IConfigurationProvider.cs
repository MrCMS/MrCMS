using Ninject.Activation;

namespace MrCMS.Settings
{
    public interface IConfigurationProvider<TSettings> : IProvider<TSettings> where TSettings : ISettings, new()
    {
        TSettings Settings { get; }
        void SaveSettings(TSettings settings);
        void DeleteSettings();
    }
}
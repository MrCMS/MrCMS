using System.Collections.Generic;
using MrCMS.Entities.Multisite;
using Ninject.Activation;

namespace MrCMS.Settings
{
    public interface IConfigurationProvider
    {
        TSettings GetSettings<TSettings>(Site site) where TSettings : ISettings, new();
        void SaveSettings(ISettings settings);
        void DeleteSettings(Site site, ISettings settings);
        List<ISettings> GetAllISettings(Site site);
    }
}
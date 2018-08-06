using System.Collections.Generic;
using System.Globalization;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.Resources;
//using MrCMS.Services.Caching;

namespace MrCMS.Services.Resources
{
    // TODO: clear cache
    public interface IStringResourceProvider //: IClearCache
    {
        IEnumerable<StringResource> AllResources { get; }
        string GetValue(string key, string defaultValue = null);
        string GetValueForCulture(string key, CultureInfo cultureInfo, string defaultValue = null);
        IEnumerable<string> GetOverriddenLanguages();
        IEnumerable<string> GetOverriddenLanguages(string key, Site site);
        void Insert(StringResource resource);
        void AddOverride(StringResource resource);
        void Update(StringResource resource);
        void Delete(StringResource resource);
    }
}
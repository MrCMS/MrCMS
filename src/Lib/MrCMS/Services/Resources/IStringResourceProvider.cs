using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using MrCMS.Entities.Resources;
using MrCMS.Services.Caching;


namespace MrCMS.Services.Resources
{
    public interface IStringResourceProvider : IClearCache
    {
        Task<IEnumerable<StringResource>> GetAllResources();
        Task<string> GetValue(string key, string defaultValue = null);
        Task<string> GetValueForCulture(string key, CultureInfo cultureInfo, string defaultValue = null);
        Task<IEnumerable<string>> GetOverriddenLanguages();
        Task<IEnumerable<string>> GetOverriddenLanguages(string key, int? siteId);
        Task Insert(StringResource resource);
        Task AddOverride(StringResource resource);
        Task Update(StringResource resource);
        Task Delete(StringResource resource);
    }
}
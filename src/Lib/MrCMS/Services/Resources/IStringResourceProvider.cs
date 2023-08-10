using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using MrCMS.Entities.Resources;
using MrCMS.Services.Caching;


namespace MrCMS.Services.Resources
{
    public interface IStringResourceProvider : IClearCache
    {
        Task<IEnumerable<StringResource>> GetAllResources();

        Task<IHtmlContent> GetValue(string key, Action<ResourceOptions> configureOptions);
        // Task<IHtmlContent> GetValueForCulture(string key, CultureInfo cultureInfo, ResourceOptions options);

        Task<string> GetValue(string key, Action<PlainResourceOptions> configureOptions = null);
        // Task<string> GetValueForCulture(string key, CultureInfo cultureInfo, PlainResourceOptions options);

        Task<IEnumerable<string>> GetOverriddenLanguages();
        Task<IEnumerable<string>> GetOverriddenLanguages(string key, int? siteId);
        Task Insert(StringResource resource);
        Task AddOverride(StringResource resource);
        Task Update(StringResource resource);
        Task Delete(StringResource resource);
    }
}

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using MrCMS.Helpers;
using MrCMS.Settings;

namespace MrCMS.Services.Resources
{
    public interface IGetSystemCultureInfo
    {
        IList<CultureInfo> GetSupportedCultures();
        IEnumerable<SelectListItem> GetCultureOptions();
    }

    public class GetSystemCultureInfo : IGetSystemCultureInfo
    {
        private readonly IOptions<SystemConfigurationSettings> _config;

        public GetSystemCultureInfo(IOptions<SystemConfigurationSettings> config)
        {
            _config = config;
        }
        public IList<CultureInfo> GetSupportedCultures()
        {
            var supportedCultures = _config.Value.SupportedCultures;
            var filteredSupportedCultures = CultureInfo.GetCultures(CultureTypes.AllCultures)
                .Where(x=>supportedCultures.Contains(x.Name))
                .OrderBy(info => info.DisplayName).ToList();
            return filteredSupportedCultures;
        }

        public IEnumerable<SelectListItem> GetCultureOptions(){ 
            return GetSupportedCultures()
            .OrderBy(info => info.DisplayName)
            .BuildSelectItemList(info => info.DisplayName, info => info.Name, emptyItem: null);
        }
    }
}

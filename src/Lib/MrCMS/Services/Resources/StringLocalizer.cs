using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Localization;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;

namespace MrCMS.Services.Resources
{
    public class StringLocalizer : IStringLocalizer
    {
        private readonly IList<LocalizationInfo> _allRecords;
        private readonly CultureInfo _culture;
        private readonly Site _site;
        private readonly Func<MissingLocalisationInfo, MissingLocalisationResult> _missingLocalisation;

        private readonly Dictionary<string, string> _cultureRecords;

        public StringLocalizer(IList<LocalizationInfo> allRecords, CultureInfo culture, Site site,
            Func<MissingLocalisationInfo, MissingLocalisationResult> missingLocalisation)
        {
            _allRecords = allRecords;
            _culture = culture;
            _site = site;
            _missingLocalisation = missingLocalisation;
            _cultureRecords = allRecords.Where(x => x.Culture == culture.Name || !x.Culture.HasValue())
                .GroupBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(x => x.Key,
                    // if there's one for the passed site, use that, otherwise use the first one you find
                    x => _site != null && x.Any(y => y.SiteId == _site.Id)
                        ? x.First(y => y.SiteId == _site?.Id).Value
                        : x.FirstOrDefault(y => !y.SiteId.HasValue)?.Value, StringComparer.OrdinalIgnoreCase)
                // filter to where the value isn't null
                .Where(x => x.Value.HasValue())
                .ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return _cultureRecords.Select(pair => new LocalizedString(pair.Key, pair.Value));
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return new StringLocalizer(_allRecords, culture, _site, _missingLocalisation);
        }

        public LocalizedString this[string name]
        {
            get
            {
                var text = GetValue(name, out bool resourceNotFound);
                return new LocalizedString(name, text, resourceNotFound);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var text = string.Format(GetValue(name, out bool resourceNotFound), arguments);
                return new LocalizedString(name, text, resourceNotFound);
            }
        }

        private string GetValue(string key, out bool resourceNotFound)
        {
            resourceNotFound = true;
            if (!_cultureRecords.ContainsKey(key))
            {
                var result = _missingLocalisation(new MissingLocalisationInfo { Key = key,
                    //Culture = _culture
                });
                if (result.Localization != null)
                    _allRecords.Add(result.Localization);
                _cultureRecords.Add(key, result.Value);
                return result.Value;
            }

            resourceNotFound = false;
            return _cultureRecords[key];
        }
    }
}
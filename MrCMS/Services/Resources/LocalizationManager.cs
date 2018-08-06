using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.Resources;

namespace MrCMS.Services.Resources
{
    public class LocalizationManager : ILocalizationManager
    {
        private readonly IGetDefaultResourceValue _getDefaultResourceValue;

        private readonly IStringResourceProvider _stringResourceProvider;

        public LocalizationManager(
            IGetDefaultResourceValue getDefaultResourceValue,
            IStringResourceProvider stringResourceProvider
        )
        {
            _getDefaultResourceValue = getDefaultResourceValue;
            _stringResourceProvider = stringResourceProvider;
        }


        public IList<LocalizationInfo> GetLocalizations()
        {
            return _stringResourceProvider.AllResources.Select(resource => new LocalizationInfo
            {
                Key = resource.Key,
                Value = resource.Value,
                Culture = resource.UICulture,
                SiteId = resource.Site?.Id
            }).ToList();
        }

        public MissingLocalisationResult HandleMissingLocalization(MissingLocalisationInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            var defaultValue = _getDefaultResourceValue.GetValue(info);

            _stringResourceProvider.Insert(new StringResource
            {
                Key = info.Key,
                //UICulture = info.Culture.Name,
                Value = defaultValue
            });

            return new MissingLocalisationResult
            {
                Localization =
                    new LocalizationInfo
                    {
                        //Culture = info.Culture.Name,
                        Key = info.Key,
                        Value = defaultValue
                    },
                Value = defaultValue
            };
        }
    }
}
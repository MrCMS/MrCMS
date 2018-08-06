using System.Collections.Generic;
using MrCMS.Entities.Multisite;

namespace MrCMS.Services.Resources
{
    public interface ILocalizationManager
    {
        IList<LocalizationInfo> GetLocalizations();
        MissingLocalisationResult HandleMissingLocalization(MissingLocalisationInfo info);
    }
}
using System;
using MrCMS.Entities.Multisite;

namespace MrCMS.Indexing.Management
{
    static internal class AzureDirectoryHelper
    {
        public static string GetAzureCatalogName(Site site, string indexFolderName)
        {
            return String.Format("Indexes-{0}-{1}", site.Id, indexFolderName.Replace(" ", ""));
        }
    }
}
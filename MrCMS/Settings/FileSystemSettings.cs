using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Services;

namespace MrCMS.Settings
{
    public class FileSystemSettings : SiteSettingsBase
    {
        public FileSystemSettings()
        {
            AzureUsingEmulator = true;
            AzureContainerName = "MrCMS";
            StorageType = typeof(FileSystem).FullName;
        }
        [DisplayName("Storage Type")]
        public string StorageType { get; set; }

        [DisplayName("Azure Storage Account Name")]
        public string AzureAccountName { get; set; }
        [DisplayName("Azure Storage Account Key")]
        public string AzureAccountKey { get; set; }
        [DisplayName("Azure Storage Container Name")]
        public string AzureContainerName { get; set; }
        [DisplayName("Azure is using Emulator")]
        public bool AzureUsingEmulator { get; set; }
        [DisplayName("Azure CDN Domain")]
        public string AzureCdnDomain { get; set; }

        [DisplayName("Use Azure for Lucene")]
        public bool UseAzureForLucene { get; set; }

        public List<SelectListItem> StorageTypeOptions
        {
            get
            {
                var types = TypeHelper.GetAllConcreteTypesAssignableFrom<IFileSystem>();

                return types.BuildSelectItemList(type => type.Name, type => type.FullName, type => type.FullName == StorageType,
                    emptyItem: null);
            }
        }

        public override bool RenderInSettings { get { return false; } }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MrCMS.Settings
{
    public class WebExtensionsToRoute : SystemSettingsBase
    {
        public WebExtensionsToRoute()
        {
            Extensions = ".asp,.php,.aspx";
        }
        [DisplayName("Page extensions you want Mr CMS to handle")]
        [AppSettingName("file-extentions-to-handle")]
        public string Extensions { get; set; }
        
        public IEnumerable<string> Get => (Extensions ?? string.Empty).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
    }
}
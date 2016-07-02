using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MrCMS.Settings
{
    public class WebExtensionsToRoute : SystemSettingsBase
    {
        public WebExtensionsToRoute()
        {
            PageExtensionsToRoute = ".asp,.php,.aspx";
        }
        [DisplayName("Page extensions you want Mr CMS to handle")]
        public string PageExtensionsToRoute { get; set; }

        public IEnumerable<string> Get => (PageExtensionsToRoute ?? string.Empty).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
    }
}
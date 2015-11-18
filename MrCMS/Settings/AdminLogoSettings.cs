using System;
using System.ComponentModel;


namespace MrCMS.Settings
{
    public class AdminLogoSettings: SiteSettingsBase
    {
       
        public AdminLogoSettings()
        {

        }

        [DisplayName("MrCMS Admin Logo")]
        public string MrCMSAdminLogo { get; set; }

        public override bool RenderInSettings
        {
            get { return false; }
        }

     
    }
}
using System.ComponentModel;

namespace MrCMS.Settings
{
    public class MailSettings : SystemSettingsBase
    {
        public MailSettings()
        {
            Port = 25;
        }
        [DisplayName("System Email Address")]
        public string SystemEmailAddress { get; set; }

        public string Host { get; set; }

        public bool UseSSL { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public int Port { get; set; }

        //public override bool RenderInSettings
        //{
        //    get { return true; }
        //}
    }
}
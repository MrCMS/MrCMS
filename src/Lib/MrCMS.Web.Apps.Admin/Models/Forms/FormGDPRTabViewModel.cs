using System.ComponentModel;

namespace MrCMS.Web.Apps.Admin.Models.Forms
{
    public class FormGDPRTabViewModel
    {
        public int Id { get; set; }


        [DisplayName("Delete entries after (days)")]
        public virtual int? DeleteEntriesAfter { get; set; }

        [DisplayName("Send by email only?")]
        public virtual bool SendByEmailOnly { get; set; }

        [DisplayName("Show GDPR consent box?")]
        public virtual bool ShowGDPRConsentBox { get; set; }
    }
}
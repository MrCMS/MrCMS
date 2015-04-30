using MrCMS.Entities.Multisite;

namespace MrCMS.Web.Areas.Admin.Models
{
    public class ChooseSiteParams
    {
        public string Key { get; set; }

        public bool Language { get; set; }

        public int Id { get; set; }
    }
}
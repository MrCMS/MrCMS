using NHibernate;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MrCMS.Entities.Documents.Web.FormProperties
{
    public class FormListOption : SiteEntity
    {
        public virtual FormPropertyWithOptions FormProperty { get; set; }
        [Required]
        public virtual string Value { get; set; }
        public virtual bool Selected { get; set; }

        public virtual async Task OnSaving(ISession session)
        {
            if (Selected && FormProperty.OnlyOneOptionSelectable)
            {
                foreach (var option in FormProperty.Options.Except(new[] { this }))
                {
                    option.Selected = false;
                    await session.UpdateAsync(option);
                }
            }
            else if (FormProperty.OnlyOneOptionSelectable && !FormProperty.Options.Except(new[] { this }).Any())
            {
                Selected = true;
            }
        }
    }
}
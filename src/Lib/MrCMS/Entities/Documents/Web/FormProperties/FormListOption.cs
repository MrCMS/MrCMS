using NHibernate;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MrCMS.Entities.Documents.Web.FormProperties
{
    public class FormListOption : SiteEntity
    {
        public virtual FormPropertyWithOptions FormProperty { get; set; }
        [Required]
        public virtual string Value { get; set; }
        public virtual bool Selected { get; set; }

        public virtual void OnSaving(ISession session)
        {
            if (Selected && FormProperty.OnlyOneOptionSelectable)
            {
                foreach (var option in FormProperty.Options.Except(new[] { this }))
                {
                    option.Selected = false;
                    session.Update(option);
                }
            }
            else if (FormProperty.OnlyOneOptionSelectable && !FormProperty.Options.Except(new[] { this }).Any())
            {
                Selected = true;
            }
        }
    }
}